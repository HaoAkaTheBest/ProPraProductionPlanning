using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.ModelBuilder;
using Google.OrTools.Sat;

namespace SupportLibrary.CreateProduction
{
    public class OptimizedProductionPlanning : IOptimizedProductionPlanning
    {
        private class AssignedTask : IComparable
        {
            public int jobID;
            public int taskID;
            public int start;
            public int duration;

            public AssignedTask(int jobID, int taskID, int start, int duration)
            {
                this.jobID = jobID;
                this.taskID = taskID;
                this.start = start;
                this.duration = duration;
            }

            public int CompareTo(object obj)
            {
                if (obj == null)
                    return 1;

                AssignedTask otherTask = obj as AssignedTask;
                if (otherTask != null)
                {
                    if (this.start != otherTask.start)
                        return this.start.CompareTo(otherTask.start);
                    else
                        return this.duration.CompareTo(otherTask.duration);
                }
                else
                    throw new ArgumentException("Object is not a Temperature");
            }
        }
        private readonly IMachineAvailabilitySqlDataService _maData;
        private readonly IOrderSqlDataService _orderData;
        private readonly IMachineSqlDataService _machineData;
        private readonly IRoutingSqlDataService _routingData;
        private readonly IMachineUsedSqlDataService _machineUsedData;

        public OptimizedProductionPlanning(IMachineAvailabilitySqlDataService maData, IOrderSqlDataService orderData, IMachineSqlDataService machineData, IRoutingSqlDataService routingData, IMachineUsedSqlDataService machineUsedData)
        {
            _maData = maData;
            _orderData = orderData;
            _machineData = machineData;
            _routingData = routingData;
            _machineUsedData = machineUsedData;
        }

