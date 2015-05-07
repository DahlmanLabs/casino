using System;

namespace LocalCasino.Common.Exceptions
{
    public class CasinoException : Exception
    {
        public CasinoException(string message)
            : base(message)
        {
        }
    }
}