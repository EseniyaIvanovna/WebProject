using System.Net;

namespace Application.Exceptions
{
    public class EntityDeleteException : BaseApplicationException
    {
        public EntityDeleteException(string message)
            : base(message) { }

        public EntityDeleteException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
        public override string Title => "Delete Operation Failed";

        public EntityDeleteException(string entityType, string key)
            : base($"Failed to delete {entityType} with key '{key}'. The entity may be in use or already deleted.") { }
    }
}