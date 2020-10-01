using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using GtaVehiclePersistence.Model;
using System.Linq;

namespace GtaVehiclePersistence.Infrastructure
{
    /// <summary>
    /// Responsible for managing vehicles. Yeah, it's not a pure 'repository' because it does IO too. Sue me.
    /// </summary>
    class VehicleRepository
    {
        public IEnumerable<MyVehicle> MyVehicles => _vehicles;
        private readonly List<MyVehicle> _vehicles = new List<MyVehicle>();
        private readonly string _storageFolderPath;

        private const string StorageFolderName = "SavedVehicles";

        public VehicleRepository(string baseDirectory)
        {
            _storageFolderPath = Path.Combine(baseDirectory, StorageFolderName);

            if (!Directory.Exists(_storageFolderPath))
            {
                Directory.CreateDirectory(_storageFolderPath);
            }
        }

        public bool AddVehicle(MyVehicle vehicle, bool saveToDisk = false)
        {
            if (_vehicles.Contains(vehicle) || _vehicles.Any(v => vehicle.SameAs(v)))
                return false;

            _vehicles.Add(vehicle);
            
            if (saveToDisk)
                SaveVehicle(vehicle);

            return true;
        }

        public bool RemoveVehicle(MyVehicle vehicle, bool deleteFromDisk = false)
        {
            if (!_vehicles.Contains(vehicle))
                return false;

            _vehicles.Remove(vehicle);

            if (deleteFromDisk)
                DeleteVehicle(vehicle);

            return true;
        }

        public bool RemoveVehicle(string licensePlate, bool deleteFromDisk = false)
        {
            var vehicle = _vehicles.FirstOrDefault(v => v.LicensePlate == licensePlate);

            if (vehicle == null)
                return false;

            _vehicles.Remove(vehicle);

            if (deleteFromDisk)
                DeleteVehicle(vehicle);

            return true;
        }

        public bool UpdateVehicle(MyVehicle vehicle, bool saveToDisk = false)
        {
            var vehicleIndex = _vehicles.FindIndex(v => vehicle.SameAs(v));

            if (vehicleIndex == -1)
                return false;

            _vehicles[vehicleIndex] = vehicle; // If object instance is the same, there is obviously nothing to update, but does no harm either.

            if (saveToDisk)
                SaveVehicle(vehicle);

            return true;
        }

        public bool AddOrUpdateVehicle(MyVehicle vehicle, bool saveToDisk = false)
        {
            return UpdateVehicle(vehicle, saveToDisk) || AddVehicle(vehicle, saveToDisk); // Short-circuits if first is true.
        }

        public void Clear()
        {
            _vehicles.Clear();
        }

        public void LoadVehicles()
        {
            if (_vehicles.Any())
            {
                throw new InvalidOperationException($"Vehicle list already contains vehicles. Replacing it might cause data loss. Call '{nameof(Clear)}()' before attempting to load vehicles (and save vehicles before clearing, if necessary).");
            }

            var fileList = Directory.GetFiles(_storageFolderPath, "*.json");

            foreach (var filePath in fileList)
            {
                try
                {
                    _vehicles.Add(
                        JsonConvert.DeserializeObject<MyVehicle>(
                            File.ReadAllText(filePath)
                    ));
                }
                catch (Exception e)
                {
                    throw new Exception($"Couldn't load vehicle from file '{filePath}'. Fix or remove this file, and try again. Error message: '{e.Message}'", e);
                }
            }
        }

        public void SaveVehicles()
        {
            foreach (var vehicle in _vehicles)
            {
                SaveVehicle(vehicle);
            }
        }

        private void SaveVehicle(MyVehicle vehicle)
        {
            var serializedVehicle = JsonConvert.SerializeObject(vehicle, Formatting.Indented);

            try
            {
                File.WriteAllText(Path.Combine(_storageFolderPath, GetVehicleFileName(vehicle)), serializedVehicle, Encoding.UTF8);
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't save vehicle file '{GetVehicleFileName(vehicle)}' to folder '{_storageFolderPath}'. Error message: '{e.Message}'", e);
            }
        }

        private void DeleteVehicle(MyVehicle vehicle)
        {
            var vehicleFilePath = Path.Combine(_storageFolderPath, GetVehicleFileName(vehicle));

            if (!File.Exists(vehicleFilePath))
                return;

            try
            {
                File.Delete(vehicleFilePath);
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't delete vehicle file '{GetVehicleFileName(vehicle)}' from folder '{_storageFolderPath}. Error message: '{e.Message}'", e);
            }
        }

        private string GetVehicleFileName(MyVehicle vehicle)
        {
            return $"{vehicle.ModelName}-{vehicle.LicensePlate}.json";
        }
    }
}