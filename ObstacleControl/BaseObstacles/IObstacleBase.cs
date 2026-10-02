using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles;

public interface IObstacleBase
{
    public Vector Location { get; set; }
    public void CalculateObstacle();
    

}