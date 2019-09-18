using System;

namespace BlueParser
{
    public class ParseException : Exception
    {
        public int Line { get; }
        public int Position { get; }

        public ParseException(int line, int position, string message) : base(message)
        {
            this.Line = line;
            this.Position = position;
        }
    }
}
