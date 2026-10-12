using static AT2.Utility.Error;

namespace AT2.Utility
{
    public class Vector
    {
        // -------------------------------------------------
        
        public enum VDirection
        {
            North, South, East, West
        }
        
        public enum VectorState
        {
            Valid, Empty
        }

        public readonly int X;
        public readonly int Y;
        public VectorState State;
        
        public Vector(int X, int Y, VectorState State = VectorState.Valid)
        {
            this.X = X;
            this.Y = Y;
            this.State = State;
        }

        // -------------------------------------------------

        public override bool Equals(object? Obj)
        {
            if (Obj == null || GetType() != Obj.GetType())
            {
                return false;
            }
            
            Vector other = (Vector)Obj;
            
            return CompareVectors(this, other, ComOperator.Eq);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        // -------------------------------------------------

        public enum ComDimension
        {
            X, Y, Both, Either
        }
        
        public enum ComOperator
        {
            Eq, // Equals,
            Neq, // Not Equals,
            Gteq, // Greater Than Or Equal To,
            Lteq, // Less Than Or Equal To,
            Lt, // Less Than,
            Gt // Greater Than,
        }
        
        public static bool CompareVectors(Vector Vector1, Vector Vector2, ComOperator Operator, ComDimension Type = ComDimension.Both)
        {
            return Type switch
            {
                ComDimension.X => CompareValues(Vector1.X, Vector2.X, Operator),
                ComDimension.Y => CompareValues(Vector1.Y, Vector2.Y, Operator),
                
                ComDimension.Both => CompareValues(Vector1.X, Vector2.X, Operator) &&
                                     CompareValues(Vector1.Y, Vector2.Y, Operator),
                
                ComDimension.Either => CompareValues(Vector1.X, Vector2.X, Operator) ||
                                       CompareValues(Vector1.Y, Vector2.Y, Operator),
                _ => false
            };
        }

        public static bool CompareVectors(Vector Vector1, Vector Vector2, VDirection Direction)
        {
            return Direction switch
            {
                VDirection.North => CompareVectors(Vector1, Vector2, ComOperator.Lt, ComDimension.Y),
                VDirection.East => CompareVectors(Vector1, Vector2, ComOperator.Gt, ComDimension.X),
                VDirection.South => CompareVectors(Vector1, Vector2, ComOperator.Gt, ComDimension.Y),
                VDirection.West => CompareVectors(Vector1, Vector2, ComOperator.Lt, ComDimension.X),
                _ => false
            };
        }
        
        private static bool CompareValues(int Value1, int Value2, ComOperator Operator)
        {
            return Operator switch
            {
                ComOperator.Eq => Value1 == Value2,
                ComOperator.Neq => Value1 != Value2,
                ComOperator.Gt => Value1 > Value2,
                ComOperator.Lt => Value1 < Value2,
                ComOperator.Lteq => Value1 <= Value2,
                ComOperator.Gteq => Value1 >= Value2,
                _ => false
            };
        }
        
        // -------------------------------------------------
        
        public static bool IsLocationValid(string? Location, out Vector CheckedVector)
        {
            try
            {
                CheckedVector = Validate.Location(Location);
                return true;
            }
            catch (InvalidInputException exM)
            {
                Console.WriteLine($"Error: {exM.Message}");
                CheckedVector = new Vector(0, 0, VectorState.Empty);
                return false;
            }
            catch (InvalidFormatException exM)
            {
                Console.WriteLine($"Error: {exM.Message}");
                CheckedVector = new Vector(0, 0, VectorState.Empty);
                return false;
            }
            catch (EmptyConsoleException exM)
            {
                Console.WriteLine($"Error: {exM.Message}");
                CheckedVector = new Vector(0, 0, VectorState.Empty);
                return false;
            }
        }
        
        // -------------------------------------------------

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}