using AT2.EndingState;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles
{
    public class Guard : IObstacleBase
    {
        public Vector Location { get; set; }

        public ObstacleId ObstacleClass => ObstacleId.Guard;

        public Guard(Vector Location)
        {
            this.Location = Location;
        }
        
        public void CalculateObstacle()
        {
            FinalState.AddObstacle(Obstacle.Create(Location, ObstacleClass));
        }

        
    }
}
