using System;

namespace Timpra.BusinessLogic.Exceptions
{
    public class InsufficientMoneyException : Exception
    {
        public InsufficientMoneyException(string message)
            : base(message)
        {
        }
    }
}