        public async Task<List<ITaskData>> CreateOptimizedProductionPlan()
        {
            await CreateProductionPlan();

            List<IOrderModel> orders = await _orderData.ReadOrder();
            List<IMachineUsedOptimizedModel> machineUsedOptimzeds = await _machineUsedData.ReadMachineUsedOptimized();

            List<int> sortedOrdersId = machineUsedOptimzeds.Select(x => x.OrderId).Distinct().ToList();

            List<ITaskData> output = new();

            foreach (var orderId in sortedOrdersId)
            {
                var order = orders.Where(x => x.Id == orderId).FirstOrDefault();
                List<IMachineUsedOptimizedModel> machineForThisOrder = machineUsedOptimzeds.Where(x => x.OrderId == orderId).ToList();
                var firstMachine = machineForThisOrder.FirstOrDefault();
                var lastMachine = machineForThisOrder.LastOrDefault();

                var duration = (lastMachine.Start - firstMachine.Start + lastMachine.Duration) * 60;

                DateTime startDate = order.EarliestStartDate.AddHours(firstMachine.Start);
                DateTime endDate = startDate.AddMinutes(duration);

                int progress =(int) await CalculateProgress(endDate, startDate);

                string note="";
                if (order.Deadline <= endDate)
                {
                    note = $"{endDate.Subtract(order.Deadline).Days} days after Deadline; ";
                }

                output.Add(new TaskData
                {
                    TaskId = order.Id,
                    TaskName = $"{order.ProductId}",
                    OrderDate = order.OrderDate,
                    Deadline = order.Deadline,
                    StartDate = startDate,
                    Duration = $"{duration}",
                    Progress = progress,
                    EndDate = endDate,
                    Note = note,
                    Status = "Optimized",
                    DurationUnit = "minute"
                });
            }

            //List<ITaskData> output = new();

            //foreach (var order in orders)
            //{
            //    IOrderProcessModel process = await CalculateDuration(order);
            //    output.Add(new TaskData
            //    {
            //        TaskId = order.Id,
            //        TaskName = $"{order.ProductId}",
            //        OrderDate = order.OrderDate,
            //        Deadline = order.Deadline,
            //        StartDate = process.StartDate,
            //        Duration = process.Duration,
            //        Progress = process.Progress,
            //        EndDate = process.EndDate,
            //        Note = process.Note,
            //        Status = process.Status,
            //        DurationUnit = "minute"
            //    });
            //}

            return output;
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


        public async Task<string> CreateProductionPlan()
        {
            var allOrders = await _orderData.ReadOrder();

            var allJobs = new List<JobModel>();
            foreach (var order in allOrders)
            {
                allJobs.Add(new JobModel { OrderId = order.Id, Tasks = await CalculateAufgaben(order.ProductId) });
            }

            int numMachines = 9;

            //       var allJobs =
            //new[] {
            //   new[] {
            //       // job0
            //       new { machine = 0, duration = 3 }, // task0
            //       new { machine = 1, duration = 2 }, // task1
            //       new { machine = 2, duration = 2 }, // task2
            //   }
            //       .ToList(),
            //   new[] {
            //       // job1
            //       new { machine = 0, duration = 2 }, // task0
            //       new { machine = 2, duration = 1 }, // task1
            //       new { machine = 1, duration = 4 }, // task2
            //   }
            //       .ToList(),
            //   new[] {
            //       // job2
            //       new { machine = 1, duration = 4 }, // task0
            //       new { machine = 2, duration = 3 }, // task1
            //   }
            //       .ToList(),
            //}
            //    .ToList();

            //int numMachines = 0;
            //foreach (var job in allJobs)
            //{
            //    foreach (var task in job.Tasks)
            //    {
            //        numMachines = Math.Max(numMachines, task.MachineId);
            //    }
            //}
            int[] allMachines = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Computes horizon dynamically as the sum of all durations.
            long horizon = 0;
            foreach (var job in allJobs)
            {
                foreach (var task in job.Tasks)
                {
                    horizon += task.Duration;
                }
            }

            CpModel model = new CpModel();

            Dictionary<Tuple<int, int>, Tuple<IntVar, IntVar, IntervalVar>> allTasks =
    new Dictionary<Tuple<int, int>, Tuple<IntVar, IntVar, IntervalVar>>(); // (start, end, duration)
            Dictionary<int, List<IntervalVar>> machineToIntervals = new Dictionary<int, List<IntervalVar>>();
            //for (int jobID = 0; jobID < allJobs.Count(); ++jobID)
            foreach (var job in allJobs)
            {
                int taskID = 1;
                //var job = allJobs[jobID];
                //for (int taskID = 0; taskID < job.Count(); ++taskID)
                foreach (var task in job.Tasks)
                {
                    //var task = job[taskID];
                    String suffix = $"_{job.OrderId}_{taskID}";
                    IntVar start = model.NewIntVar(0, horizon, "start" + suffix);
                    IntVar end = model.NewIntVar(0, horizon, "end" + suffix);
                    IntervalVar interval = model.NewIntervalVar(start, task.Duration, end, "interval" + suffix);
                    var key = Tuple.Create(job.OrderId, taskID);
                    allTasks[key] = Tuple.Create(start, end, interval);
                    if (!machineToIntervals.ContainsKey(task.MachineId))
                    {
                        machineToIntervals.Add(task.MachineId, new List<IntervalVar>());
                    }
                    machineToIntervals[task.MachineId].Add(interval);
                    taskID++;
                }
                
            }

            // Create and add disjunctive constraints.
            foreach (int machine in allMachines)
            {
                model.AddNoOverlap(machineToIntervals[machine]);
            }

            // Precedences inside a job.
            //for (int jobID = 0; jobID < allJobs.Count(); ++jobID)
            foreach (var job in allJobs)
            {


                //var job = allJobs[jobID];
                for (int taskID = 1; taskID < job.Tasks.Count() ; ++taskID)
                {
                    var key = Tuple.Create(job.OrderId, taskID);
                    var nextKey = Tuple.Create(job.OrderId, taskID + 1);
                    model.Add(allTasks[nextKey].Item1 >= allTasks[key].Item2);
                }
                
            }

            // Makespan objective.
            IntVar objVar = model.NewIntVar(0, horizon, "makespan");

            List<IntVar> ends = new List<IntVar>();
            //for (int jobID = 0; jobID < allJobs.Count(); ++jobID)
            foreach (var job in allJobs)
            {
                //var job = allJobs[jobID];
                var key = Tuple.Create(job.OrderId, job.Tasks.Count() - 1);
                ends.Add(allTasks[key].Item2);
            }
            model.AddMaxEquality(objVar, ends);
            model.Minimize(objVar);


            //Solve
            CpSolver solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);

            string superOutput = $"Solve status: {status} \n";

            //Console.WriteLine($"Solve status: {status}");

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                superOutput += "Solution: \n";
                //Console.WriteLine("Solution:");

                Dictionary<int, List<AssignedTask>> assignedJobs = new Dictionary<int, List<AssignedTask>>();
                //for (int jobID = 0; jobID < allJobs.Count(); ++jobID)
                foreach (var job in allJobs)
                {
                    int taskID = 1;
                    //var job = allJobs[jobID];
                    //for (int taskID = 0; taskID < job.Count(); ++taskID)
                    foreach (var task in job.Tasks)
                    {
                        //var task = job[taskID];
                        var key = Tuple.Create(job.OrderId, taskID);
                        int start = (int)solver.Value(allTasks[key].Item1);
                        if (!assignedJobs.ContainsKey(task.MachineId))
                        {
                            assignedJobs.Add(task.MachineId, new List<AssignedTask>());
                        }
                        assignedJobs[task.MachineId].Add(new AssignedTask(job.OrderId, taskID, start, task.Duration));
                        taskID++;
                    }
                }

                foreach (var machine in allMachines)
                {
                    assignedJobs[machine].Sort();
                    foreach (var item in assignedJobs[machine])
                    {
                        await _machineUsedData.CreateMachineUsedOptimized(item.jobID, machine, item.start, item.duration, item.taskID);
                    }
                }

                // Create per machine output lines.
                String output = "";
                foreach (int machine in allMachines)
                {
                    // Sort by starting time.
                    assignedJobs[machine].Sort();
                    String solLineTasks = $"Machine {machine}: ";
                    String solLine = "           ";

                    foreach (var assignedTask in assignedJobs[machine])
                    {
                        String name = $"job_{assignedTask.jobID}_task_{assignedTask.taskID}";
                        // Add spaces to output to align columns.
                        solLineTasks += $"{name,-15}";

                        String solTmp = $"[{assignedTask.start},{assignedTask.start + assignedTask.duration}]";
                        // Add spaces to output to align columns.
                        solLine += $"{solTmp,-15}";
                    }
                    output += solLineTasks + "\n";
                    output += solLine + "\n";
                }
                // Finally print the solution found.
                superOutput += $"Optimal Schedule Length: {solver.ObjectiveValue} \n";
                //Console.WriteLine($"Optimal Schedule Length: {solver.ObjectiveValue}");

                superOutput += $"\n{output}\n";
                //Console.WriteLine($"\n{output}");
            }
            else
            {
                superOutput += "No solution found.\n";
                //Console.WriteLine("No solution found.");
            }

            return superOutput;
        }

        private async Task<List<AufgabeModel>> CalculateAufgaben(int productId)
        {
            var steps = await _routingData.ReadRoutingForOneProduct(productId);
            var output = new List<AufgabeModel>();
            foreach (var step in steps)
            {
                var machine = await _machineData.ReadMachine(step.MachineId);
                double effectivity = 1;
                if (machine.Effectivity != 0)
                {
                    effectivity = machine.Effectivity/100;
                }
                var dur = (step.ProcessTimeInSeconds + step.SetupTimeInSeconds) / effectivity / 3600;

                output.Add(new AufgabeModel { MachineId = step.MachineId, Duration = (int) Math.Ceiling(dur)});
            }

            return output;
        }
    }
}
