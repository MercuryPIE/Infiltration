using AT2.EndingState;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles
{
    public class Fence : IObstacleExtension<Vector>
    {
        public Vector Location { get; set; }
        public Vector ExtensionData { get; set; }
        private readonly ClassId _obstacleClass = ClassId.Fence;

        public Fence(Vector StartLocation, Vector EndLocation)
        {
            Location = StartLocation;
            ExtensionData = EndLocation;

        }
        
        public void CalculateObstacle()
        {
            Vector start = Location;
            Vector end = ExtensionData;
            
            FinalState.AddObstacle(Obstacle.Create(start, _obstacleClass));
            FinalState.AddObstacle(Obstacle.Create(end, _obstacleClass));

            var isHorizontal = start.X == end.X;

            var stepControl = isHorizontal ? Math.Sign(end.Y - start.Y) : Math.Sign(end.X - start.X);
            
            var loopStart = isHorizontal ? start.Y : start.X;
            var loopEnd = isHorizontal ? end.Y : end.X;

            for (var i = loopStart; i != loopEnd; i += stepControl)
            {
                var x = isHorizontal ? start.X : i;
                var y = isHorizontal ? i : start.Y;
                
                FinalState.AddObstacle(Obstacle.Create(new Vector(x, y), _obstacleClass));
            }
        }
        
        public void RemoveExtensionData()
        {
            ExtensionData = new Vector(0, 0, Vector.VectorState.Empty);
        }

        public Vector GetExtensionData()
        {
            return ExtensionData;
        }
    }
}
