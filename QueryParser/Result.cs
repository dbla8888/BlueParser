using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryParser
{
    /// <summary>
    /// Represents the result of a operation which may be expected to fail.
    /// </summary>
    /// <typeparam name="T">The Type of the returned value of the operation</typeparam>
    public interface IResult<out T>
    {
        /// <summary>
        /// Indicates whether or not the operation was successfull
        /// </summary>
        bool IsSuccess { get; }
        /// <summary>
        /// The value returned from the operation, if successful.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when attempting 
        /// to retrieve the value of an unsuccessful operation</exception>
        T Value { get; }
        /// <summary>
        /// Contains exception thrown by failing operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when attempting 
        /// to retrieve the exceptions of an successful operation</exception>
        Exception Exception { get; }
    }

    internal class Result<T> : IResult<T>
    {
        private T value;
        private Exception exception;

        public bool IsSuccess { get; }

        public T Value
        {
            get
            {
                if (this.IsSuccess)
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

        public Exception Exception
        {
            get
            {
                if (this.IsSuccess)
                {
                    throw new InvalidOperationException("Cannot to retrieve exception of successful parse.");
                }
                else
                {
                    return this.exception;
                }
            }
            private set
            {
                this.exception = value;
            }
        }

        public Result(T value)
        {
            this.IsSuccess = true;
            this.value = value;
        }

        public Result(Exception exception)
        {
            this.IsSuccess = false;
            this.exception = exception;
        }

        public override string ToString()
        {
            return this.IsSuccess ?
                $"{nameof(IsSuccess)}:{this.IsSuccess} {nameof(this.Value)}:{this.Value}" :
                $"{nameof(IsSuccess)}:{this.IsSuccess} {nameof(this.Exception)}:{String.Join("\r\n", this.exception.Message)}";
        }

        public static Result<T> GetResult(Func<T> operation)
        {
            try
            {
                return new Result<T>(operation());
            }
            catch (Exception ex)
            {
                return new Result<T>(ex);
            }
        }
    }
}
