using AT2.EndingState;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles
{
    public class Fence
    {
        private readonly ClassId CID;
        private readonly Vector Start, End;

        private Fence(Instruction ReceivedInstruction)
        {
            Start = ReceivedInstruction.GetLocation();
            CID = ReceivedInstruction.GetCID();

            ReceivedInstruction.GetLink().GetExtensionData(out End);

            CalculateFence();
        }
        private void CalculateFence()
        {
            FinalState.AddObstacle(Obstacle.Create(Start, CID));
            FinalState.AddObstacle(Obstacle.Create(End, CID));

            int StepX = Math.Sign(End.GetX() - Start.GetX());
            int StepY = Math.Sign(End.GetY() - Start.GetY());

            bool IsHorizontal = Vector.CompareVectors(Start, End, Vector.ComOperator.EQ, Vector.ComDimension.X);

            int StepControl = IsHorizontal ? StepY : StepX;

            int LoopStart = IsHorizontal ? Start.GetY() : Start.GetX();
            int LoopEnd = IsHorizontal ? End.GetY() : End.GetX();

            for (int i = LoopStart; i != LoopEnd; i += StepControl)
            {
                int X = IsHorizontal ? Start.GetX() : i;
                int Y = IsHorizontal ? i : Start.GetY();

                FinalState.AddObstacle(Obstacle.Create(Vector.CreateVector(X, Y), CID));
            }
        }

        public static Fence CreateFence(Instruction BaseInstruction)
        {
            return new Fence(BaseInstruction);
        }
    }
}
