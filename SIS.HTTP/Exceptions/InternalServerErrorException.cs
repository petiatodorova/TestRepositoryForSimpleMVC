namespace SIS.HTTP.Exceptions
{
    using System;
    using System.Net;

    public class InternalServerErrorException : Exception
    {
        public const HttpStatusCode StatusCode = HttpStatusCode.InternalServerError;

        public const string DefaultMessage = "The Server has encountered an error.";

        public InternalServerErrorException()
            : base(DefaultMessage)
        {
        }
    }
}
