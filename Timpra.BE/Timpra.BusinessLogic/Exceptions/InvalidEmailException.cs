using System;

namespace Timpra.BusinessLogic.Exceptions
{
    public class InvalidEmailException : Exception
    {
        public InvalidEmailException(string message)
            : base(message)
        {
        }
    }
}
