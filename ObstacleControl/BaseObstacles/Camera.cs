using AT2.EndingState;
using AT2.InstructionControl;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles
{
    public class Camera : IObstacleExtension<char>
    {
        public Vector Location { get; set; }
        public char ExtensionData { get; set; }
        public ClassId ObstacleClass => ClassId.Camera;

        public Camera(Vector Location)
        {
            this.Location = Location;
            ExtensionData = SetExtensionData();
        }
        
        public char GetExtensionData()
        {
            return ExtensionData;
        }

        public char SetExtensionData()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter the direction the camera is facing (n, s, e or w):");
                    
                    var consoleInput = Console.ReadLine();

                    return Validate.CameraDirection(consoleInput);
                }
                catch (Error.InvalidDirectionException exM)
                {
                    Console.WriteLine($"Error: {exM.Message}");
                }
                catch (Error.InvalidInputException exM)
                {
                    Console.WriteLine($"Error: {exM.Message}");
                }
                catch (Error.EmptyConsoleException exM)
                {
                    Console.WriteLine($"Error: {exM.Message}");
                }
            }
        }


        public void CalculateObstacle()
        {
            // Add staring point.
            FinalState.AddObstacle(Obstacle.Create(Location, ObstacleClass));
            
            var boundaryEdge = Boundary.GetSide(ExtensionData);
            
            var distance = ExtensionData switch
            {
                'n' or 's' => Math.Abs(Location.Y - boundaryEdge),
                'e' or 'w' => Math.Abs(Location.X - boundaryEdge),
                 _ => throw new Error.InvalidInputException("Invalid camera direction")
            };
            
            for (var step = 1; step <= distance; step++)
            {
                var level = MoveTowardsBoundary(step);
                BuildLevel(level, step);
            }
        }

        private Vector MoveTowardsBoundary(int Step)
        {
            return ExtensionData switch
            {
                'n' => new Vector(Location.X, Location.Y - Step),
                's' => new Vector(Location.X, Location.Y + Step),
                'e' => new Vector(Location.X + Step, Location.Y),
                'w' => new Vector(Location.X - Step, Location.Y),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void BuildLevel(Vector Level, int Step)
        {
            for (var offset = -Step; offset <= Step; offset++)
            {
                var location = ExtensionData is 'n' or 's'
                    ? new Vector(Location.X + offset, Level.Y)
                    : new Vector(Level.X, Location.Y + offset);
                
                // Check if the generated location falls in the boundary.
                if (Boundary.IsInBoundary(location, out var calculatedLocation))
                {
                    // Add location to the FinalState.
                    FinalState.AddObstacle(Obstacle.Create(calculatedLocation, ObstacleClass));
                }
            }
        }
    }
}