namespace SIS.HTTP.Exceptions
{
    using System;
    using System.Net;

    public class BadRequestException : Exception
    {
        public const HttpStatusCode StatusCode = HttpStatusCode.BadRequest;

        public const string DefaultMessage = "The Request was malformed or contains unsupported elements.";

        public BadRequestException()
            : base(DefaultMessage)
        {
        }
    }
}
