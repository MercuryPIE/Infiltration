using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AT2.Utility;

namespace AT2.ObstacleControl
{
    public class Boundary
    {
        /// <summary>
        /// Represents a set of Vectors associated with cardinal directions.
        /// </summary>
        private struct LinkSet
        {
            private static Vector North = Vector.CreateEmptyVector();
            private static Vector South = Vector.CreateEmptyVector();
            private static Vector East = Vector.CreateEmptyVector();
            private static Vector West = Vector.CreateEmptyVector();

            /// <summary>
            /// Private constructor for initialising the LinkSet with the specified starting Vector.
            /// </summary>
            /// <param name="StartSet">The starting Vector for the LinkSet.</param>
            private LinkSet(Vector StartSet)
            {
                North = StartSet;
                South = StartSet;
                East = StartSet;
                West = StartSet;
            }

            /// <summary>
            /// Gets the Vector associated with the specified direction.
            /// </summary>
            /// <param name="Key">The direction for which to retrieve the Vector.</param>
            /// <returns>
            /// The Vector associated with the specified direction.
            /// </returns>
            public static Vector Get(Direction Key)
            {
                Dictionary<Direction,Vector> NSEWIndex = new Dictionary<Direction, Vector>
                {
                    {Direction.North, North},
                    {Direction.South, South},
                    {Direction.East, East},
                    {Direction.West, West},
                };

                return NSEWIndex[Key];
             }

            /// <summary>
            /// Sets the Vector associated with the specified direction to the provided new location.
            /// </summary>
            /// <param name="Direction">The direction for which to set the new location.</param>
            /// <param name="NewLocation">The new Vector location to set for the specified direction.</param>
            public static void Set(Direction Direction, Vector NewLocation)
            {
                switch (Direction)
                {
                    case Direction.North:
                        North = NewLocation;
                        break;

                    case Direction.South:
                        South = NewLocation;
                        break;

                    case Direction.East:
                        East = NewLocation;
                        break;

                    case Direction.West:
                        West = NewLocation;
                        break;
                }
            }

            /// <summary>
            /// Creates a new instance of the LinkSet class with the specified starting Vector.
            /// </summary>
            /// <param name="Start">The starting Vector for the new LinkSet.</param>
            /// <returns>A new instance of the LinkSet struct.</returns>
            public static LinkSet Create(Vector Start)
            {
                return new LinkSet(Start);
            }
        }

        public enum Corner
        {
            TopLeft, BottomRight
        }

        /// <summary>
        /// Enumeration representing cardinal directions (North, South, East, West).
        /// </summary>
        public enum Direction
        {
            North, South, East, West
        }

        const int Padding = 5;
        private readonly Vector TL, BR;

        private Boundary(Vector TL, Vector BR)
        {
            this.TL = TL;
            this.BR = BR;
        }

        // -------------------------------------------------
        public Vector GetCorner(Corner Request)
        {
            Vector? Response = Request switch
            {
                Corner.TopLeft => TL,
                Corner.BottomRight => BR,
                _ => null
            };
            return Response!;
        }
        public int GetSide(char Request)
        {
            Dictionary<char, int> CharToDirection = new Dictionary<char, int>
            {
                {'n', TL.GetY()},
                {'s', BR.GetY()},
                {'e', BR.GetX()},
                {'w', TL.GetX()},
            };

            return CharToDirection[Request];
        }

        // -------------------------------------------------

        private static Vector GetBoundaryCorner(Corner CornerName)
        {
            // Get the Direction of X and Y (DX, DY).
            Direction DX = CornerName == Corner.TopLeft ? Direction.West : Direction.East;
            Direction DY = CornerName == Corner.TopLeft ? Direction.North : Direction.South;

            // Using that Direction, we get the vectors of that corner.
            Vector X = LinkSet.Get(DX);
            Vector Y = LinkSet.Get(DY);

            // Calculate the corner with -/+ of padding.
            int RX = X.GetX() + (CornerName == Corner.TopLeft ? -Padding : +Padding);
            int RY = Y.GetY() + (CornerName == Corner.TopLeft ? -Padding : +Padding);

            return Vector.CreateVector(RX, RY);
        }
        // -------------------------------------------------

        public static Boundary Calculate(params Vector[] InjectedLocations)
        {
            Vector Start = Instruction.AccessInstructionList.Count == 0
                ? Vector.CreateEmptyVector()
                : Instruction.AccessInstructionList[0].GetLocation();

            LinkSet.Create(Start);

            if (InjectedLocations.Any())
            {
                foreach (Vector V in InjectedLocations)
                {
                    CheckCoreSet(V);
                }
            }

            foreach (Instruction PulledInstruction in Instruction.AccessInstructionList)
            {
                Vector Location = PulledInstruction.GetLocation();
                CheckCoreSet(Location);

                if (PulledInstruction.GetCID() == ClassId.Fence)
                {
                    PulledInstruction.GetLink().GetExtensionData(out Vector LinkLocation);
                    CheckCoreSet(LinkLocation);
                }
            }
            return new Boundary(GetBoundaryCorner(Corner.TopLeft), GetBoundaryCorner(Corner.BottomRight));
        }

        private static void CheckCoreSet(Vector UpdatedLocation)
        {
            // For each direction (NSEW) preform a comparison to update the NSEW values.
            foreach (Direction LinkDirection in Enum.GetValues(typeof(Direction)))
            {
                // Grab the stored location, from the linkset
                Vector StoredLocation = LinkSet.Get(LinkDirection);

                // This is the main comparison of the direction:
                /*
                IsNorthOrSouth is used to determine the Comparison axis (X,Y). So if the direction is North or South
                This condition is true else false.

                IsSouthOrEast is used to determine the Comparison operator (<, >). so if the direction is South or East
                This condition is true else false
                 */
                bool INOS = LinkDirection is Direction.North or Direction.South; // For the ComDim
                bool ISOE = LinkDirection is Direction.South or Direction.East; // For the ComOPs

                // Using the value above from INOS we can load the ComDimension into ComDim. In the case of INOS being true
                // the ComDim would be Y else it would be X
                Vector.ComDimension ComDim = INOS ? Vector.ComDimension.Y : Vector.ComDimension.X;

                // Using the value above from ISOE, we can load the ComOperator into ComOpr. In the case of ISOE being true
                // the ComOpr would be Greater than else it would be less then.
                Vector.ComOperator ComOpr = ISOE ? Vector.ComOperator.GT : Vector.ComOperator.LT;

                // Checks is the stored location is less than or greater than the updated location on the X or Y,
                // this is determined by ISOE & INOS.
                if (Vector.CompareVectors(UpdatedLocation, StoredLocation, ComOpr, ComDim))
                {
                    LinkSet.Set(LinkDirection, UpdatedLocation);
                }
            }
        }

        public static Boundary SetBoundary(Vector TL, Vector BR)
        {
            return new Boundary(TL, BR);
        }

        public static bool IsInBoundary(Vector Location, Boundary PassedBoundary, out Vector CheckedVector)
        {
            CheckedVector = new Vector(0, 0, Vector.VectorState.Empty);

            Vector topLeft = PassedBoundary.GetCorner(Corner.TopLeft);
            Vector bottomRight = PassedBoundary.GetCorner(Corner.BottomRight);

            bool checkTopLeft = Vector.CompareVectors(Location, topLeft, Vector.ComOperator.GTEQ);
            bool checkBottomRight = Vector.CompareVectors(Location, bottomRight, Vector.ComOperator.LTEQ);

            if (!checkTopLeft || !checkBottomRight) return false;
            
            CheckedVector = Location;
            return true;

        }
    }
}