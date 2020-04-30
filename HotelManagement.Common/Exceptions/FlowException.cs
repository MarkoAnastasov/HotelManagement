using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Common.Exceptions
{
    public class FlowException : Exception
    {
        public FlowException(string message) : base(message)
        {

        }
    }
}
