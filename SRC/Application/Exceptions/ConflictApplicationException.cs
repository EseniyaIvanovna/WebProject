using System.Net;

namespace Application.Exceptions
{
    public class ConflictException : BaseApplicationException
    {
        public ConflictException(string resourceName, object key)
            : base(
                title: "Conflict",
                message: $"{resourceName} with identifier {key} already exists",
                statusCode: HttpStatusCode.Conflict)
        {
            Resource = resourceName;
            Key = key?.ToString() ?? "null";
        }

        public string Resource { get; }
        public string Key { get; }
    }
}