using System;
using System.Runtime.Serialization;


namespace HDT.Core.Exceptions
{
    public class HDTException: Exception
    {
        public HDTException()
        {

        }
        public HDTException(string message) : base(message)
        {

        }

        public HDTException(string message, Exception innerException): base(message, innerException)
        {

        }

        public HDTException(SerializationInfo serializationInfo, StreamingContext context): base(serializationInfo, context)
        {

        }
    }
}
