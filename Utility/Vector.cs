using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AT2.Utility.Error;

namespace AT2.Utility
{
    public class Vector : Debug
    {
        public struct Range
        {
            private readonly int MinValue, MaxValue;
            private Range(int MinValue, int MaxValue)
            {
                this.MinValue = MinValue;
                this.MaxValue = MaxValue;
            }

            public readonly int GetMin() { return MinValue; }
            public readonly int GetMax() { return MaxValue; }

            public static Range CreateRange(int Min, int Max)
            {
                return new Range(Min, Max);
            }
            public static Range UpdateRange(int NewMin, int NewMax)
            {
                return CreateRange(NewMin, NewMax);
            }

            public override string ToString()
            {
                return $"({MinValue}, {MaxValue})";
            }
        }

        // -------------------------------------------------

        public enum VectorState
        {
            Valid, Empty
        }

        private int X, Y;
        private readonly VectorState State;
        public Vector(int X, int Y, VectorState State)
        {
            this.X = X;
            this.Y = Y;
            this.State = State;
        }

        // -------------------------------------------------

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Vector other = (Vector)obj;
            return CompareVectors(this, other, ComOperator.EQ);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        // -------------------------------------------------

        public enum ComDimension
        {
            X, Y, Both
        }
        public enum ComOperator
        {
            EQ, // Equals,
            GTEQ, // Greater Than Or Equal To,
            LTEQ, // Less Than Or Equal To,
            LT, // Less Than,
            GT // Greater Than,
        }
        public static bool CompareVectors(Vector Vector_1, Vector Vector_2, ComOperator Operator, ComDimension Type = ComDimension.Both)
        {
            switch (Type)
            {
                case ComDimension.X:
                    return CompareValues(Vector_1.GetX(), Vector_2.GetX(), Operator);

                case ComDimension.Y:
                    return CompareValues(Vector_1.GetY(), Vector_2.GetY(), Operator);

                case ComDimension.Both:
                    return CompareValues(Vector_1.GetX(), Vector_2.GetX(), Operator) && CompareValues(Vector_1.GetY(), Vector_2.GetY(), Operator);

                default:
                    return false;
            }
        }
        private static bool CompareValues(int Value_1, int Value_2, ComOperator Operator)
        {
            switch (Operator)
            {
                case ComOperator.EQ:
                    return Value_1 == Value_2;

                case ComOperator.GT:
                    return Value_1 > Value_2;

                case ComOperator.LT:
                    return Value_1 < Value_2;

                case ComOperator.LTEQ:
                    return Value_1 <= Value_2;

                case ComOperator.GTEQ:
                    return Value_1 >= Value_2;

                default:
                    return false;
            }
        }

        // -------------------------------------------------

        public int GetX()
        {
            return X;
        }
        public int GetY()
        {
            return Y;
        }
        public VectorState GetState()
        {
            return State;
        }

        // -------------------------------------------------

        public static Vector CreateVector(int X, int Y)
        {
            return new Vector(X, Y, VectorState.Valid);
        }
        public static Vector CreateEmptyVector()
        {
            return new Vector(0, 0, VectorState.Empty);
        }
        public static Vector UpdateVector(int NewX, int NewY)
        {
            return CreateVector(NewX, NewY);
        }

        public static bool IsLocationValid(string? Location, out Vector CheckedVector)
        {
            try
            {
                CheckedVector = Validate.Location(Location);
                return true;
            }
            catch (InvalidInputException ExM)
            {
                Console.WriteLine($"Error: {ExM.Message}");
                CheckedVector = CreateEmptyVector();
                return false;
            }
            catch (InvalidFormatException ExM)
            {
                Console.WriteLine($"Error: {ExM.Message}");
                CheckedVector = CreateEmptyVector();
                return false;
            }
            catch (EmptyConsoleException ExM)
            {
                Console.WriteLine($"Error: {ExM.Message}");
                CheckedVector = CreateEmptyVector();
                return false;
            }
        }

        // -------------------------------------------------

        public override string ToString()
        {
            return $"({GetX()}, {GetY()})";
        }
    }
}