using ObiletCase.Core.Exceptions.Base;
using System.Net;

namespace ObiletCase.Core.Exceptions.Validation
{
    public sealed class NotFoundException : CustomBaseException
    {
        private const string DefaultMessageFormat = "{objectName} Kayıt bulunamadı. {propertyName}: '{propertyValue}'";

        public override string MessageFormat => DefaultMessageFormat;
        public override string Title => "Not Found Error";

        public NotFoundException(string objectName, string propertyName, string propertyValue)
            : base(
                DefaultMessageFormat
                    .Replace("{objectName}", objectName)
                    .Replace("{propertyName}", propertyName)
                    .Replace("{propertyValue}", propertyValue),
                HttpStatusCode.NotFound) // Kayıt bulunamadığı için BadRequest (400) yerine NotFound (404)
        {
            MessageProps.Add("{objectName}", objectName);
            MessageProps.Add("{propertyName}", propertyName);
            MessageProps.Add("{propertyValue}", propertyValue);
        }
    }
}
