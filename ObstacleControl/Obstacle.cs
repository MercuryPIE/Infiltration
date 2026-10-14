using AT2.ObstacleControl.BaseObstacles;
using AT2.Utility;

namespace AT2.ObstacleControl
{
    public class Obstacle
    {
        
        
        private Vector Location;
        private ObstacleId CID;
        private int PriorityValue;
        private Obstacle(Vector Location, ObstacleId CID)
        {
            this.Location = Location;
            this.CID = CID;
            PriorityValue = SetPv(CID);
        }

        // -------------------------------------------------

        public Vector GetLocation() { return Location; }
        public ObstacleId GetCID() { return CID; }
        public int GetPV() { return PriorityValue; }

        // -------------------------------------------------

        public static bool IsPVHigher(int NewPV, int StoredPV)
        {
            return NewPV > StoredPV;
        }

        // -------------------------------------------------

        public static readonly Dictionary<ObstacleId, int> MapCIDToPV = new Dictionary<ObstacleId, int>
        {
            {ObstacleId.Camera, 1},
            {ObstacleId.Sensor, 2},
            {ObstacleId.Guard, 3},
            {ObstacleId.Fence, 4},
        };

        public static readonly Dictionary<int, ObstacleId> MapPVToCID = new Dictionary<int, ObstacleId>
        {
            {1, ObstacleId.Camera},
            {2, ObstacleId.Sensor},
            {3, ObstacleId.Guard},
            {4, ObstacleId.Fence}
        };

        // -------------------------------------------------

        public static int SetPv(ObstacleId Cid)
        {
            return MapCIDToPV.GetValueOrDefault(Cid, 0);
        }

        public static ObstacleId SetCid(int Pv)
        {
            return MapPVToCID.GetValueOrDefault(Pv, ObstacleId.Guard);
        }

        // -------------------------------------------------

        public static Obstacle Create(Vector Location, ObstacleId CID)
        {
            return new Obstacle(Location, CID);
        }
        public override string ToString()
        {
            return $"CID: {GetCID()} | Location: {GetLocation()}";
        }
    }
}
