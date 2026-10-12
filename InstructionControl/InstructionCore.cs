using System.Text;
using AT2.ObstacleControl;
using AT2.Utility;

namespace AT2.InstructionControl
{
    public enum ClassId
    {
        Guard = 'g',
        Fence = 'f',
        Sensor = 's',
        Camera = 'c',
    }

    public abstract class InstructionCore
    {
        private readonly Vector Location;
        private readonly ClassId CID;


        protected InstructionCore(Vector Location, ClassId CID)
        {
            this.Location = Location;
            this.CID = CID;

        }

        // -------------------------------------------------

        public Vector GetLocation() { return Location; }
        public ClassId GetCID() { return CID; }


        // -------------------------------------------------
        

        public override string ToString()
        {
            ClassId CID = GetCID();

            StringBuilder ObstacleString = new StringBuilder();
            
            if (CID == ClassId.Guard)
            {
                ObstacleString.AppendLine($"If You See this, know you weren't meant too");
            }
            else
            {
                ObstacleString.AppendLine($"Location: {GetLocation()}");
                ObstacleString.AppendLine($"Class: {GetCID()}");
            }
            return ObstacleString.ToString();
        }
    }
}