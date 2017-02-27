using System;
namespace SharpCpp
{
    public class TUnsupportedException : TException
    {
        public TUnsupportedException() : base("Unsupported")
        {
        }
    }
}
