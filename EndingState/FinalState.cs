using AT2.ObstacleControl;
using AT2.Utility;

namespace AT2.EndingState
{
    public class FinalState
    {
        private static readonly Dictionary<Vector, int> FinalObstacleList = new Dictionary<Vector, int>();

        public static Dictionary<Vector, int> AccessFinalObstacles => FinalObstacleList;

        public static void AddObstacle(Obstacle O)
        {
            Vector Key = O.GetLocation();
            int NewPV = O.GetPV();

            if (FinalObstacleList.ContainsKey(Key) && FinalObstacleList.TryGetValue(Key, out int StoredPV))
            {
                if (Obstacle.IsPVHigher(NewPV, StoredPV))
                {
                    FinalObstacleList[Key] = NewPV;
                }
            }
            else
            {
                FinalObstacleList.Add(Key, NewPV);
            }
        }
        public static void Wipe()
        {
            FinalObstacleList.Clear();
            AccessFinalObstacles.Clear();
        }

    }
}
