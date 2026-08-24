using System.Text;
using AT2.ObstacleControl;
using AT2.Utility;

namespace AT2
{
    public enum ClassId
    {
        Guard = 'g',
        Fence = 'f',
        Sensor = 's',
        Camera = 'c',
        Invalid = 'i'
    }

    public abstract class InstructionCore
    {
        private readonly Vector Location;
        private readonly ClassId CID;
        private Link OLink;

        protected InstructionCore(Vector Location, ClassId CID)
        {
            this.Location = Location;
            this.CID = CID;
            OLink = Link.CreateLink(null, null);
        }

        // -------------------------------------------------

        public Vector GetLocation() { return Location; }
        public ClassId GetCID() { return CID; }
        public Link GetLink() { return OLink; }


        // -------------------------------------------------

        public static void AddExtension(Instruction I, Link L)
        {
            I.OLink = L;
        }

        public override string ToString()
        {
            ClassId CID = GetCID();

            StringBuilder ObstacleString = new StringBuilder();
            
            if (CID == ClassId.Invalid)
            {
                ObstacleString.AppendLine($"If You See this, know you weren't meant too");
            }
            else
            {
                ObstacleString.AppendLine($"Location: {GetLocation()}");
                ObstacleString.AppendLine($"Class: {GetCID()}");

                switch (GetCID())
                {
                    case ClassId.Fence:
                        GetLink().GetExtensionData(out Vector FenceLocation);
                        ObstacleString.AppendLine($"Link: {FenceLocation}");
                        break;

                    case ClassId.Sensor:
                        GetLink().GetExtensionData(out double SensorRange);
                        ObstacleString.AppendLine($"Link: {SensorRange}");
                        break;

                    case ClassId.Camera:
                        GetLink().GetExtensionData(out char CameraDirection);
                        ObstacleString.AppendLine($"Link: {CameraDirection}");
                        break;

                    default:
                        ObstacleString.AppendLine("This Obstacle Has No Link."); 
                        break;
                }
            }
            return ObstacleString.ToString();
        }
    }
}