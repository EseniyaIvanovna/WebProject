using Application.Exceptions.Application.Exceptions;
using System.Net;

namespace Application.Exceptions
{
    public class ConflictApplicationException : BaseApplicationException
    {
        public ConflictApplicationException(string message)
            : base(message) { }

        public ConflictApplicationException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public override string Title => "Conflict";

        public ConflictApplicationException(string entityType, string key)
            : base($"{entityType} with key '{key}' already exists.") { }
    }
}
