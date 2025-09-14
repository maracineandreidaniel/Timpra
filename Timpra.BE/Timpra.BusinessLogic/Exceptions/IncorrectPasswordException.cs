using System;

namespace Timpra.BusinessLogic.Exceptions
{
    public class IncorrectPasswordException : Exception
    {
        public IncorrectPasswordException(string message)
            : base(message)
        {
        }
    }
}