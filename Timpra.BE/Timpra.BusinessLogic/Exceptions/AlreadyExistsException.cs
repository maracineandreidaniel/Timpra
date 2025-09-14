using System;

namespace Timpra.BusinessLogic.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}
