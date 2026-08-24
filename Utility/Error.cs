using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AT2.Utility
{
    internal class Error
    {
        public class InvalidInputException : Exception
        {
            public InvalidInputException(string message) : base(message) { }
        }
        public class InvalidFormatException : Exception
        {
            public InvalidFormatException(string message) : base(message) { }
        }
        public class EmptyConsoleException : Exception
        {
            public EmptyConsoleException(string message) : base(message) { }
        }
        public class RangeBelowZeroException : Exception
        {
            public RangeBelowZeroException(string Message) : base(Message) { }
        }
        public class InvalidDirectionException : Exception
        {
            public InvalidDirectionException(string Message) : base(Message) { }
        }
        public class InvalidRangeException : Exception
        {
            public InvalidRangeException(string Message) : base(Message) { }
        }
        public class InvalidMapDimensionException : Exception
        {
            public InvalidMapDimensionException(string Message) : base(Message) { }
        }
    }
}
