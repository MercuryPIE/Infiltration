using System.Text;
using AT2.ObstacleControl;
using AT2.ObstacleControl.BaseObstacles;
using AT2.Utility;
using static AT2.ObstacleControl.Boundary;
using static AT2.Utility.Error;


namespace AT2.EndingState
{
    public static class Map
    {
        public static void CreateMap()
        {
            while (true)
            {
                var tl = CreateBoundaryCorner(true);
                var br = CreateBoundaryCorner(false);

                if (!CheckMapDimension(tl, br)) continue;
                
                BuildBoundary(false);
                SetBoundaryCorners(tl, br);
                
                ObstacleFactory.Build();
                DisplayMap();
                
                break;
            }
        }
        private static Vector CreateBoundaryCorner(bool IsTopLeft)
        {
            Vector? validLocation;

            while (true)
            {
                var cornerName = IsTopLeft ? "top-left" : "bottom-right";
                Console.WriteLine($"Enter the location of the {cornerName} cell of the map (X,Y):");

                var consoleInput = Console.ReadLine();

                if (!Vector.IsLocationValid(consoleInput, out var location)) continue;
                validLocation = location;
                break;
            }
            return validLocation!;
        }

        private static void DisplayMap()
        {

            var finalObList = FinalState.AccessFinalObstacles;
            var sb = new StringBuilder();


            var topLeft = GetTopLeft();
            var bottomRight = GetBottomRight();
            
            
            for (var y = topLeft.Y; y <= bottomRight.Y; y++)
            {
                for (var x = topLeft.X; x <= bottomRight.X; x++)
                {
                    var cKey = new Vector(x, y);

                    if (finalObList.ContainsKey(cKey) && finalObList.TryGetValue(cKey, out var pv))
                    {
                        sb.Append((char)Obstacle.SetCid(pv));
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());
        }

        private static bool CheckMapDimension(Vector TopLeft, Vector BottomRight)
        {
            try
            {
                Validate.MapDimension(TopLeft, BottomRight);
                return true;
            }
            catch (InvalidMapDimensionException exM)
            {
                Console.WriteLine($"Error: {exM.Message}");
                return false;
            }
        }

    }
}