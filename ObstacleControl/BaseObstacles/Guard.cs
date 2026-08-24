using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AT2.EndingState;
using AT2.ObstacleControl;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles
{
    public class Guard
    {
        private readonly Vector Location;
        private readonly ClassId CID;

        private Guard(Instruction ReceivedInstruction)
        {
            Location = ReceivedInstruction.GetLocation();
            CID = ReceivedInstruction.GetCID();

            CalculateGuard();
        }

        private void CalculateGuard()
        {
            FinalState.AddObstacle(Obstacle.Create(Location, CID));
        }
        public static Guard CreateGuard(Instruction BaseInstruction)
        {
            return new Guard(BaseInstruction);
        }
    }
}
