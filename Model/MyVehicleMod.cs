using GTA;

namespace GtaVehiclePersistence.Model
{
    public class MyVehicleMod
    {
        public VehicleModType Type { get; set; }
        
        /// <summary>
        /// Friendly name of the vehicle mod. Helps to make vehicle save files more readable.
        /// </summary>
        public string FriendlyName { get; set; }
        public int Index { get; set; }
    }
}
