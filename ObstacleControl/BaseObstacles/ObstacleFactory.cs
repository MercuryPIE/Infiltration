using AT2.InstructionControl;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles;

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

    public static void AddObstacle(ClassId ObstacleId)
    {
        while (true)
        {
            var consoleLocation = GetConsoleLocation(ObstacleId);

            if (!Vector.IsLocationValid(consoleLocation, out var validVector)) continue;
            
            IObstacleBase? obstacle = ObstacleId switch
            {
                ClassId.Guard => new Guard(validVector),
                ClassId.Fence => new Fence(validVector),
                ClassId.Sensor => new Sensor(validVector),
                ClassId.Camera => new Camera(validVector),
                _ => null
            };

            if (obstacle == null) continue;
            
            Obstacles.Add(obstacle);
            break;
        }
    }
    
    private static string? GetConsoleLocation(ClassId ObstacleId)
    {
        var printTxt = ObstacleId == ClassId.Fence
            ? "Enter the location where the fence starts (X,Y):"
            : $"Enter the {ObstacleId}'s location (X,Y):";
        
        Console.Write(printTxt); 
        return Console.ReadLine();
    }
}