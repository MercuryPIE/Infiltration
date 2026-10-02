using AT2.Utility;

namespace AT2.ObstacleControl
{
    public interface IDataContainer
    {
        object GetData();
    }
    public enum LinkType
    {
        GuardLink,
        FenceLink,
        SensorLink,
        CameraLink,
        InvalidLink
    }

    public class Link
    {
        private readonly struct FenceEnd : IDataContainer
        {
            private readonly Vector Data;
            private FenceEnd(Vector Data)
            {
                this.Data = Data;
            }

            public static FenceEnd CreateData(object? Data)
            {
                if (Data is Vector convertedData)
                {
                    return new FenceEnd(convertedData);
                }
                throw new ArgumentException("Invalid data type for FenceEnd: Must be Vector.");
            }

            object IDataContainer.GetData() { return Data; }
        }

        private readonly struct SensorRange : IDataContainer
        {
            private readonly double Data;
            private SensorRange(double Data)
            {
                this.Data = Data;
            }

            public static SensorRange CreateData(object? Data)
            {
                if (Data is double ConvertedData)
                {
                    return new SensorRange(ConvertedData);
                }
                throw new ArgumentException("Invalid data type for SensorRange: Must be double.");
            }

            object IDataContainer.GetData() { return Data; }
        }

        private readonly struct CameraDirection : IDataContainer
        {
            private readonly char Data;
            private CameraDirection(char Data)
            {
                this.Data = Data;
            }

            public static CameraDirection CreateData(object? Data)
            {
                if (Data is char ConvertedData)
                {
                    return new CameraDirection(ConvertedData);
                }
                throw new ArgumentException("Invalid data type for CameraDirection: Must be char.");
            }

            object IDataContainer.GetData() { return Data; }
        }


        private readonly LinkType Type;
        private readonly IDataContainer? LinkedData;

        private Link(LinkType Type, IDataContainer? LinkedData)
        {
            this.Type = Type;
            this.LinkedData = LinkedData;
        }

        public LinkType GetLinkType() { return Type; }

        public static Link CreateLink(ClassId? CID, object? Data)
        {
            switch (CID)
            {
                case ClassId.Guard:
                    return new Link(LinkType.GuardLink, null);

                case ClassId.Fence:
                    return new Link(LinkType.FenceLink, FenceEnd.CreateData(Data));

                case ClassId.Sensor:
                    return new Link(LinkType.SensorLink, SensorRange.CreateData(Data));

                case ClassId.Camera:
                    return new Link(LinkType.CameraLink, CameraDirection.CreateData(Data));

                default:
                    return new Link(LinkType.InvalidLink, null);
            }
        }

        public void GetExtensionData<T>(out T Data)
        {
            if (typeof(T) == typeof(Vector) && LinkedData is FenceEnd || 
                typeof(T) == typeof(double) && LinkedData is SensorRange || 
                typeof(T) == typeof(char) && LinkedData is CameraDirection)
            {
                Data = (T)LinkedData.GetData();
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
