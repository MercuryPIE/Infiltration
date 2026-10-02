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
        private struct BoundaryCore
        {
            
        }
        
        
        
        
        /// <summary>
        /// Represents a set of Vectors associated with cardinal directions.
        /// </summary>
        private struct LinkSet
        {
            private static Vector _north = new(0, 0, Vector.VectorState.Empty);
            private static Vector _south = new(0, 0, Vector.VectorState.Empty);
            private static Vector _east = new(0, 0, Vector.VectorState.Empty);
            private static Vector _west = new(0, 0, Vector.VectorState.Empty);

            /// <summary>
            /// Private constructor for initialising the LinkSet with the specified starting Vector.
            /// </summary>
            /// <param name="StartSet">The starting Vector for the LinkSet.</param>
            private LinkSet(Vector StartSet)
            {
                _north = StartSet;
                _south = StartSet;
                _east = StartSet;
                _west = StartSet;
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
                Dictionary<Direction,Vector> directionIndex = new Dictionary<Direction, Vector>
                {
                    {Direction.North, _north},
                    {Direction.South, _south},
                    {Direction.East, _east},
                    {Direction.West, _west},
                };

                return directionIndex[Key];
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
                        _north = NewLocation;
                        break;

                    case Direction.South:
                        _south = NewLocation;
                        break;

                    case Direction.East:
                        _east = NewLocation;
                        break;

                    case Direction.West:
                        _west = NewLocation;
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

        private const int Padding = 5;
        private readonly Vector _topLeftBound, _bottomRightBound;

        private Boundary(Vector TopLeftBound, Vector BottomRightBound)
        {
            _topLeftBound = TopLeftBound;
            _bottomRightBound = BottomRightBound;
        }

        // -------------------------------------------------
        public Vector GetCorner(Corner Request)
        {
            return Request switch
            {
                Corner.TopLeft => _topLeftBound,
                Corner.BottomRight => _bottomRightBound,
                _ => throw new ArgumentOutOfRangeException(nameof(Request)),
            };
        }
        public int GetSide(char Request)
        {
            var charToDirection = new Dictionary<char, int>
            {
                {'n', _topLeftBound.Y},
                {'s', _bottomRightBound.Y},
                {'e', _bottomRightBound.X},
                {'w', _topLeftBound.X},
            };

            return charToDirection[Request];
        }

        // -------------------------------------------------


        private static Boundary _currentBoundary;

        public void BuildBoundary(params Vector[] InjectedLocations)
        {
            var locationList = Instruction.AccessInstructionList;
            var firstLocation = locationList.Count == 0 ? new Vector(0, 0, Vector.VectorState.Empty) : locationList[0].GetLocation();
            
            LinkSet.Create(firstLocation);

            if (InjectedLocations.Any())
            {
                foreach (var loc in InjectedLocations)
                {
                    CheckCoreSet(loc);
                }
            }

            foreach (var loc in locationList)
            {
                CheckCoreSet(loc.GetLocation());

                if (loc.GetCID() == ClassId.Fence)
                {
                    //TODO: change the code for new system for getting fence data
                }
            }

            _currentBoundary = new Boundary(GetBoundaryCorner(Corner.TopLeft), GetBoundaryCorner(Corner.BottomRight));
        }
        
        
        
        private static Vector GetBoundaryCorner(Corner CornerName)
        {
            // Get the Direction of X and Y (DX, DY).
            var horizontalDirection = CornerName == Corner.TopLeft ? Direction.West : Direction.East;
            var verticalDirection = CornerName == Corner.TopLeft ? Direction.North : Direction.South;

            // Using that Direction, we get the vectors of that corner.
            var horVec = LinkSet.Get(horizontalDirection);
            var verVec = LinkSet.Get(verticalDirection);

            // Calculate the corner with -/+ of padding.
            var rx = horVec.X + (CornerName == Corner.TopLeft ? -Padding : +Padding);
            var ry = verVec.Y + (CornerName == Corner.TopLeft ? -Padding : +Padding);

            return new Vector(rx, ry);
        }
        // -------------------------------------------------

        public static Boundary Calculate(params Vector[] InjectedLocations)
        {
            var start = Instruction.AccessInstructionList.Count == 0
                ? new Vector(0, 0, Vector.VectorState.Empty)
                : Instruction.AccessInstructionList[0].GetLocation();

            LinkSet.Create(start);

            if (InjectedLocations.Any())
            {
                foreach (Vector V in InjectedLocations)
                {
                    CheckCoreSet(V);
                }
            }

            foreach (var pulledInstruction in Instruction.AccessInstructionList)
            {
                Vector location = pulledInstruction.GetLocation();
                CheckCoreSet(location);

                if (pulledInstruction.GetCID() == ClassId.Fence)
                {
                    pulledInstruction.GetLink().GetExtensionData(out Vector linkLocation);
                    CheckCoreSet(linkLocation);
                }
            }
            return new Boundary(GetBoundaryCorner(Corner.TopLeft), GetBoundaryCorner(Corner.BottomRight));
        }

        private static void CheckCoreSet(Vector UpdatedLocation)
        {
            // For each direction (NSEW) preform a comparison to update the NSEW values.
            foreach (Direction linkDirection in Enum.GetValues(typeof(Direction)))
            {
                // Grab the stored location, from the link set.
                var storedLocation = LinkSet.Get(linkDirection);

                // This is the main comparison of the direction:
                /*
                IsNorthOrSouth is used to determine the Comparison axis (X,Y). So if the direction is North or South
                This condition is true else false.

                IsSouthOrEast is used to determine the Comparison operator (<, >). so if the direction is South or East
                This condition is true else false
                 */
                var inos = linkDirection is Direction.North or Direction.South; // For the ComDim
                var isoe = linkDirection is Direction.South or Direction.East; // For the ComOPs

                // Using the value above from INOS we can load the ComDimension into ComDim. In the case of INOS being true
                // the ComDim would be Y else it would be X
                var comDim = inos ? Vector.ComDimension.Y : Vector.ComDimension.X;

                // Using the value above from ISOE, we can load the ComOperator into ComOpr. In the case of ISOE being true
                // the ComOpr would be Greater than else it would be less then.
                var comOpr = isoe ? Vector.ComOperator.Gt : Vector.ComOperator.Lt;

                // Checks is the stored location is less than or greater than the updated location on the X or Y,
                // this is determined by ISOE & INOS.
                if (Vector.CompareVectors(UpdatedLocation, storedLocation, comOpr, comDim))
                {
                    LinkSet.Set(linkDirection, UpdatedLocation);
                }
            }
        }

        public static Boundary SetBoundary(Vector Tl, Vector Br)
        {
            return new Boundary(Tl, Br);
        }

        public static bool IsInBoundary(Vector Location, out Vector CheckedVector)
        {
            CheckedVector = new Vector(0, 0, Vector.VectorState.Empty);

            Vector topLeft =  _currentBoundary.GetCorner(Corner.TopLeft);
            Vector bottomRight = _currentBoundary.GetCorner(Corner.BottomRight);

            bool checkTopLeft = Vector.CompareVectors(Location, topLeft, Vector.ComOperator.Gteq);
            bool checkBottomRight = Vector.CompareVectors(Location, bottomRight, Vector.ComOperator.Lteq);

            if (!checkTopLeft || !checkBottomRight) return false;
            
            CheckedVector = Location;
            return true;

        }
    }
}