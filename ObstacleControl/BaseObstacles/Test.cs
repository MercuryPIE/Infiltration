using AT2.EndingState;

namespace AT2.ObstacleControl.BaseObstacles;

public class Test
{
    public static void Build()
    {
        ObstacleFactory.AddObstacle(ObstacleId.Camera);
        // ObstacleFactory.AddObstacle(ObstacleId.Camera);
        // Boundary.BuildBoundary();
        // ObstacleFactory.Build();
        
        Map.CreateMap();


        
    }
}