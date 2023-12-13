
using System.Reflection.PortableExecutable;

namespace SupportLibrary.CreateProduction
{
    public class ProductionPlanning : IProductionPlanning
    {
        private readonly IMachineAvailabilitySqlDataService _maData;
        private readonly IOrderSqlDataService _orderData;
        private readonly IMachineSqlDataService _machineData;
        private readonly IRoutingSqlDataService _routingData;
        private readonly IMachineUsedSqlDataService _machineUsedData;

        public ProductionPlanning(IMachineAvailabilitySqlDataService maData, IOrderSqlDataService orderData, IMachineSqlDataService machineData, IRoutingSqlDataService routingData, IMachineUsedSqlDataService machineUsedData)
        {
            _maData = maData;
            _orderData = orderData;
            _machineData = machineData;
            _routingData = routingData;
            _machineUsedData = machineUsedData;
        }
        public async Task<List<ITaskData>> CreateProductionPlan()
        {
            List<IOrderModel> orders = await _orderData.ReadOrder();

            List<ITaskData> output = new();

            foreach (var order in orders)
            {
                IOrderProcessModel process = await CalculateDuration(order);
                output.Add(new TaskData
                {
                    TaskId = order.Id,
                    TaskName = $"{order.ProductId}",
                    OrderDate = order.OrderDate,
                    Deadline = order.Deadline,
                    StartDate = process.StartDate,
                    Duration = process.Duration,
                    Progress = process.Progress,
                    EndDate = process.EndDate,
                    Note = process.Note,
                    Status = process.Status,
                    DurationUnit = "minute"
                });
            }

            return output;
        }

        private async Task<IOrderProcessModel> CalculateDuration(IOrderModel order)
        {
            string duration;
            string Note = "";

            double tempDuration = 0.0;
            OrderProcessModel orderProcess = new();

            DateTime startDate = order.EarliestStartDate.AddHours(6);
            DateTime endDate;
            DateTime usingDate = order.EarliestStartDate.AddHours(6);

            List<IRoutingModel> steps = await _routingData.ReadRoutingForOneProduct(order.ProductId);

            foreach (var step in steps)
            {
                IMachineModel machine = await _machineData.ReadMachine(step.MachineId);
                string descriptionForAvailability;
                bool isAvailable;
                bool isTheMachineNotUsed;
                IMachineUsedModel usedMachine;
                double durationForThisMachine = 0.0;


                (isAvailable,descriptionForAvailability) = await CheckMachineAvailability(machine.Id, usingDate);

                while (isAvailable == false)
                {
                    Note += $"1 day: {descriptionForAvailability}; "; // add note
                    if (step.StepId == 1)
                    {
                        startDate = startDate.AddDays(1); // check if available in the next day
                    }
                    usingDate = usingDate.AddDays(1); // check if available in the next day

                    (isAvailable, descriptionForAvailability) = await CheckMachineAvailability(machine.Id, usingDate);
                }

                (isTheMachineNotUsed, usedMachine) = await CheckIfNotUsedMachine(order.Id, machine.Id, usingDate);

                int swapMachinecount = 0;

                while (isTheMachineNotUsed == false)
                {
                    if (machine.MachineAlternativityGroup !=0 && swapMachinecount==0)
                    {
                        var newMachine = await CheckMachineAlternativity(machine.Id,machine.MachineAlternativityGroup);
                        (isTheMachineNotUsed, usedMachine) = await CheckIfNotUsedMachine(order.Id, newMachine.Id, usingDate);
                        swapMachinecount += 1;
                        if (isTheMachineNotUsed)
                        {
                            machine = newMachine;
                            continue;
                        }
                    }
                    double waitingTime = ((usedMachine.EndTime - usingDate).TotalSeconds / 3600.0);
                    durationForThisMachine += waitingTime; // add awaiting time to this machine time
                    usingDate = usingDate.AddHours(waitingTime); // add waiting time to total process
                    if (step.StepId == 1)
                    {
                        startDate = usingDate; // update start time
                    }
                    (isTheMachineNotUsed, usedMachine) = await CheckIfNotUsedMachine(order.Id, machine.Id, usingDate);
                }


                if (isAvailable && isTheMachineNotUsed)
                {
                    double stepTime = (step.ProcessTimeInSeconds + step.SetupTimeInSeconds) / 3600.0;
                    if (machine.Effectivity !=0)
                    {
                        stepTime = (stepTime * machine.Effectivity) / 100;  // calculate the process corresponding to the effectivity of the machine
                    }


                    durationForThisMachine += stepTime;// add processing time

                    await _machineUsedData.CreateMachineUsed(order.Id ,machine.Id, usingDate, usingDate.AddHours(stepTime));
                    usingDate = usingDate.AddHours(stepTime);
                    tempDuration += durationForThisMachine;
                }
                else
                {
                    throw new Exception("Problem with Machine Availability");
                }

            }
            
            duration = (tempDuration * 60).ToString("F2");
            endDate = usingDate;

            orderProcess.Duration = duration;

            if (DateTime.Now <= startDate)
            {
                orderProcess.Status = "Not Started";
            }
            else if (DateTime.Now >= startDate.AddHours(tempDuration))
            {
                orderProcess.Status = "Finished";
            }
            else
            {
                orderProcess.Status = "In Production";
            }
            if (order.Deadline <= endDate)
            {
                orderProcess.Note = "After Deadline; " + Note ;
            }
            else
            {
                orderProcess.Note = Note;
            }
            orderProcess.StartDate = startDate;

            orderProcess.Progress = (int)await CalculateProgress(endDate, startDate);

            orderProcess.EndDate = endDate;

            return orderProcess;

        }

        private async Task<IMachineModel> CheckMachineAlternativity(int machineId,int alternativityGroup)
        {
            var newMachine = await _machineData.ReadMachineAlternativity(machineId,alternativityGroup);

            return newMachine;
        }

        private async Task<(bool,IMachineUsedModel)> CheckIfNotUsedMachine(int orderId ,int machineid, DateTime usingDate)
        {
            bool isNotUsed = false;
            var usedMachine = await _machineUsedData.ReadMachineUsedInThisTime(machineid, usingDate);//this machine is being used
            if (usedMachine == null)
            {
                isNotUsed = true;
            }
            else if (usedMachine.OrderId == orderId)
            {
                isNotUsed = true;
            }
            else
            {
                isNotUsed = false;
            }

            return (isNotUsed,usedMachine);
        }

        private async Task<double> CalculateProgress(DateTime endDate, DateTime startDate)
        {
            DateTime currentDate = DateTime.Now;
            TimeSpan totalTimeSpan = endDate - startDate;
            TimeSpan elapsedTimeSpan = currentDate - startDate;

            double progress = (elapsedTimeSpan.TotalSeconds / totalTimeSpan.TotalSeconds) * 100;
            progress = Math.Min(Math.Max(progress, 0), 100);
            return progress;
        }

        private async Task<(bool,string)> CheckMachineAvailability(int machineId, DateTime startDate)
        {
            string description = "";
            bool isAvailable = false;
            var availability = await _maData.ReadMachineAvailabilityForProduction(machineId, startDate);
            if (availability == null)
            {
                isAvailable = true;
            }
            else
            {
                description = availability.Description;
            }
            return (isAvailable,description);
        }
    }
}
