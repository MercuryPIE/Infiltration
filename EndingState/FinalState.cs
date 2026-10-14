using AT2.ObstacleControl;
using AT2.Utility;

namespace AT2.EndingState
{
    public class FinalState
    {
        public static Dictionary<Vector, int> AccessFinalObstacles { get; } = new();

        public static void AddObstacle(Obstacle O)
        {
            var key = O.GetLocation();
            var newPv = O.GetPV();

            if (AccessFinalObstacles.ContainsKey(key) && AccessFinalObstacles.TryGetValue(key, out var storedPv))
            {
                if (Obstacle.IsPVHigher(newPv, storedPv))
                {
                    AccessFinalObstacles[key] = newPv;
                }
            }
            else
            {
                AccessFinalObstacles.Add(key, newPv);
            }
        }
        public static void Wipe()
        {
            AccessFinalObstacles.Clear();
        }

    }
}
