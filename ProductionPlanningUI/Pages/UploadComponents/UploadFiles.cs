using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductionPlanningUI.Models;
using SupportLibrary.Data;
using SupportLibrary.Models;
using System;

namespace ProductionPlanningUI.Pages.UploadComponents
{
    public partial class UploadFiles
    {
        [Inject]
        public ILogger<UploadFiles> Logger { get; set; }
        [Inject]
        public IConfiguration config { get; set; }
        [Inject]
        public IMachineSqlDataService machineDataAccess { get; set; }
        [Inject]
        public IOrderSqlDataService orderDataAccess { get; set; }
        [Inject]
        public IRoutingSqlDataService routingDataAccess { get; set; }
        [Inject]
        public IMachineAvailabilitySqlDataService machineAvailabilityDataAccess { get; set; }

        private IBrowserFile machinesFile;
        private IBrowserFile routingsFile;
        private IBrowserFile ordersFile;
        private IBrowserFile machineAvailabilityFile;

        List<OrderModel> orders = new();
        List<MachineModel> machines = new();
        List<RoutingModel> routings = new();
        List<MachineAvailabilityModel> machineAvailabilites = new();

        List<MachineModel> duplicatedMachines = new();
        List<OrderModel> duplicatedOrders = new();
        List<RoutingModel> duplicatedRoutings = new();
        List<MachineAvailabilityModel> duplicatedMA = new();

        private bool showMachinesFile;
        private bool showRoutingsFile;
        private bool showOrdersFile;
        private bool showMachineAvailabilityFile;

        private long maxFileSize = 1;

        private bool uploading;
        private bool uploadSuccess;
        private bool uploadFail;

        private bool duplicatedEntries;
        private bool isDupMachines;
        private bool isDupOrders;
        private bool isDupRoutings;
        private bool isDupMA;

        private List<string> errors = new();
        private UploadFilesModel uploadFiles = new();

        private async Task ShowOrders()
        {
            errors.Clear();
            if (ordersFile == null)
            {
                errors.Add("There are no uploaded Orders!!");
                return;
            }
            uploadSuccess = false;
            if (showMachinesFile)
            {
                showMachinesFile = false;
            }
            if (showRoutingsFile)
            {
                showRoutingsFile = false;
            }
            if (showMachineAvailabilityFile)
            {
                showMachineAvailabilityFile = false;
            }
            showOrdersFile = true;
        }

        private async Task ShowMachines()
        {
            errors.Clear();
            if (machinesFile == null)
            {
                errors.Add("There are no uploaded Machines!!");
                return;
            }
            uploadSuccess = false;
            if (showOrdersFile)
            {
                showOrdersFile = false;
            }
            if (showRoutingsFile)
            {
                showRoutingsFile = false;
            }
            if (showMachineAvailabilityFile)
            {
                showMachineAvailabilityFile = false;
            }
            showMachinesFile = true;
        }

        private async Task ShowRoutings()
        {
            errors.Clear();
            if (routingsFile == null)
            {
                errors.Add("There are no uploaded Routings!!");
                return;
            }
            uploadSuccess = false;
            if (showOrdersFile)
            {
                showOrdersFile = false;
            }
            if (showMachinesFile)
            {
                showMachinesFile = false;
            }
            if (showMachineAvailabilityFile)
            {
                showMachineAvailabilityFile = false;
            }
            showRoutingsFile = true;
        }
        private async Task ShowMA()
        {
            errors.Clear();
            if (machineAvailabilityFile == null)
            {
                errors.Add("There are no uploaded Machine Availability!!");
                return;
            }
            uploadSuccess = false;
            if (showOrdersFile)
            {
                showOrdersFile = false;
            }
            if (showRoutingsFile)
            {
                showRoutingsFile = false;
            }
            if (showMachinesFile)
            {
                showMachinesFile = false;
            }
            showMachineAvailabilityFile = true;
        }

        private async Task LoadOrdersFile(InputFileChangeEventArgs e)
        {
            await Task.Run(() => { ordersFile = e.File; });
        }
        private async Task LoadMachinesFile(InputFileChangeEventArgs e)
        {
            await Task.Run(() => { machinesFile = e.File; });
        }
        private async Task LoadRoutingsFile(InputFileChangeEventArgs e)
        {
            await Task.Run(() => { routingsFile = e.File; });
        }
        private async Task LoadMachineAvailabilityFile(InputFileChangeEventArgs e)
        {
            await Task.Run(() => { machineAvailabilityFile = e.File; });
        }

