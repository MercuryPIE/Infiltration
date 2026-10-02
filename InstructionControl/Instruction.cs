using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AT2.ObstacleControl;
using AT2.ObstacleControl.BaseObstacles;
using AT2.Utility;
using static AT2.Utility.Error;

namespace AT2
{
    public class Instruction : InstructionCore
    {
        protected Instruction(Vector Location, ClassId CID) : base(Location, CID)
        {
            CreateExtension(CID);

            if (GetCID() != ClassId.Invalid)
            {
                Instructions.Add(this);
            }
        }

        // -------------------------------------------------

        private static readonly List<Instruction> Instructions = new List<Instruction>();

        public static List<Instruction> AccessInstructionList => Instructions;

        public static void DisplayInstructionList()
        {
            foreach (Instruction I in AccessInstructionList)
            {
                Console.WriteLine(I.ToString());
            }
        }

        // -------------------------------------------------

        public static Instruction CreateInstruction(Vector Location, ClassId CID)
        {
            return new Instruction(Location, CID);
        }

        public static void AddInstruction(ClassId CID)
        {
            while (true)
            {
                string? PotentialLocation = GetConsoleLocation(CID);

                if (Vector.IsLocationValid(PotentialLocation, out Vector ValidLocation))
                {
                    CreateInstruction(ValidLocation, CID);
                    break;
                }
            }
        }

        private static string? GetConsoleLocation(ClassId CID)
        {
            if (CID == ClassId.Fence)
            {
                Console.WriteLine("Enter the location where the fence starts (X,Y):");
            }
            else
            {
                Console.WriteLine($"Enter the {CID}'s location (X,Y):");
            }

            string? Input = Console.ReadLine();
            return Input;
        }

        // -------------------------------------------------

        private void CreateExtension(ClassId CID)
        {
            switch (CID)
            {
                case ClassId.Guard:
                    Link.CreateLink(GetCID(), null);
                    break;

                case ClassId.Fence:
                    GetFenceEnd();
                    break;

                case ClassId.Sensor:
                    GetSensorRange();
                    break;

                case ClassId.Camera:
                    GetCameraDirection();
                    break;

                default:
                    Link.CreateLink(null, null);
                    break;
            }
        }

        protected void GetFenceEnd()
        {
            while (true)
            {
                Console.WriteLine("Enter the location where the fence ends (X,Y):");
                string? Input = Console.ReadLine();

                if (Vector.IsLocationValid(Input, out Vector EndLocation))
                {
                    try
                    {
                        Validate.FenceRange(GetLocation(), EndLocation);
                        AddExtension(this, Link.CreateLink(GetCID(), EndLocation));
                        break;
                    }
                    catch (InvalidRangeException ExM)
                    {
                        Console.WriteLine($"Error: {ExM.Message}");
                    }
                }
            }
        }

        protected void GetSensorRange()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter the sensor's range (in klicks):");
                    string? Input = Console.ReadLine();
                    Validate.SensorRange(Input, out double Range);
                    AddExtension(this, Link.CreateLink(GetCID(), Range));
                    break;
                }
                catch (RangeBelowZeroException ExM)
                {
                    Console.WriteLine($"Error: {ExM.Message}");
                }
                catch (InvalidInputException ExM)
                {
                    Console.WriteLine($"Error: {ExM.Message}");
                }
                catch (EmptyConsoleException ExM)
                {
                    Console.WriteLine($"Error: {ExM.Message}");
                }
            }
        }

        protected void GetCameraDirection()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter the direction the camera is facing (n, s, e or w):");
                    string? Input = Console.ReadLine();
                    Validate.CameraDirection(Input, out char Direction);
                    AddExtension(this, Link.CreateLink(GetCID(), Direction));
                    break;
                }
                catch (InvalidDirectionException ExM)
                {
                    Console.WriteLine($"Error: {ExM.Message}");
                }
                catch (InvalidInputException ExM)
                {
                    Console.WriteLine($"Error: {ExM.Message}");
                }
                catch (EmptyConsoleException ExM)
                {
                    Console.WriteLine($"Error: {ExM.Message}");
                }
            }
        }

        public static void CalculateInstructions(Boundary PassedBoundary)
        {
            foreach (Instruction PassedInstruction in AccessInstructionList)
            {
                switch (PassedInstruction.GetCID())
                {
                    case ClassId.Guard:
                        Guard.CreateGuard(PassedInstruction);
                        break;

                    case ClassId.Fence:
                        Fence.CreateFence(PassedInstruction);
                        break;

                    case ClassId.Sensor:
                        Sensor.CreateSensor(PassedInstruction, PassedBoundary);
                        break;

                    case ClassId.Camera:
                        Camera.CreateCamera(PassedInstruction, PassedBoundary);
                        break;
                }
            }
        }
    }
}
