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
        public int LineCount;
        public AssParserErrorType ErrorType;
        public AssParserException(StreamReader streamReader, int lineCount, AssParserErrorType errorType)
        {
            this.streamReader = streamReader;
            LineCount = lineCount;
            ErrorType = errorType;
        }
        public AssParserException(string message, StreamReader streamReader, int lineCount, AssParserErrorType errorType) : base(message)
        {
            streamReader.DiscardBufferedData();
            this.streamReader = streamReader;
            LineCount = lineCount;
            ErrorType = errorType;
        }
        public AssParserException(string message, Exception inner, StreamReader streamReader, int lineCount, AssParserErrorType errorType) : base(message, inner)
        {
            this.streamReader = streamReader;
            LineCount = lineCount;
            ErrorType = errorType;
        }
        protected AssParserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context, StreamReader streamReader, int lineCount, AssParserErrorType errorType) : base(info, context)
        {
            this.streamReader = streamReader;
            LineCount = lineCount;
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
            streamReader.DiscardBufferedData();
            streamReader.BaseStream.Position = 0;
            int i = 1;
            while (i < LineCount)
            {
                i++;
                _ = streamReader.ReadLine();
            }
            return $"Line {LineCount} : {streamReader.ReadLine()}";
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
