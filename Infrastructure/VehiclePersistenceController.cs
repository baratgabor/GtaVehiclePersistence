using GtaVehiclePersistence.Model;
using GtaVehiclePersistence.Infrastructure;
using GTA;
using System.Linq;
using System.Windows.Forms;

namespace CarPersistence.Infrastructure
{
    class VehiclePersistenceController
    {
        private readonly VehicleRepository _vehicleRepository;
        private int _selectedVehicle;
        private const string Instructions = "\r\nAdd/Update current vehicle - NumPad*\nDelete current vehicle - Ctrl+Shift+NumPad*\nSelect previous/next vehicle - NumPad- / NumPad+\nSpawn selected vehicle - NumPad/\nDespawn selected vehicle - Ctrl+Shift+NumPad/";

        private int VehicleCount => _vehicleRepository.MyVehicles.Count();

        public VehiclePersistenceController(VehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;

            LoadVehicles();
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Game.IsLoading || Game.IsPaused)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Add:
                    IncrementSelectedVehicle();
                    break;
                case Keys.Subtract:
                    DecrementSelectedVehicle();
                    break;
                case Keys.Multiply:
                    if (e.Control && e.Shift)
                    {
                        DeleteVehicle(Game.Player.Character.CurrentVehicle.Mods.LicensePlate);
                    }
                    else
                    {
                        AddOrUpdateVehicle(Game.Player.Character.CurrentVehicle);
                    }
                    break;
                case Keys.Divide:
                    if (e.Control && e.Shift)
                    {
                        if (e.Alt)
                        {
                            DespawnAllKnownVehicles();
                        }
                        else
                        {
                            DespawnSelectedVehicle();
                        }
                    }
                    else
                    {
                        SpawnSelectedVehicle();
                    }
                    break;
                default:
                    break;
            }
        }

        private void IncrementSelectedVehicle()
        {
            if (VehicleCount == 0)
            {
                Stdout("There are no vehicles to select from.");
                return;
            }

            _selectedVehicle = (_selectedVehicle + 1) % VehicleCount;
            DisplaySelectedVehicle();
        }

        private void DecrementSelectedVehicle()
        {
            if (VehicleCount == 0)
            {
                Stdout("There are no vehicles to select from.");
                return;
            }

            _selectedVehicle = (_selectedVehicle - 1 + VehicleCount) % VehicleCount;
            DisplaySelectedVehicle();
        }

        private void DisplaySelectedVehicle()
        {
            var vehicle = _vehicleRepository.MyVehicles.ElementAt(_selectedVehicle);
            Stdout($"Selected Vehicle {_selectedVehicle + 1}/{VehicleCount}: {vehicle.ModelName}, Plate: {vehicle.LicensePlate}, Color: {vehicle.PrimaryColor}");
        }

        private void LoadVehicles()
        {
            _vehicleRepository.LoadVehicles();
            Stdout($"Loaded {_vehicleRepository.MyVehicles.Count()} vehicles. {Instructions}", 20000);
        }

        private void DeleteVehicle(string licensePlate)
        {
            var deleted = _vehicleRepository.RemoveVehicle(licensePlate, deleteFromDisk: true);

            if (deleted)
            {
                Stdout($"Successfully removed vehicle with license plate '{licensePlate}'.");
                _selectedVehicle--; // TODO: Perhaps add logic to ascertain if selected vehicle index has to be decreased due to vehicle removal.
            }
            else
            {
                Stdout($"Vehicle with license plate '{licensePlate}' couldn't be found for removal.");
            }
        }

        private void AddOrUpdateVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
            {
                Stdout("There is no current vehicle to add or update.");
                return;
            }

            var myVehicle = new MyVehicle(vehicle);

            if (_vehicleRepository.UpdateVehicle(myVehicle, saveToDisk: true))
            {
                Stdout($"Successfully updated vehicle '{myVehicle.ModelName}' with license plate '{myVehicle.LicensePlate}'.");
            }
            else if (_vehicleRepository.AddVehicle(myVehicle, saveToDisk: true))
            {
                Stdout($"Successfully added vehicle '{myVehicle.ModelName}' with license plate '{myVehicle.LicensePlate}'.");
            }
            else
            {
                Stdout("Couldn't add or update vehicle.");
            }
        }
        
        private void SpawnSelectedVehicle()
        {
            if (VehicleCount == 0)
            {
                Stdout("No vehicles have been loaded.");
                return;
            }

            if (Game.Player.Character.CurrentVehicle != null)
            {
                Stdout("Cannot spawn vehicle when you're in another vehicle.");
                return;
            }

            _vehicleRepository.MyVehicles.ElementAt(_selectedVehicle).Spawn(Game.Player.Character.FrontPosition + Game.Player.Character.ForwardVector * 5);
            Stdout("Selected vehicle spawned.");
        }

        private void DespawnSelectedVehicle()
        {
            var selectedVehicle = _vehicleRepository.MyVehicles.ElementAt(_selectedVehicle);

            var matchingNearVehicles = World.GetNearbyVehicles(Game.Player.Character.Position, 100).Where(v => v.Mods.LicensePlate == selectedVehicle.LicensePlate);
            var matchingNearVehicleCount = matchingNearVehicles.Count();

            foreach (var v in matchingNearVehicles)
            {
                v.Delete();
            }

            Stdout($"Despawned {matchingNearVehicleCount} instances of the selected vehicle from nearby.");
        }

        private void DespawnAllKnownVehicles()
        {
            var allNearVehicles = World.GetNearbyVehicles(Game.Player.Character.Position, 100);
            var despawnedVehicleCount = 0;

            foreach (var v in allNearVehicles)
            {
                if (_vehicleRepository.MyVehicles.Any(myV => myV.LicensePlate == v.Mods.LicensePlate))
                {
                    v.Delete();
                    despawnedVehicleCount++;
                }
            }

            Stdout($"Despawned {despawnedVehicleCount} known vehicles from nearby.");
        }

        private void Stdout(string message, int milliseconds = 5000)
        {
            GTA.UI.Screen.ShowSubtitle(message, milliseconds);
        }
    }
}
