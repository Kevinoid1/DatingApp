using System;

namespace DatingApp
{
    public class MyCoolException : Exception
    {
        private readonly int code;
        public MyCoolException(int code, string message) : base(message)  
        {
            this.code = code;
        }

        public int Baba { get { return this.code; } }
    }
}