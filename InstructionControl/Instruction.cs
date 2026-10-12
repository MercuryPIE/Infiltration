using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AT2.InstructionControl;
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
            

            if (GetCID() != ClassId.Guard)
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
        }
}
