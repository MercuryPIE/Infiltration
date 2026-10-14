using AT2.EndingState;
using AT2.ObstacleControl;
using AT2.ObstacleControl.BaseObstacles;
using Path = AT2.EndingState.Path;

namespace AT2
{
    internal class Program
    {
        private enum Options
        {
            AddGuard = 'g',
            AddFence = 'f',
            AddSensor = 's',
            AddCamera = 'c',
            ShowSafeDirection = 'd',
            DisplayMap = 'm',
            FindPath = 'p',
            Exit = 'x',
        }

        static void Main(string[] args)
        {
            while (true)
            {
                WriteOptions();
                var selectedOption = GetConsoleOptions();

                if (selectedOption == Options.Exit)
                {
                    break;
                }

                RunOption(selectedOption);
            }
        }

        private static void WriteOptions()
        {
            Console.WriteLine("Select one of the following options");
            Console.WriteLine("g) Add 'Guard' obstacle");
            Console.WriteLine("f) Add 'Fence' obstacle");
            Console.WriteLine("s) Add 'Sensor' obstacle");
            Console.WriteLine("c) Add 'Camera' obstacle");
            Console.WriteLine("d) Show safe directions");
            Console.WriteLine("m) Display obstacle map");
            Console.WriteLine("p) Find safe path");
            Console.WriteLine("x) Exit");
            Console.WriteLine("Enter code:");
        }

        private static Options GetConsoleOptions()
        {
            var consoleInput = Console.ReadLine();
            
            // Try to parse the input as char and check if the char is defined in the enum of options.
            var optionCheck = char.TryParse(consoleInput, out var optionSelected) && Enum.IsDefined(typeof(Options), (int)optionSelected);
            
            return optionCheck ? (Options)optionSelected : throw new ArgumentOutOfRangeException();
        }

        private static void RunOption(Options OptionPicked)
        {
            switch (OptionPicked)
            {
                case Options.AddGuard:
                    ObstacleFactory.AddObstacle(ObstacleId.Guard);
                    break;
                case Options.AddFence:
                    ObstacleFactory.AddObstacle(ObstacleId.Fence);
                    break;
                case Options.AddSensor:
                    ObstacleFactory.AddObstacle(ObstacleId.Sensor);
                    break;
                case Options.AddCamera:
                    ObstacleFactory.AddObstacle(ObstacleId.Camera);
                    break;
                case Options.ShowSafeDirection:
                    Show.SafeDirections();
                    FinalState.Wipe();
                    break;
                case Options.DisplayMap:
                    Map.CreateMap();
                    FinalState.Wipe();
                    break;
                case Options.FindPath:
                    // Path.FindSafePath();
                    FinalState.Wipe();
                    break;
                default:
                    WriteOptions();
                    break;
            }
        }
    }
}