using static AT2.Utility.Error;

namespace AT2.Utility
{
    public static class Validate
    {
        private static void IsConsoleEmpty(string? ConsoleInput)
        {
            if (string.IsNullOrEmpty(ConsoleInput))
            {
                throw new EmptyConsoleException("Invalid Input: Nothing Detected!\n");
            }
        }
        
        public static char CameraDirection(string? Direction)
        {
            IsConsoleEmpty(Direction);

            if (!char.TryParse(Direction, out var directionChar))
            {
                throw new InvalidInputException(
                    "Invalid input. Direction Cannot be numbers or symbols Must be (n, s, e or w).\n");
            }

            if (directionChar is not ('n' or 's' or 'e' or 'w'))
            {
                throw new InvalidDirectionException("Invalid Direction, Must be (n, s, e or w).\n");
            }
            
            return directionChar;
            
        }

        public static double SensorRange(string? Range)
        {
            IsConsoleEmpty(Range);

            if (!double.TryParse(Range, out var result))
            {
                throw new InvalidInputException("Invalid input. Range must be numbers, Not letters or symbols.\n");
            }

            if (result <= 0)
            {
                throw new RangeBelowZeroException("Invalid Input. Range cannot be less than or equal to 0.\n");
            }
            
            return result;
        }

        public static Vector Location(string? Location)
        {
            IsConsoleEmpty(Location);
            
            var locationArray = Location!.Split(',');

            if (locationArray.Length != 2)
            {
                throw new InvalidFormatException("Invalid input format. Location must be in the form of (X, Y).\n");
            }

            if (!int.TryParse(locationArray[0], out var x) || !int.TryParse(locationArray[1], out var y))
            {
                throw new InvalidInputException("Invalid input. Location must be numbers, Not letters or symbols.\n");
            } 
            
            return new Vector(x, y);
        }

        public static void FenceRange(Vector Start, Vector End)
        {
            if (Vector.CompareVectors(Start, End, Vector.ComOperator.Neq))
            {
                throw new InvalidRangeException("Invalid Range. Fences must be horizontal or vertical.\n");
            }
            if (Vector.CompareVectors(Start, End, Vector.ComOperator.Eq))
            {
                throw new InvalidRangeException("Invalid Range. Fences must be more than one location.\n");
            }
        }

        public static void MapDimension(Vector V1, Vector V2)
        {
            if (Vector.CompareVectors(V2, V1, Vector.ComOperator.Lt, Vector.ComDimension.Either))
            {
                throw new InvalidMapDimensionException("Invalid Map Dimensions. Bottom Right must be greater than Top Left for a valid map.\n");
            }
            
        }
    }
}