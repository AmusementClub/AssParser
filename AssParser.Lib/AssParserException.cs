using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssParser.Lib
{
    [Serializable]
    public class AssParserException : Exception
    {
        public StreamReader streamReader;
        public long Position;
        public AssParserErrorType ErrorType;
        public AssParserException(StreamReader streamReader, AssParserErrorType errorType)
        {
            this.streamReader = streamReader;
            Position = streamReader.BaseStream.Position;
            ErrorType = errorType;
        }
        public AssParserException(string message, StreamReader streamReader, AssParserErrorType errorType) : base(message)
        {
            this.streamReader = streamReader;
            Position = streamReader.BaseStream.Position;
            ErrorType = errorType;
        }
        public AssParserException(string message, Exception inner, StreamReader streamReader, AssParserErrorType errorType) : base(message, inner)
        {
            this.streamReader = streamReader;
            Position = streamReader.BaseStream.Position;
            ErrorType = errorType;
        }
        protected AssParserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context, StreamReader streamReader, AssParserErrorType errorType) : base(info, context)
        {
            this.streamReader = streamReader;
            Position = streamReader.BaseStream.Position;
            ErrorType = errorType;
        }
        /// <summary>
        /// Print readable exception in English.
        /// </summary>
        /// <returns>Exception message and line content.</returns>
        public override string ToString()
        {
            string? Line = PrintErrorLine();
            return $"{base.Message}{Environment.NewLine}{Line}";
        }
        /// <summary>
        /// Print the line where exception occurs.
        /// </summary>
        /// <returns>Line number and the content of the line.The format is "Line XX : ********".</returns>
        public string? PrintErrorLine()
        {
            streamReader.BaseStream.Position = 0;
            int LineCount = 0;
            string? Line = "";
            while (streamReader.BaseStream.Position < Position)
            {
                LineCount++;
                Line = streamReader.ReadLine();
            }
            streamReader.BaseStream.Position = Position;
            return $"Line {LineCount} : {Line}";
        }
    }
    public enum AssParserErrorType
    {
        UnknownError,
        InvalidSection,
        MissingFormatLine,
        InvalidStyleLine,
        InvalidStyle,
        InvalidEvent,
    }
}
