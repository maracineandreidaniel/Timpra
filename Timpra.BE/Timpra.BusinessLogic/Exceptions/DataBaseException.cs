using System;

namespace Timpra.BusinessLogic.Exceptions
{
    public class DataBaseException : Exception
    {
        public DataBaseException(string message)
            : base(message)
        {
        }
    }
}
