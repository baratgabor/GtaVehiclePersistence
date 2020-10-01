using GTA.Math;

namespace GtaVehiclePersistence.Model
{
    public class MyVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public MyVector3()
        {
        }

        public MyVector3(Vector3 vector3)
        {
            X = vector3.X;
            Y = vector3.Y;
            Z = vector3.Z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
