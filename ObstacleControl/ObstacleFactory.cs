using AT2.ObstacleControl.BaseObstacles;
using AT2.Utility;

namespace AT2.ObstacleControl;

public static class ObstacleFactory
{
    public static readonly List<IObstacleBase> Obstacles = new();
    
    public static void Build()
    {
        foreach (var obstacle in Obstacles)
        {
            obstacle.CalculateObstacle();
        }
    }

    public static void AddObstacle(ObstacleId OId)
    {
        while (true)
        {
            var consoleLocation = GetConsoleLocation(OId);

            if (!Vector.IsLocationValid(consoleLocation, out var validVector)) continue;
            
            IObstacleBase? obstacle = OId switch
            {
                ObstacleId.Guard => new Guard(validVector),
                ObstacleId.Fence => new Fence(validVector),
                ObstacleId.Sensor => new Sensor(validVector),
                ObstacleId.Camera => new Camera(validVector),
                _ => null
            };

            if (obstacle == null) continue;
            
            Obstacles.Add(obstacle);
            break;
        }
    }
    
    private static string? GetConsoleLocation(ObstacleId ObstacleId)
    {
        var printTxt = ObstacleId == ObstacleId.Fence
            ? "Enter the location where the fence starts (X,Y):"
            : $"Enter the {ObstacleId}'s location (X,Y):";
        
        Console.Write(printTxt); 
        return Console.ReadLine();
    }
}