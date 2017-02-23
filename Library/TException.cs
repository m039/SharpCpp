using System;
namespace SharpCpp
{
    /// <summary>
    /// Not critical exception, an application should handle and recover from it very easy.
    /// </summary>
    public class TException : Exception
    {
        public TException(string message) : base(message)
        {
        }
    }
}
