using CsvHelper;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace SupportLibrary.Data
{
    public static class ReadCSV
    {
        public static async Task<List<OrderModel>> ReadOrdersFile(IBrowserFile file)
        {
            List<OrderModel> output = new();
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                var content = await streamReader.ReadToEndAsync();


                var csvData = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(line => line.Split(';').ToList())
                                 .ToList();
                csvData.RemoveAt(0);
                string format = "dd.MM.yyyy";
                foreach (var entry in csvData)
                {
                    output.Add(new OrderModel
                    {
                        Id = int.Parse(entry[0]),
                        ProductId = int.Parse(entry[1]),
                        Deadline = DateTime.ParseExact(entry[2],format,CultureInfo.InvariantCulture),
                        EarliestStartDate = DateTime.ParseExact(entry[3], format, CultureInfo.InvariantCulture),
                        OrderDate = DateTime.ParseExact(entry[4], format, CultureInfo.InvariantCulture)

                    });
                }
            }
            return output;
        }

        public static async Task<List<MachineModel>> ReadMachinesFile(IBrowserFile file)
        {
            CultureInfo germanyCulture = new CultureInfo("de-DE");
            List<MachineModel> output = new();
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                var content = await streamReader.ReadToEndAsync();


                var csvData = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(line => line.Split(';').ToList())
                                 .ToList();
                csvData.RemoveAt(0);

                foreach (var entry in csvData)
                {
                    entry[3]= entry[3].Replace(',', '.');
                    var isValidDouble = double.TryParse(entry[3], out double effectivity);
                    var isValidInt = int.TryParse(entry[4], out int MAG);
                    output.Add(new MachineModel
                    {
                        Id= int.Parse(entry[0]),
                        ShortName = entry[1],
                        Description = entry[2],
                        Effectivity = effectivity,
                        MachineAlternativityGroup = MAG
                    });
                }
            }

            return output;
        }

        public static async Task<List<RoutingModel>> ReadRoutingsFile(IBrowserFile file)
        {
            List<RoutingModel> output = new();
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                var content = await streamReader.ReadToEndAsync();


                var csvData = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(line => line.Split(';').ToList())
                                 .ToList();

                csvData.RemoveAt(0);
               
                foreach (var entry in csvData)
                {

                    output.Add(new RoutingModel
                    {
                        ProductId = int.Parse(entry[0]),
                        StepId = int.Parse(entry[1]),
                        MachineId = int.Parse(entry[2]),
                        SetupTimeInSeconds = int.Parse(entry[3]),
                        ProcessTimeInSeconds = int.Parse(entry[4])
                    });
                }
            }

            return output;
        }

        public static async Task<List<MachineAvailabilityModel>> ReadMachineAvailabilityFile(IBrowserFile file)
        {
            List<MachineAvailabilityModel> output = new();
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                var content = await streamReader.ReadToEndAsync();


                var csvData = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(line => line.Split(';').ToList())
                                 .ToList();

                csvData.RemoveAt(0);
                string format = "dd.MM.yyyy HH:mm";
                foreach (var entry in csvData)
                {
                    output.Add(new MachineAvailabilityModel
                    {
                        MachineId = int.Parse(entry[0]),
                        PauseStartDate = DateTime.ParseExact(entry[1],format,CultureInfo.InvariantCulture),
                        PauseEndDate = DateTime.ParseExact(entry[2],format, CultureInfo.InvariantCulture),
                        Description = entry[3]
                    });
                }
            }
            return output;
        }

    }
}



