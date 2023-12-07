
namespace SupportLibrary.CreateProduction
{
    public class ProductionPlanning : IProductionPlanning
    {
        private readonly IMachineAvailabilitySqlDataService _maData;
        private readonly IOrderSqlDataService _orderData;
        private readonly IMachineSqlDataService _machineData;
        private readonly IRoutingSqlDataService _routingData;

        public ProductionPlanning(IMachineAvailabilitySqlDataService maData, IOrderSqlDataService orderData, IMachineSqlDataService machineData, IRoutingSqlDataService routingData)
        {
            _maData = maData;
            _orderData = orderData;
            _machineData = machineData;
            _routingData = routingData;
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
            DateTime endDate;
            string Note = "";

            double tempDuration = 0.0;
            OrderProcessModel orderProcess = new();

            DateTime startDate = order.EarliestStartDate;
            DateTime usingDate = startDate;

            List<IRoutingModel> steps = await _routingData.ReadRoutingForOneProduct(order.ProductId);
            List<IMachineModel> neededMachines = new();

            foreach (var step in steps)
            {
                string descriptionForAvailability;
                bool isAvailable;
                var machine = await _machineData.ReadMachine(step.MachineId);
                (isAvailable,descriptionForAvailability) = await CheckMachineAvailability(step.MachineId, usingDate);


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
                if (isAvailable)
                {
                    tempDuration += ((step.SetupTimeInSeconds + step.ProcessTimeInSeconds) / 3600.0);
                }
                else
                {
                    throw new Exception("Problem with Machine Availability");
                }

            }
            duration = tempDuration.ToString("F2");
            endDate = startDate.AddHours(tempDuration);

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
