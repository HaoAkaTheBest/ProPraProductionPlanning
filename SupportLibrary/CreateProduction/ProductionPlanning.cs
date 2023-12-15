
using System.Reflection.PortableExecutable;
using System.Transactions;

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

            DateTime startDate = order.EarliestStartDate.AddHours(6); // startDate for the whole production
            DateTime endDate;
            DateTime usingDate = order.EarliestStartDate.AddHours(6); // the date that will be used to calculate everything with an assumption that working day starts at 6 and ends at 22

            List<IRoutingModel> steps = await _routingData.ReadRoutingForOneProduct(order.ProductId);

            foreach (var step in steps)
            {
                IMachineModel machine = await _machineData.ReadMachine(step.MachineId); // machine for this step
                string descriptionForAvailability; // description for machineAvailability
                bool isAvailable;
                bool isTheMachineNotUsed;
                IMachineUsedModel thisMachineIsUsedInOtherOrder; // give the machine and the time when the machine is used

                int delayedDays = 0;
                List<string> reasonsForDelay = new();

                double durationForThisStep = 0.0;
                double totalWaitingTimeForThisStep = 0.0; // waitting time when the machine is used for other orders

                bool isThisStepAfter22 = false;

                double stepTime = (step.ProcessTimeInSeconds + step.SetupTimeInSeconds) / 3600.0; //calculate stepTime in hour
                if (machine.Effectivity != 0)
                {
                    stepTime = (stepTime / machine.Effectivity) * 100;  // calculate the process corresponding to the effectivity of the machine
                }

                durationForThisStep += stepTime;// add processing time

                do
                {
                    (isAvailable, descriptionForAvailability) = await CheckMachineAvailability(machine.Id, usingDate); // check if the machine is available on this day

                    // if the machine is not available, move to next day and check again till it is available
                    while (isAvailable == false)
                    {
                        //delayedDays += 1; // delayed startdate
                        //reasonsForDelay.Add(descriptionForAvailability);
                        if (step.StepId == 1)
                        {
                            startDate = startDate.AddDays(1); // update the startDate if the first step is on company holidays, weekend or maintenance
                        }
                        else
                        {
                            totalWaitingTimeForThisStep += 24.0; // add 24 hours of waiting if is not first step
                        }
                        usingDate = usingDate.AddDays(1); // check if the next day is not company holidays, weekend or maintenance

                        (isAvailable, descriptionForAvailability) = await CheckMachineAvailability(machine.Id, usingDate); // check again
                    }

                    (isTheMachineNotUsed, thisMachineIsUsedInOtherOrder) = await CheckIfNotUsedMachine(order.Id, machine.Id, usingDate, usingDate.AddHours(stepTime)); // check if the machine is already being used for other orders

                    int swapMachinecount = 0; // can only swap the machine once

                    while (isTheMachineNotUsed == false)
                    {
                        if (machine.MachineAlternativityGroup != 0 && swapMachinecount == 0)
                        {
                            var newMachine = await CheckMachineAlternativity(machine.Id, machine.MachineAlternativityGroup);
                            (isTheMachineNotUsed, thisMachineIsUsedInOtherOrder) = await CheckIfNotUsedMachine(order.Id, newMachine.Id, usingDate, usingDate.AddHours(stepTime));
                            swapMachinecount += 1;
                            if (isTheMachineNotUsed) // if there is an alternate machine then dont have to wait
                            {
                                machine = newMachine;
                                break;
                            }
                        }

                        double waitOnOccupiedMachine = ((thisMachineIsUsedInOtherOrder.EndTime - usingDate).TotalSeconds / 3600.0);

                        totalWaitingTimeForThisStep += waitOnOccupiedMachine; // calculate waitingTime when the machine is already occupied

                        usingDate = usingDate.AddHours(waitOnOccupiedMachine);

                        (isTheMachineNotUsed, thisMachineIsUsedInOtherOrder) = await CheckIfNotUsedMachine(order.Id, machine.Id, usingDate, usingDate.AddHours(stepTime)); // LOGIC HERE HAS TO BE OPTIMIZED
                    }

                    if (step.StepId == 1)
                    {
                        startDate = usingDate; // update start time
                    }

                    if ((usingDate.AddHours(stepTime).TimeOfDay >= TimeSpan.FromHours(22)) || (usingDate.AddHours(stepTime).TimeOfDay <= TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(59)))) /*endtime should not between 6 and 22*/   
                    {
                        // if the step takes until after 22:00 than delays to next day
                        var tempDate = new DateTime(usingDate.Year, usingDate.Month, usingDate.Day).AddDays(1).AddHours(6);

                        totalWaitingTimeForThisStep += ((tempDate - usingDate).TotalSeconds / 3600.0); // wait till next day

                        usingDate = tempDate;
                        isThisStepAfter22 = true;
                    }
                    else if (usingDate.TimeOfDay >= TimeSpan.FromHours(22) && usingDate.TimeOfDay <= new TimeSpan(23, 59, 59) /*starttime should not between 22:00pm and 23:59pm*/)
                    {
                        var tempDate = new DateTime(usingDate.Year, usingDate.Month, usingDate.Day).AddDays(1).AddHours(6);

                        totalWaitingTimeForThisStep += ((tempDate - usingDate).TotalSeconds / 3600.0); // wait till next day

                        usingDate = tempDate;
                        isThisStepAfter22 = true;
                    }
                    else if (usingDate.TimeOfDay < TimeSpan.FromHours(6) && usingDate.TimeOfDay >= TimeSpan.FromHours(0) /*starttime should not between 12:00am and 6:00 am*/)
                    {
                        var tempDate = new DateTime(usingDate.Year, usingDate.Month, usingDate.Day).AddHours(6);

                        totalWaitingTimeForThisStep += ((tempDate - usingDate).TotalSeconds / 3600.0); // wait till next day

                        usingDate = tempDate;
                        isThisStepAfter22 = true;
                    }
                    else
                    {
                        isThisStepAfter22 = false;
                    }

                } while (/*usingDate.AddHours(stepTime).Hour >= 22*/ isThisStepAfter22);

                //if (delayedDays >0)
                //{
                //    Note = $"delayed {delayedDays} days";

                //    reasonsForDelay = new HashSet<string>(reasonsForDelay).ToList(); // remove duplicate

                //    foreach (var reason in reasonsForDelay)
                //    {
                //        Note += $" {reason}";
                //    }
                    
                //}

                // when everything is okay, then write this step into databank and move to next step
                if (isAvailable && isTheMachineNotUsed)
                {
                    durationForThisStep += totalWaitingTimeForThisStep; // add awaiting time to this machine time

                    await _machineUsedData.CreateMachineUsed(order.Id, machine.Id, usingDate, usingDate.AddHours(stepTime)); // usingDate and usingDate.AddHours(stepTime) means the time when the process acutally takes place

                    usingDate = usingDate.AddHours(stepTime); // calculate usingDate(startTime) for the next step

                    tempDuration += durationForThisStep;
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
                orderProcess.Note = $"{endDate.Subtract(order.Deadline).Days} days after Deadline; " + Note;
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

        private async Task<IMachineModel> CheckMachineAlternativity(int machineId, int alternativityGroup)
        {
            var newMachine = await _machineData.ReadMachineAlternativity(machineId, alternativityGroup);

            return newMachine;
        }

        private async Task<(bool, IMachineUsedModel)> CheckIfNotUsedMachine(int orderId, int machineid, DateTime startTime, DateTime endTime)
        {
            bool isNotUsed = false;
            var usedMachine = await _machineUsedData.ReadMachineUsedInThisTime(machineid, startTime, endTime);//this machine is being used
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

            return (isNotUsed, usedMachine);
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

        private async Task<(bool, string)> CheckMachineAvailability(int machineId, DateTime startDate)
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
            return (isAvailable, description);
        }

    }
}
