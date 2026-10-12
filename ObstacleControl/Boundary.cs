using AT2.InstructionControl;
using AT2.ObstacleControl.BaseObstacles;
using AT2.Utility;

namespace AT2.ObstacleControl
{
    public static class Boundary
    {
        private enum Direction
        {
            North, South, East, West
        }
        
        private const int Padding = 5;
        private static Vector? _topLeftCorner;
        private static Vector? _bottomRightCorner;

        private static readonly Dictionary<Direction,Vector> BoundaryCore = new()
        {
            {Direction.North, new Vector(0, 0, Vector.VectorState.Empty)},
            {Direction.South, new Vector(0, 0, Vector.VectorState.Empty)},
            {Direction.East, new Vector(0, 0, Vector.VectorState.Empty)},
            {Direction.West, new Vector(0, 0, Vector.VectorState.Empty)},
        };

        
        public static void BuildBoundary(bool GenCorners = true, params Vector[] InjectedLocations)
        {

            var locationList = ObstacleFactory.Obstacles;
            
            if (locationList.Count == 0)
            {
                throw new NotImplementedException("Instruction list is empty unable to build boundary.");
            }
            
            if (InjectedLocations.Any())
            {
                foreach (var injLoc in InjectedLocations)
                {
                    CheckNewLocation(injLoc);
                }
            }

            foreach (var loc in locationList)
            {
                CheckNewLocation(loc.Location);

                if (loc.ObstacleClass == ClassId.Fence)
                {
                    //TODO: change the code for new system for getting fence data
                    CheckNewLocation(loc.Location); // This needs to be the Fence end pos.
                }
            }

            if (GenCorners)
            {
                GenerateBoundaryCorners();
            }
            
        }
        
        private static void SetLocation(Direction Direction, Vector NewLocation)
        {
            BoundaryCore[Direction] = NewLocation;
        }

        private static Vector GetLocation(Direction Direction)
        {
            return BoundaryCore[Direction];
        }

        private static void CheckNewLocation(Vector NewLocation)
        {
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                var currentLocation = GetLocation(dir);

                var (dimension, comparison) = dir switch
                {
                    Direction.North => (Vector.ComDimension.Y, Vector.ComOperator.Lt),
                    Direction.South => (Vector.ComDimension.Y, Vector.ComOperator.Gt),
                    Direction.East => (Vector.ComDimension.X, Vector.ComOperator.Gt),
                    Direction.West => (Vector.ComDimension.X, Vector.ComOperator.Lt),
                    _ => throw new ArgumentOutOfRangeException(nameof(dir))
                };
                
                if (Vector.CompareVectors(NewLocation,currentLocation, comparison, dimension))
                {
                    SetLocation(dir, NewLocation);
                }
            }
        }

        private static void GenerateBoundaryCorners()
        {
            _topLeftCorner = new Vector(
                GetLocation(Direction.West).X - Padding,
                GetLocation(Direction.North).Y - Padding);
            
            _bottomRightCorner = new Vector(
                GetLocation(Direction.East).X + Padding,
                GetLocation(Direction.West).Y + Padding);
        }
        
        
        public static Vector GetTopLeft()
        {
            return _topLeftCorner ?? throw new NullReferenceException("Unable to get top left corner, did you build the boundary?");
        }

        public static Vector GetBottomRight()
        {
            return _bottomRightCorner ?? throw new NullReferenceException("Unable to get bottom right corner, did you build the boundary?");
        }
        
        
        public static int GetSide(char Request)
        {
            return Request switch
            {
                'n' => GetTopLeft().Y,
                's' => GetBottomRight().Y,
                'e' => GetBottomRight().X,
                'w' => GetTopLeft().X,
                _ => throw new Error.InvalidDirectionException($"Invalid Direction, Must be (n, s, e or w). Not: {Request}")
            };
        }

        public static void SetBoundaryCorners(Vector TopLeftCorner, Vector BottomRightCorner)
        {
            _topLeftCorner = TopLeftCorner;
            _bottomRightCorner = BottomRightCorner;
        }

        public static bool IsInBoundary(Vector Location, out Vector CheckedVector)
        {
            CheckedVector = new Vector(0, 0, Vector.VectorState.Empty);
            
            var checkTopLeft = Vector.CompareVectors(Location, GetTopLeft(), Vector.ComOperator.Gteq);
            var checkBottomRight = Vector.CompareVectors(Location, GetBottomRight(), Vector.ComOperator.Lteq);

            if (!checkTopLeft || !checkBottomRight) return false;
            
            CheckedVector = Location;
            return true;

        }


        public static string PrintInfo()
        {
            return $"Boundary Info:\nTop Left: {_topLeftCorner}\nBottom Right: {_bottomRightCorner}";
        }
    }
}