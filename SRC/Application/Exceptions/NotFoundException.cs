using System.Net;

namespace Application.Exceptions
{
    public class NotFoundApplicationException : BaseApplicationException
    {
        public NotFoundApplicationException(string message) : base(message) { }

        public NotFoundApplicationException(string message, Exception inner)
            : base(message, inner) { }

        public HttpStatusCode StatusCode => HttpStatusCode.NotFound;

        public override string Message => "Entity not found";
    }
}