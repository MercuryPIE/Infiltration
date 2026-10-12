using System.Text;
using AT2.ObstacleControl;
using AT2.Utility;

namespace AT2.EndingState
{
    internal static class Show
    {
        public static void SafeDirections()
        {
            while (true)
            {
                Console.WriteLine("Enter your current location (X,Y):");
                var consoleInput = Console.ReadLine();

                if (!Vector.IsLocationValid(consoleInput, out var validLocation)) continue;
                
                Boundary.BuildBoundary(true, validLocation);
                Console.WriteLine(GetSafeLocations(validLocation));

                break;
            }
        }

        private static string GetSafeLocations(Vector Location)
        {
            if (FinalState.AccessFinalObstacles.ContainsKey(Location))
            {
                return "Agent, your location is compromised. Abort mission.\n";
            }

            var safeDirections = new StringBuilder();

            var safeLocations = new Dictionary<string, Vector>
            {
                { "N", new Vector(Location.X, Location.Y - 1) }, // North
                { "S", new Vector(Location.X, Location.Y + 1) }, // South
                { "W", new Vector(Location.X - 1, Location.Y) }, // West
                { "E", new Vector(Location.X + 1, Location.Y) }  // East
                
            };

            foreach (var kvp in safeLocations.Where(Kvp => !FinalState.AccessFinalObstacles.ContainsKey(Kvp.Value)))
            {
                safeDirections.Append(kvp.Key);
            }

            return safeDirections.Length > 0 ?
                $"You can safely take any of the following directions: {safeDirections}\n" :
                "You cannot safely move in any direction. Abort mission.\n";
        }
    }
}