        private async Task SubmitForm()
        {
            uploadSuccess = false;
            try
            {
                uploading = true;
                await CaptureFile();
                uploadSuccess = true;
                uploading = false;
            }
            catch (Exception ex)
            {
                errors.Add($"Error: {ex.Message}");
                uploadFail = true;
                throw;
            }
            finally
            {
                
                uploading = false;
            }
        }

        private async Task ResetForm()
        {
            uploading = false;
            uploadFail = false;
            uploadSuccess = false;

            uploadFiles = new();
            routingsFile = null;
            ordersFile = null;
            machinesFile = null;
            machineAvailabilityFile = null;

            showMachinesFile = false;
            showRoutingsFile = false;
            showOrdersFile = false;
            showMachineAvailabilityFile = false;

            duplicatedEntries = false;
            isDupMA = false;
            isDupMachines = false;
            isDupOrders = false;
            isDupRoutings = false;

            routings.Clear();
            machineAvailabilites.Clear();
            errors.Clear();
            orders.Clear();
            machines.Clear();
            duplicatedMachines.Clear();
            duplicatedOrders.Clear();
            duplicatedMA.Clear();
            duplicatedRoutings.Clear();
        }

        private async Task CaptureFile()
        {
            if (machinesFile != null)
            {
                if (machinesFile.Size > (maxFileSize * 1024 * 1024))
                {
                    return;
                }
                try
                {
                    // save file to machine

                    //string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(machinesFile.Name));

                    //var path = Path.Combine(config.GetValue<string>("FileStorage"), "testUser",
                    //        newFileName);

                    //Directory.CreateDirectory(Path.Combine(config.GetValue<string>("FileStorage"), "testUser"));

                    //await using FileStream fs = new(path, FileMode.Create);
                    //await machinesFile.OpenReadStream(maxFileSize * 1024 * 1024).CopyToAsync(fs);

                    machines = await ReadCSV.ReadMachinesFile(machinesFile);
                    int numberOfDuplicateEntries = 0;
                    foreach (var m in machines)
                    {
                        try
                        {
                            await machineDataAccess.CreateMachine(m);
                        }
                        catch (Exception ex)
                        {
                            numberOfDuplicateEntries += 1;
                            duplicatedMachines.Add(m);
                        }

                    }
                    if (numberOfDuplicateEntries == machines.Count())
                    {
                        duplicatedMachines.Clear();
                        throw new Exception("All the values are duplicated");

                    }
                    else if (numberOfDuplicateEntries >= 1 && numberOfDuplicateEntries < machines.Count())
                    {
                        duplicatedEntries = true;
                        isDupMachines = true;
                        throw new Exception($"Duplicated entries");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"File: {machinesFile.Name} Error: {ex.Message}");
                    Logger.LogError("File: {Filename} Error: {Error}",
                        machinesFile.Name, ex.Message);
                }
            }
            if (ordersFile != null)
            {
                if (ordersFile.Size > (maxFileSize * 1024 * 1024))
                {
                    return;
                }
                try
                {
                    // save file to machine

                    //string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(ordersFile.Name));

                    //var path = Path.Combine(config.GetValue<string>("FileStorage"), "testUser",
                    //        newFileName);

                    //Directory.CreateDirectory(Path.Combine(config.GetValue<string>("FileStorage"), "testUser"));

                    //await using FileStream fs = new(path, FileMode.Create);
                    //await ordersFile.OpenReadStream(maxFileSize * 1024 * 1024).CopyToAsync(fs);

                    orders = await ReadCSV.ReadOrdersFile(ordersFile);
                    int numberOfDuplicateEntries = 0;
                    foreach (var o in orders)
                    {
                        try
                        {
                            await orderDataAccess.CreateOrder(o);
                        }
                        catch (Exception ex)
                        {
                            numberOfDuplicateEntries += 1;
                            duplicatedOrders.Add(o);
                        }
                    }
                    if (numberOfDuplicateEntries == orders.Count())
                    {
                        duplicatedOrders.Clear();
                        throw new Exception("All the values are duplicated");
                    }
                    else if (numberOfDuplicateEntries >= 1 && numberOfDuplicateEntries < orders.Count())
                    {
                        duplicatedEntries = true;
                        isDupOrders = true;
                        throw new Exception($"Duplicated entries");
                    }

                }
                catch (Exception ex)
                {
                    errors.Add($"File: {ordersFile.Name} Error: {ex.Message}");
                    Logger.LogError("File: {Filename} Error: {Error}",
                        ordersFile.Name, ex.Message);
                }
            }

            if (routingsFile != null)
            {
                if (routingsFile.Size > (maxFileSize * 1024 * 1024))
                {
                    return;
                }

                try
                {
                    // save file to machine

                    //string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(routingsFile.Name));

                    //var path = Path.Combine(config.GetValue<string>("FileStorage"), "testUser",
                    //        newFileName);

                    //Directory.CreateDirectory(Path.Combine(config.GetValue<string>("FileStorage"), "testUser"));

                    //await using FileStream fs = new(path, FileMode.Create);
                    //await routingsFile.OpenReadStream(maxFileSize * 1024 * 1024).CopyToAsync(fs);

                    routings = await ReadCSV.ReadRoutingsFile(routingsFile);
                    int numberOfDuplicateEntries = 0;
                    foreach (var r in routings)
                    {
                        try
                        {
                            await routingDataAccess.CreateRouting(r);
                        }
                        catch (Exception ex)
                        {
                            numberOfDuplicateEntries += 1;
                            duplicatedRoutings.Add(r);
                        }
                    }
                    if (numberOfDuplicateEntries == routings.Count())
                    {
                        duplicatedRoutings.Clear();
                        throw new Exception("All the values are duplicated");

                    }
                    else if (numberOfDuplicateEntries >= 1 && numberOfDuplicateEntries < routings.Count())
                    {
                        duplicatedEntries = true;
                        isDupRoutings = true;
                        throw new Exception($"Duplicated entries");
                    }

                }
                catch (Exception ex)
                {
                    errors.Add($"File: {routingsFile.Name} Error: {ex.Message}");
                    Logger.LogError("File: {Filename} Error: {Error}",
                        routingsFile.Name, ex.Message);
                }
            }

            if (machineAvailabilityFile != null)
            {
                if (machineAvailabilityFile.Size > (maxFileSize * 1024 * 1024))
                {
                    return;
                }

                try
                {
                    //save file to machine

                    //string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(machineAvailabilityFile.Name));

                    //var path = Path.Combine(config.GetValue<string>("FileStorage"), "testUser",
                    //        newFileName);

                    //Directory.CreateDirectory(Path.Combine(config.GetValue<string>("FileStorage"), "testUser"));

                    //await using FileStream fs = new(path, FileMode.Create);
                    //await machineAvailabilityFile.OpenReadStream(maxFileSize * 1024 * 1024).CopyToAsync(fs);

                    machineAvailabilites = await ReadCSV.ReadMachineAvailabilityFile(machineAvailabilityFile);
                    int numberOfDuplicateEntries = 0;
                    foreach (var availability in machineAvailabilites)
                    {
                        try
                        {
                            await machineAvailabilityDataAccess.CreateMachineAvailability(availability);
                        }
                        catch (Exception ex)
                        {
                            numberOfDuplicateEntries += 1;
                            duplicatedMA.Add(availability);
                        }
                    }
                    if (numberOfDuplicateEntries == machineAvailabilites.Count())
                    {
                        duplicatedMA.Clear();
                        throw new Exception("All the values are duplicated");
                    }
                    else if (numberOfDuplicateEntries >= 1 && numberOfDuplicateEntries < machineAvailabilites.Count())
                    {
                        duplicatedEntries = true;
                        isDupMA = true;
                        throw new Exception($"Duplicated entries");
                    }

                }
                catch (Exception ex)
                {
                    errors.Add($"File: {machineAvailabilityFile.Name} Error: {ex.Message}");
                    Logger.LogError("File: {Filename} Error: {Error}",
                        machineAvailabilityFile.Name, ex.Message);

                }
            }
        }
    }
}
