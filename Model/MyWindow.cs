using GTA;
using GTA.Math;
using System.Windows.Forms;

namespace GtaVehiclePersistence.Model
{
    public class MyBone
    {
        public int Index { get; set; }
        public MyVector3 Pose { get; set; }
        public Matrix PoseMatrix { get; set; }

        public MyBone()
        {
        }

        public MyBone(EntityBone bone)
        {
            Index = bone.Index;
            Pose = new MyVector3(bone.Pose);
            PoseMatrix = bone.PoseMatrix;
        }
    }

    public class MyWindow
    {
        public VehicleWindowIndex Index { get; set; }
        public bool IsIntact { get; set; }
    }
}
