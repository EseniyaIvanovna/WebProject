using System.Net;

namespace Application.Exceptions
{
    public class EntityUpdateException : BaseApplicationException
    {
        public EntityUpdateException(string message)
            : base(message) { }

        public EntityUpdateException(string message, Exception inner)
            : base(message, inner) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public override string Title => "Update Operation Failed";

        public EntityUpdateException(string entityType, string key)
            : base($"Failed to update {entityType} with key '{key}'. The entity may have been modified or deleted.") { }
    }
}