using AT2.EndingState;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles
{
    public class Camera
    {
        // Boundary Information.
        private readonly Boundary ReceivedBoundary;
        private readonly int BoundaryEdge;

        // Instruction Information.
        private readonly ClassId CID;
        private readonly Vector Location;
        private readonly char CameraDirection;

        // Calculation Information.
        private Vector CurrentLevel;
        private Vector.Range XRange, YRange, LoopControl;

        private Camera(Instruction ReceivedInstruction, Boundary ReceivedBoundary)
        {
            // Takes the Received Boundary and adds it into the field.
            this.ReceivedBoundary = ReceivedBoundary; //<--- this can be removed, if we point to the boundary class.

            // Get the information fom the form the instruction that is passed.
            CID = ReceivedInstruction.GetCID();
            Location = ReceivedInstruction.GetLocation();

            // Access the instruction link to add the information to the field of this class.
            ReceivedInstruction.GetLink().GetExtensionData(out CameraDirection);
            BoundaryEdge = ReceivedBoundary.GetSide(CameraDirection);

            // Get the X & Y from the Instruction.
            int X = ReceivedInstruction.GetLocation().GetX();
            int Y = ReceivedInstruction.GetLocation().GetY();

            // Create XRange & YRange with the stating values
            XRange = Vector.Range.CreateRange(X, X);
            YRange = Vector.Range.CreateRange(Y, Y);

            // Call the last information for this constructor to function.
            CurrentLevel = ReceivedInstruction.GetLocation();
            LoopControl = Vector.Range.CreateRange(0, 0);

            CalculateCamera();
        }

        // -------------------------------------------------

        /// <summary>
        /// Calculates the camera based on the information from the Instruction, Boundary and the calculated Ranges.
        /// </summary>
        private void CalculateCamera()
        {
            // This is the main control of the entire Camera calculation methods. (Checks if the Camera is North or South)
            bool IsNorthOrSouth = CameraDirection is 'n' or 's';

            // Add the starting location into the FinalState List.
            FinalState.AddObstacle(Obstacle.Create(Location, CID));

            // The Starting point is where the camera is, If North Or South is True we only use the Y Value of the location,
            // else if it's an East Or West Camera we only need the X.
            int StartPoint = IsNorthOrSouth ? Location.GetY() : Location.GetX();

            // Find the Max/Min between the Starting Point & the Boundary Edge, this will give use the loop start (Min)
            // & the loop end (Max).
            int Min = Math.Min(StartPoint, BoundaryEdge);
            int Max = Math.Max(StartPoint, BoundaryEdge);

            // The Min will be the start of the loop and the Max is the End of the loop.
            Vector.Range Limit = Vector.Range.CreateRange(Min, Max);

            // Call the method with the loop Limits and the Direction Condition Check.
            BuildCamera(Limit, IsNorthOrSouth);
        }

        /// <summary>
        /// The Main calculation method for the Camera class.
        /// </summary>
        /// <param name="IsNorthOrSouth">This Condition is used to check things in this method.</param>
        private void ControlCalculation(bool IsNorthOrSouth)
        {
            /*So the calculation is a combination of 4 vector or vector like objects:

            CurrentLevel(Vector): 
            When using CL depending on the INOS(IsNorthOrSouth) Condition only one side is used,
            In the case of the direction being North the CL is -1 so it moves by -1 on the Y axis, 
            so when it's north or south we only need to look at the Y of the CL.
            
            XRange & YRange(Vector.Range):
            XR and YR are both calculated, however they aren't both used in the same way as CL, 
            If INOS is true we only use XR because the pointer goes from left to right, on the flip of INOS
            we check the YR because we calculate the top to bottom of the column
            
            LoopControl(Vector.Range):
            LC is the main controller of the inner loop in the calculation method this will run from Left to Right 
            if INOS is true and Top to Bottom if INOS is false*/

            int CurrentX = CameraDirection == 'w' ? CurrentLevel.GetX() - 1 : CurrentLevel.GetX() + 1;
            int CurrentY = CameraDirection == 'n' ? CurrentLevel.GetY() - 1 : CurrentLevel.GetY() + 1;
            CurrentLevel = Vector.UpdateVector(CurrentX, CurrentY);

            int XRange_MinX = IsNorthOrSouth ? XRange.GetMin() - 1 : XRange.GetMin();
            int XRange_MaxX = IsNorthOrSouth ? XRange.GetMax() + 1 : XRange.GetMax();
            XRange = Vector.Range.UpdateRange(XRange_MinX, XRange_MaxX);

            int YRange_MinY = IsNorthOrSouth ? YRange.GetMin() : YRange.GetMin() - 1;
            int YRange_MaxY = IsNorthOrSouth ? YRange.GetMax() : YRange.GetMax() + 1;
            YRange = Vector.Range.UpdateRange(YRange_MinY, YRange_MaxY);

            int Start = IsNorthOrSouth ? XRange.GetMin() : YRange.GetMin();
            int End = IsNorthOrSouth ? XRange.GetMax() : YRange.GetMax();
            LoopControl = Vector.Range.UpdateRange(Start, End);
        }

        /// <summary>
        /// Builds the Camera Row by Row. 
        /// </summary>
        /// <param name="Limit">This is the Outer loop control.</param>
        /// <param name="IsNorthOrSouth">This is used for the calculations for the ranges.</param>
        private void BuildCamera(Vector.Range Limit, bool IsNorthOrSouth)
        {
            // Outer Loop.
            for (int i = Limit.GetMin(); i <= Limit.GetMax(); i++)
            {
                // Calculate the new ranges based on the bool condition.
                ControlCalculation(IsNorthOrSouth);

                // Get the Value based on the condition with the location.
                int Cord = IsNorthOrSouth ? CurrentLevel.GetY() : CurrentLevel.GetX();

                // Inner Loop. 
                for (int L = LoopControl.GetMin(); L <= LoopControl.GetMax(); L++)
                {
                    // Get the X & Y based on the condition.
                    // If INOS is true it means that X is the calculated location so Y would be whatever the CurrentLevel Y is.
                    int X = IsNorthOrSouth ? L : Cord;
                    int Y = IsNorthOrSouth ? Cord : L;

                    // Create a new Vector from the X & Y.
                    Vector NewLocation = Vector.CreateVector(X, Y);

                    // Check if the new calculated location is within the boundary or not.
                    if (Boundary.IsInBoundary(NewLocation, ReceivedBoundary, out Vector CalculatedLocation))
                    {
                        // Add location to the FinalState.
                        FinalState.AddObstacle(Obstacle.Create(CalculatedLocation, CID));
                    }
                }
            }
        }

        /// <summary>
        /// Public access to the camera class.
        /// </summary>
        /// <param name="BaseInstruction">This is the Instruction, the CID will be camera</param>
        /// <param name="BaseBoundary">This is the Boundary that is calculated</param>
        /// <returns></returns>
        public static Camera CreateCamera(Instruction BaseInstruction, Boundary BaseBoundary)
        {
            return new Camera(BaseInstruction, BaseBoundary);
        }
    }
}