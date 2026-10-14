namespace AT2.ObstacleControl;

public interface IObstacleExtension<T> : IObstacleBase
{
    T ExtensionData { get; set; }
    T GetExtensionData();
    T SetExtensionData();
}