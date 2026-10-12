using AT2.InstructionControl;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles;

public interface IObstacleBase
{
    public Vector Location { get; set; }
    public void CalculateObstacle();
    public ClassId ObstacleClass { get; }


}