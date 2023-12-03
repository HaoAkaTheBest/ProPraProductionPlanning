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

        private bool showMachinesFile;
        private bool showRoutingsFile;
        private bool showOrdersFile;
        private bool showMachineAvailabilityFile;

        private long maxFileSize = 1;

        private bool uploading;
        private bool uploadSuccess;

        private List<string> errors = new();
        private UploadFilesModel uploadFiles = new();

        private async Task ShowOrders()
        {
            if (ordersFile == null)
            {
                errors.Add("There are no uploaded orders!!");
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
            if (machinesFile ==null)
            {
                errors.Add("There are no uploaded machines!!");
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
            machines = await ReadCSV.ReadMachinesFile(machinesFile);
            showMachinesFile = true;
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
            try
            {
                uploading = true;
                await CaptureFile();
                uploadSuccess = true;
                uploading = false;
                errors.Clear();
            }
            catch (Exception ex)
            {
                errors.Add($"Error: {ex.Message}");
                throw;
            }
        }

        private async Task ResetForm()
        {
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

            errors.Clear();
            orders.Clear();
            machines.Clear();
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
                    string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(machinesFile.Name));

                    var path = Path.Combine(config.GetValue<string>("FileStorage"), "testUser",
                            newFileName);

                    Directory.CreateDirectory(Path.Combine(config.GetValue<string>("FileStorage"), "testUser"));

                    await using FileStream fs = new(path, FileMode.Create);
                    await machinesFile.OpenReadStream(maxFileSize * 1024 * 1024).CopyToAsync(fs);

                    var tempMachines = await ReadCSV.ReadMachinesFile(machinesFile);
                    foreach (var m in tempMachines)
                    {
                        machineDataAccess.CreateMachine(m);
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
                    string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(ordersFile.Name));

                    var path = Path.Combine(config.GetValue<string>("FileStorage"), "testUser",
                            newFileName);

                    Directory.CreateDirectory(Path.Combine(config.GetValue<string>("FileStorage"), "testUser"));

                    await using FileStream fs = new(path, FileMode.Create);
                    await ordersFile.OpenReadStream(maxFileSize * 1024 * 1024).CopyToAsync(fs);

                    orders = await ReadCSV.ReadOrdersFile(ordersFile);
                    foreach (var o in orders)
                    {
                        orderDataAccess.CreateOrder(o);
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
                    string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(routingsFile.Name));

                    var path = Path.Combine(config.GetValue<string>("FileStorage"), "testUser",
                            newFileName);

                    Directory.CreateDirectory(Path.Combine(config.GetValue<string>("FileStorage"), "testUser"));

                    await using FileStream fs = new(path, FileMode.Create);
                    await routingsFile.OpenReadStream(maxFileSize * 1024 * 1024).CopyToAsync(fs);

                    var routings = await ReadCSV.ReadRoutingsFile(routingsFile);
                    foreach (var r in routings)
                    {
                        routingDataAccess.CreateRouting(r);
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
                    string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(machineAvailabilityFile.Name));

                    var path = Path.Combine(config.GetValue<string>("FileStorage"), "testUser",
                            newFileName);

                    Directory.CreateDirectory(Path.Combine(config.GetValue<string>("FileStorage"), "testUser"));

                    await using FileStream fs = new(path, FileMode.Create);
                    await machineAvailabilityFile.OpenReadStream(maxFileSize * 1024 * 1024).CopyToAsync(fs);

                    var mas = await ReadCSV.ReadMachineAvailabilityFile(machineAvailabilityFile);
                    foreach (var availability in mas)
                    {
                        machineAvailabilityDataAccess.CreateMachineAvailability(availability);
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
