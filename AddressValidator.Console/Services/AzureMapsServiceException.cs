using System;

namespace AddressValidator.Console.Services
{
    public class AzureMapsServiceException : Exception
    {
        public AzureMapsServiceException(string message) 
            : base(message)
        {
        }

        public AzureMapsServiceException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
