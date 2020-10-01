using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GtaVehiclePersistence.Model
{
    class MyVehicle
    {
        public string ModelName { get; set; }
        public int ModelHash { get; set; }
        public MyVector3 Position { get; set; }
        public float Heading { get; set; }
        public string LicensePlate { get; set; }
        public LicensePlateStyle LicensePlateStyle { get; set; }
        public float DirtLevel { get; set; }
        public float BodyHealth { get; set; }
        public float EngineHealth { get; set; }
        public float PetrolTankHealth { get; set; }
        public bool CanTiresBurst { get; set; }
        public VehicleRoofState RoofState { get; set; }
        public VehicleWheelType WheelType { get; set; }
        public VehicleColor PrimaryColor { get; set; }
        public VehicleColor SecondaryColor { get; set; }
        public VehicleColor PearlescentColor { get; set; }
        public VehicleColor RimColor { get; set; }
        public VehicleColor TrimColor { get; set; }
        public VehicleWindowTint WindowTint { get; set; }
        public List<MyVehicleMod> Mods { get; set; }
        public List<MyWindow> Windows { get; set; }
        public List<MyWheel> Wheels { get; set; }
        public List<MyDoor> Doors { get; set; }
        public List<MyBone> Bones { get; set; }

        public MyVehicle()
        {
            Mods = new List<MyVehicleMod>();
            Windows = new List<MyWindow>();
            Wheels = new List<MyWheel>();
            Doors = new List<MyDoor>();
            Bones = new List<MyBone>();
        }

        public MyVehicle(Vehicle vehicle)
        {
            ModelHash = vehicle.Model.Hash;
            ModelName = vehicle.DisplayName;
            Position = new MyVector3(vehicle.Position);
            Heading = vehicle.Heading;
            WheelType = vehicle.Mods.WheelType;
            LicensePlate = vehicle.Mods.LicensePlate;
            LicensePlateStyle = vehicle.Mods.LicensePlateStyle;
            DirtLevel = vehicle.DirtLevel;
            BodyHealth = vehicle.BodyHealth;
            EngineHealth = vehicle.EngineHealth;
            PetrolTankHealth = vehicle.PetrolTankHealth;
            RoofState = vehicle.RoofState;
            CanTiresBurst = vehicle.CanTiresBurst;
            PrimaryColor = vehicle.Mods.PrimaryColor;
            SecondaryColor = vehicle.Mods.SecondaryColor;
            PearlescentColor = vehicle.Mods.PearlescentColor;
            RimColor = vehicle.Mods.RimColor;
            TrimColor = vehicle.Mods.TrimColor;
            WindowTint = vehicle.Mods.WindowTint;

            Mods = vehicle.Mods.ToArray()
                .Select(mod => new MyVehicleMod() { Type = mod.Type, Index = mod.Index, FriendlyName = mod.LocalizedTypeName })
                .ToList();

            Doors = vehicle.Doors.ToArray()
                .Select(door => new MyDoor() { Index = door.Index, IsBroken = door.IsBroken, AngleRatio = door.AngleRatio })
                .ToList();

            Windows = new List<MyWindow>();
            foreach (VehicleWindowIndex windowIndex in Enum.GetValues(typeof(VehicleWindowIndex)))
            {
                var window = vehicle.Windows[windowIndex];

                if (window != null)
                {
                    Windows.Add(new MyWindow()
                    {
                        Index = window.Index,
                        IsIntact = window.IsIntact
                    });
                }
            }

            Bones = vehicle.Bones
                .Select(bone => new MyBone() { Index = bone.Index, Pose = new MyVector3(bone.Pose), PoseMatrix = bone.PoseMatrix })
                .ToList();
        }

        public bool SameAs(MyVehicle otherVehicle)
        {
            return this.ModelName == otherVehicle.ModelName && this.LicensePlate == otherVehicle.LicensePlate;
        }

        public void Spawn(Vector3? overridePosition = null)
        {
            var spawnPosition = overridePosition != null ? overridePosition.Value : Position.ToVector3();
            var vehicle = World.CreateVehicle(new GTA.Model(ModelHash), spawnPosition, Heading);

            vehicle.DirtLevel = DirtLevel;
            vehicle.BodyHealth = BodyHealth;
            vehicle.EngineHealth = EngineHealth;
            vehicle.PetrolTankHealth = PetrolTankHealth;
            vehicle.RoofState = RoofState;
            vehicle.CanTiresBurst = CanTiresBurst;
            vehicle.Mods.WheelType = WheelType;
            vehicle.Mods.LicensePlate = LicensePlate;
            vehicle.Mods.LicensePlateStyle = LicensePlateStyle;
            vehicle.Mods.PrimaryColor = PrimaryColor;
            vehicle.Mods.SecondaryColor = SecondaryColor;
            vehicle.Mods.PearlescentColor = PearlescentColor;
            vehicle.Mods.RimColor = RimColor;
            vehicle.Mods.TrimColor = TrimColor;
            vehicle.Mods.WindowTint = WindowTint;

            GTA.Native.Function.Call(GTA.Native.Hash.SET_VEHICLE_MOD_KIT, vehicle, 0);
            foreach (var myMod in Mods)
            {
                GTA.Native.Function.Call(GTA.Native.Hash.SET_VEHICLE_MOD, vehicle, myMod.Type, myMod.Index, false);
            }

            foreach (var myWindow in Windows.Where(w => w.IsIntact == false))
            {
                vehicle.Windows[myWindow.Index].Smash();
            }

            foreach (var myDoor in Doors)
            {
                if (myDoor.IsBroken)
                {
                    vehicle.Doors[myDoor.Index].Break(stayInTheWorld: false);
                    continue; // Skip setting door open state if broken.
                }

                if (myDoor.AngleRatio > 0)
                {
                    var vehicleDoor = vehicle.Doors[myDoor.Index];
                    vehicleDoor.Open(loose: true);
                    vehicleDoor.AngleRatio = myDoor.AngleRatio;
                }
            }

            foreach (var myBone in Bones)
            {
                var vehicleBone = vehicle.Bones[myBone.Index];
                vehicleBone.PoseMatrix = myBone.PoseMatrix;
                vehicleBone.Pose = myBone.Pose.ToVector3();
            }
        }
    }
}
