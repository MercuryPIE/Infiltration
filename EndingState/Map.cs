using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AT2.ObstacleControl;
using AT2.Utility;
using static AT2.ObstacleControl.Boundary;
using static AT2.Utility.Error;


namespace AT2.EndingState
{
    public class Map
    {
        public static void CreateMap()
        {
            while (true)
            {
                Vector TL = CreateBoundaryCorner(Corner.TopLeft);
                Vector BR = CreateBoundaryCorner(Corner.BottomRight);

                if (CheckMapDimension(TL, BR))
                {
                    Boundary MapBoundary = SetBoundary(TL, BR);
                    Instruction.CalculateInstructions(MapBoundary);
                    DisplayMap(MapBoundary);
                    break;
                }
            }
        }
        private static Vector CreateBoundaryCorner(Corner Request)
        {
            Vector? ValidLocation = null;

            while (ValidLocation == null)
            {
                string CornerName = Request == Corner.TopLeft ? "top-left" : "bottom-right";
                Console.WriteLine($"Enter the location of the {CornerName} top-left cell of the map (X,Y):");

                string? Input = Console.ReadLine();

                if (Vector.IsLocationValid(Input, out Vector Location))
                {
                    ValidLocation = Location;
                }
            }
            return ValidLocation!;
        }

        private static void DisplayMap(Boundary MapBoundary)
        {
            StringBuilder SB = new StringBuilder();

            Vector TL = MapBoundary.GetCorner(Corner.TopLeft);
            Vector BR = MapBoundary.GetCorner(Corner.BottomRight);

            for (int Y = TL.GetY(); Y <= BR.GetY(); Y++)
            {
                for (int X = TL.GetX(); X <= BR.GetX(); X++)
                {
                    Vector CKey = Vector.CreateVector(X, Y);

                    if (FinalState.AccessFinalObstacles.ContainsKey(CKey) && FinalState.AccessFinalObstacles.TryGetValue(CKey, out int PV))
                    {
                        SB.Append((char)Obstacle.SetCid(PV));
                    }
                    else
                    {
                        SB.Append('.');
                    }
                }
                SB.AppendLine();
            }
            Console.WriteLine(SB.ToString());
        }

        private static bool CheckMapDimension(Vector TopLeft, Vector BottomRight)
        {
            try
            {
                Validate.MapDimension(TopLeft, BottomRight);
                return true;
            }
            catch (InvalidMapDimensionException ExM)
            {
                Console.WriteLine($"Error: {ExM.Message}");
                return false;
            }
        }

    }
}