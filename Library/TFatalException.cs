using System;
namespace SharpCpp
{
    /// <summary>
    /// This exception could be thrown in an initialization phase of the library. 
    /// Recommendation is not to handle it, instead, let an application crash, it should help locate the problem.
    /// </summary>
    public class TFatalException : Exception
    {
        public TFatalException(string message) : base(message)
        {
        }
    }
}
