using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AT2.EndingState;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles
{
    public class Sensor
    {
        private readonly Boundary ReceivedBoundary;
        private readonly ClassId CID;
        private readonly Vector Location;
        private readonly double Range;

        private Sensor(Instruction ReceivedInstruction, Boundary ReceivedBoundary)
        {
            this.ReceivedBoundary = ReceivedBoundary;

            CID = ReceivedInstruction.GetCID();
            Location = ReceivedInstruction.GetLocation();
            ReceivedInstruction.GetLink().GetExtensionData(out Range);

            CalculateSensor();
        }

        private void CalculateSensor()
        {
            double X = Convert.ToDouble(Location.GetX());
            double Y = Convert.ToDouble(Location.GetY());

            double XRange = X + Range;
            double YRange = Y + Range;

            int StartX = Convert.ToInt32(Math.Floor(X - Range));
            int StartY = Convert.ToInt32(Math.Floor(Y - Range));

            for (int XPos = StartX; XPos <= XRange; XPos++)
            {
                for (int YPos = StartY; YPos <= YRange; YPos++)
                {
                    double XPower = Math.Pow(XPos - X, 2);
                    double YPower = Math.Pow(YPos - Y, 2);

                    double Distance = Math.Sqrt(XPower + YPower);

                    Vector NewLocation = Vector.CreateVector(XPos, YPos);

                    if (Distance <= Range && Boundary.IsInBoundary(NewLocation, ReceivedBoundary, out Vector CheckedLocation))
                    {
                        FinalState.AddObstacle(Obstacle.Create(CheckedLocation, CID));
                    }
                }
            }
        }

        public static Sensor CreateSensor(Instruction BaseInstruction, Boundary BaseBoundary)
        {
            return new Sensor(BaseInstruction, BaseBoundary);
        }
    }
}
