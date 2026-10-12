using AT2.EndingState;
using AT2.InstructionControl;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles
{
    public class Fence : IObstacleExtension<Vector>
    {
        public Vector Location { get; set; }
        public Vector ExtensionData { get; set; }
        public ClassId ObstacleClass => ClassId.Fence;

        public Fence(Vector StartLocation)
        {
            Location = StartLocation;
            ExtensionData = SetExtensionData();

        }
        
        public Vector SetExtensionData()
        {
            while (true)
            {
                Console.WriteLine("Enter the location where the fence ends (X,Y):");
                var consoleInput = Console.ReadLine();

                if (!Vector.IsLocationValid(consoleInput, out var endLocation)) continue;
                
                try
                {
                    Validate.FenceRange(Location, endLocation);
                    return endLocation;
                }
                catch (Error.InvalidRangeException exM)
                {
                    Console.WriteLine($"Error: {exM.Message}");
                }
            }
        }
        
        public void CalculateObstacle()
        {
            var start = Location;
            var end = ExtensionData;
            
            FinalState.AddObstacle(Obstacle.Create(start, ObstacleClass));
            FinalState.AddObstacle(Obstacle.Create(end, ObstacleClass));

            var isHorizontal = start.X == end.X;

            var stepControl = isHorizontal ? Math.Sign(end.Y - start.Y) : Math.Sign(end.X - start.X);
            
            var loopStart = isHorizontal ? start.Y : start.X;
            var loopEnd = isHorizontal ? end.Y : end.X;

            for (var i = loopStart; i != loopEnd; i += stepControl)
            {
                var x = isHorizontal ? start.X : i;
                var y = isHorizontal ? i : start.Y;
                
                FinalState.AddObstacle(Obstacle.Create(new Vector(x, y), ObstacleClass));
            }
        }
        
        public Vector GetExtensionData()
        {
            return ExtensionData;
        }
    }
}
