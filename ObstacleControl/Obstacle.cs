using AT2.InstructionControl;
using AT2.Utility;

namespace AT2.ObstacleControl
{
    public class Obstacle
    {
        
        
        private Vector Location;
        private ClassId CID;
        private int PriorityValue;
        private Obstacle(Vector Location, ClassId CID)
        {
            this.Location = Location;
            this.CID = CID;
            PriorityValue = SetPv(CID);
        }

        // -------------------------------------------------

        public Vector GetLocation() { return Location; }
        public ClassId GetCID() { return CID; }
        public int GetPV() { return PriorityValue; }

        // -------------------------------------------------

        public static bool IsPVHigher(int NewPV, int StoredPV)
        {
            return NewPV > StoredPV;
        }

        // -------------------------------------------------

        public static readonly Dictionary<ClassId, int> MapCIDToPV = new Dictionary<ClassId, int>
        {
            {ClassId.Camera, 1},
            {ClassId.Sensor, 2},
            {ClassId.Guard, 3},
            {ClassId.Fence, 4},
        };

        public static readonly Dictionary<int, ClassId> MapPVToCID = new Dictionary<int, ClassId>
        {
            {1, ClassId.Camera},
            {2, ClassId.Sensor},
            {3, ClassId.Guard},
            {4, ClassId.Fence}
        };

        // -------------------------------------------------

        public static int SetPv(ClassId Cid)
        {
            return MapCIDToPV.GetValueOrDefault(Cid, 0);
        }

        public static ClassId SetCid(int Pv)
        {
            return MapPVToCID.GetValueOrDefault(Pv, ClassId.Guard);
        }

        // -------------------------------------------------

        public static Obstacle Create(Vector Location, ClassId CID)
        {
            return new Obstacle(Location, CID);
        }
        public override string ToString()
        {
            return $"CID: {GetCID()} | Location: {GetLocation()}";
        }
    }
}
