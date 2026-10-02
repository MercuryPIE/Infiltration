namespace AT2.ObstacleControl.BaseObstacles;

public interface IObstacleExtension<T> : IObstacleBase
{
    T ExtensionData { get; set; }
    T GetExtensionData();
    void RemoveExtensionData();
    
}