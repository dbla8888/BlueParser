using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueParser
{
    /// <summary>
    /// Represents the result of a parsing attempt.
    /// </summary>
    /// <typeparam name="T">The Type of the value parsed from the input, 
    /// if parsing is successful</typeparam>
    public interface IParseResult<out T>
    {     
        /// <summary>
        /// Indicates whether or not the input was successfully parsed
        /// </summary>
        bool IsSuccess { get; }
        /// <summary>
        /// The value returned from the parse attempt, if successful.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when attempting 
        /// to retrieve the value of an unsuccessful parse</exception>
        T Value { get; }
        /// <summary>
        /// Contains any thrown exceptions if parsing fails.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when attempting 
        /// to retrieve the exceptions of an successful parse</exception>
        IEnumerable<Exception> Exceptions { get; }
    }

    internal class ParseResult<T> : IParseResult<T>
    {
        private T value;
        private IEnumerable<Exception> exceptions;

        public bool IsSuccess { get; }

        public T Value {
            get
            {
                if(this.IsSuccess)
                {
                    return this.value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot to retrieve value of failed parse.");
                }
            }
            private set
            {
                this.value = value;
            }
        }

        public IEnumerable<Exception> Exceptions
        {
            get
            {
                if (this.IsSuccess)
                {
                    throw new InvalidOperationException("Cannot to retrieve exception of successful parse.");
                }
                else
                {
                    return this.exceptions;
                }
            }
            private set
            {
                this.exceptions = value;
            }
        }

        public ParseResult(T value)
        {
            this.IsSuccess = true;
            this.value = value;
        }

        public ParseResult(Exception exception)
        {
            this.IsSuccess = false;
            this.exceptions = new[] { exception };           
        }

        public ParseResult(IEnumerable<Exception> exceptions)
        {
            this.IsSuccess = false;
            this.exceptions = exceptions;
        }

        public override string ToString()
        {
            return this.IsSuccess ?
                $"{nameof(IsSuccess)}:{this.IsSuccess} {nameof(this.Value)}:{this.Value}" :
                $"{nameof(IsSuccess)}:{this.IsSuccess} {nameof(this.Exceptions)}:{String.Join("\r\n", this.exceptions.Select(x => x.Message))}";
        }

        public static ParseResult<T> GetResult(Func<T> operation)
        {
            try
            {
                return new ParseResult<T>(operation());
            }catch(Exception ex)
            {
                return new ParseResult<T>(ex);
            }
        }
    }
}
