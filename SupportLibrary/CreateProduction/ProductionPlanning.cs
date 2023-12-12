
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
                    ProductId = $"{order.ProductId}",
                    OrderDate = order.OrderDate,
                    Deadline = order.Deadline,
                    EarliestStartDate = process.StartDate,
                    Duration = process.Duration,
                    Progress = process.Progress,
                    EndDate = process.EndDate,
                    Note = process.Note,
                    Status = process.Status,
                    DurationUnit = "hour"
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

            DateTime startDate = order.EarliestStartDate;
            DateTime endDate = order.EarliestStartDate;
            DateTime usingDate = startDate;

            List<IRoutingModel> steps = await _routingData.ReadRoutingForOneProduct(order.ProductId);

            foreach (var step in steps)
            {
                string descriptionForAvailability;
                bool isAvailable;
                bool isTheMachineUsed;
                IMachineUsedModel usedModel;
                double durationForThisMachine = 0.0;

                var machine = await _machineData.ReadMachine(step.MachineId);
                (isAvailable,descriptionForAvailability) = await CheckMachineAvailability(step.MachineId, usingDate);
                (isTheMachineUsed, usedModel) = await CheckIfNotUsedMachine(step.MachineId, usingDate);

 
                while (isAvailable == false)
                {
                    if (step.StepId == 1)
                    {
                        Note += $"1 day: {descriptionForAvailability}; "; // add note
                        startDate = startDate.AddDays(1); // check if available in the next day
                    }
                    usingDate = usingDate.AddDays(1); // check if available in the next day

                    (isAvailable, descriptionForAvailability) = await CheckMachineAvailability(step.MachineId, usingDate);
                }

                while (isTheMachineUsed == true)
                {
                    durationForThisMachine += ((usedModel.EndTime - usedModel.StartTime).TotalSeconds / 3600.0);
                    usingDate = usedModel.EndTime;
                    (isTheMachineUsed, usedModel) = await CheckIfNotUsedMachine(step.MachineId, usingDate);
                }


                if (isAvailable && !isTheMachineUsed)
                {
                    double stepTime = (step.ProcessTimeInSeconds + step.SetupTimeInSeconds) / 3600.0;

                    durationForThisMachine += stepTime;

                    await _machineUsedData.CreateMachineUsed(step.MachineId, usingDate, usingDate.AddHours(stepTime));
                    usingDate = usingDate.AddHours(stepTime);
                    endDate = usingDate.AddHours(stepTime);
                    tempDuration += durationForThisMachine;
                }
                else
                {
                    throw new Exception("Problem with Machine Availability");
                }

            }
            duration = tempDuration.ToString("F2");
            //endDate = startDate.AddHours(tempDuration);

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
            if (order.Deadline <= order.EarliestStartDate)
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

        private async Task<(bool,IMachineUsedModel)> CheckIfNotUsedMachine(int machineid, DateTime usingDate)
        {
            bool isUsed = false;
            var machineIsUsed = await _machineUsedData.ReadMachineUsedInThisTime(machineid, usingDate);
            if (machineIsUsed == null)
            {
                isUsed = false;
            }
            else
            {
                isUsed = true;
            }

            return (isUsed,machineIsUsed);
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
            var availability = await _maData.ReadMachineAvailabilityForProduction(machineId, startDate.AddHours(7));
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
