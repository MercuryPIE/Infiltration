using AT2.ObstacleControl.BaseObstacles;
using AT2.Utility;

namespace AT2.ObstacleControl;

public interface IObstacleBase
{
    public Vector Location { get; set; }
    public ObstacleId ObstacleClass { get; }
    public void CalculateObstacle();
    


}