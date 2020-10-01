using GTA;
using GtaVehiclePersistence.Infrastructure;
using CarPersistence.Infrastructure;

namespace GtaVehiclePersistence
{
    public class Bootstrap : Script
    {
        private readonly VehiclePersistenceController _controller;

        public Bootstrap()
        {
            _controller = new VehiclePersistenceController(new VehicleRepository(this.BaseDirectory));
            this.KeyDown += _controller.OnKeyDown;
        }
    }
}