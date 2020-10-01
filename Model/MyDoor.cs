using GTA;

namespace GtaVehiclePersistence.Model
{
    public class MyDoor
    {
        public VehicleDoorIndex Index { get; set; }
        public bool IsBroken { get; set; }
        
        /// <summary>
        /// Zero to one scale of door state where zero is fully closed, and one is fully open.
        /// </summary>
        public float AngleRatio { get; set; }
    }
}
