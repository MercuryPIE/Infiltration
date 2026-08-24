using System.Text;
using AT2.ObstacleControl;
using AT2.Utility;

namespace AT2.EndingState
{
    internal class Show
    {
        public static void SafeDirections()
        {
            while (true)
            {
                Console.WriteLine("Enter your current location (X,Y):");
                string? Input = Console.ReadLine();

                if (Vector.IsLocationValid(Input, out Vector ValidLocation))
                {
                    Instruction.CalculateInstructions(Boundary.Calculate(ValidLocation));
                    Console.WriteLine(GetSafeLocations(ValidLocation));

                    break;
                }
            }
        }

        public static string GetSafeLocations(Vector Location)
        {
            if (FinalState.AccessFinalObstacles.ContainsKey(Location))
            {
                return "Agent, your location is compromised. Abort mission.\n";
            }

            StringBuilder SafeDirections = new StringBuilder();

            Dictionary<string, Vector> SafeLocations = new Dictionary<string, Vector>
            {
                { "N", Vector.CreateVector(Location.GetX(), Location.GetY() - 1) }, // North
                { "S", Vector.CreateVector(Location.GetX(), Location.GetY() + 1) }, // South
                { "W", Vector.CreateVector(Location.GetX() - 1, Location.GetY()) }, // West
                { "E", Vector.CreateVector(Location.GetX() + 1, Location.GetY()) }  // East
                
            };

            foreach (var KVP in SafeLocations)
            {
                if (!FinalState.AccessFinalObstacles.ContainsKey(KVP.Value))
                {
                    SafeDirections.Append(KVP.Key);
                }
            }

            return SafeDirections.Length > 0 ?
                $"You can safely take any of the following directions: {SafeDirections}\n" :
                $"You cannot safely move in any direction. Abort mission.\n";
        }
    }
}