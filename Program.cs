using AT2.EndingState;
using AT2.InstructionControl;
using AT2.ObstacleControl.BaseObstacles;

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
            Invalid = 'i'
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
            
            return optionCheck ? (Options)optionSelected : Options.Invalid;
        }

        private static void RunOption(Options OptionPicked)
        {
            switch (OptionPicked)
            {
                case Options.AddGuard:
                    Instruction.AddInstruction(ClassId.Guard);
                    break;

                case Options.AddFence:
                    Instruction.AddInstruction(ClassId.Fence);
                    break;

                case Options.AddSensor:
                    Instruction.AddInstruction(ClassId.Sensor);
                    break;

                case Options.AddCamera:
                    Instruction.AddInstruction(ClassId.Camera);
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
                    Node.FindSafePath();
                    FinalState.Wipe();
                    break;

                case Options.Invalid:
                    Instruction.DisplayInstructionList();
                    break;
            }
        }
    }
}