using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static AT2.Utility.Error;

namespace AT2.Utility
{
    public class Validate
    {
        public static char CameraDirection(string? Direction, out char Data)
        {
            if (!string.IsNullOrEmpty(Direction))
            {
                if (char.TryParse(Direction, out char Face))
                {
                    if (Face == 'n' || Face == 's' || Face == 'e' || Face == 'w')
                    {
                        Data = Face;
                        return Face;
                    }
                    else { throw new InvalidDirectionException("Invalid Direction, Must be (n, s, e or w).\n"); }
                }
                else { throw new InvalidInputException("Invalid input. Direction Cannot be numbers or symbols Must be (n, s, e or w).\n"); }
            }
            else { throw new EmptyConsoleException("Invalid Input: Nothing Detected!\n"); }
        }

        public static double SensorRange(string? Range, out double Data)
        {
            if (!string.IsNullOrEmpty(Range))
            {
                if (double.TryParse(Range, out double value))
                {
                    if (value > 0)
                    {
                        Data = value;
                        return value;
                    }
                    else { throw new RangeBelowZeroException("Invalid Input. Range cannot be less than or equal to 0.\n"); }
                }
                else { throw new InvalidInputException("Invalid input. Range must be numbers, Not letters or symbols.\n"); }
            }
            else { throw new EmptyConsoleException("Invalid Input: Nothing Detected!\n"); }
        }

        public static Vector Location(string? Location)
        {
            if (!string.IsNullOrEmpty(Location))
            {
                string[] LocationArray = Location.Split(',');

                if (LocationArray.Length == 2)
                {
                    if (int.TryParse(LocationArray[0], out int X) && int.TryParse(LocationArray[1], out int Y))
                    {
                        return Vector.CreateVector(X, Y);
                    }
                    else { throw new InvalidInputException("Invalid input. Location must be numbers, Not letters or symbols.\n"); }
                }
                else { throw new InvalidFormatException("Invalid input format. Location must be in the form of (X, Y).\n"); }
            }
            else { throw new EmptyConsoleException("Invalid Input: Nothing Detected!\n"); }
        }

        public static void FenceRange(Vector Start, Vector End)
        {
            if (Start.GetX() != End.GetX() && Start.GetY() != End.GetY())
            {
                throw new InvalidRangeException("Invalid Range. Fences must be horizontal or vertical.\n");
            }
            if (Start.GetX() == End.GetX() && Start.GetY() == End.GetY())
            {
                throw new InvalidRangeException("Invalid Range. Fences must be more than one location.\n");
            }
        }

        public static void MapDimension(Vector V1, Vector V2)
        {
            if (V2.GetX() < V1.GetX() || V2.GetY() < V1.GetY())
            {
                throw new InvalidMapDimensionException("Invalid Map Dimensions. Bottom Right must be greater than Top Left for a valid map.\n");
            }
        }
    }
}