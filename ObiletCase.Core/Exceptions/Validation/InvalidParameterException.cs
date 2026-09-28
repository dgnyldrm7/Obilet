using ObiletCase.Core.Exceptions.Base;
using System.Net;

namespace ObiletCase.Core.Exceptions.Validation
{
    public sealed class InvalidParameterException : CustomBaseException
    {
        private const string DefaultMessageFormat = "Geçersiz parametre. {fieldName} : {fieldValue}";

        public override string MessageFormat => DefaultMessageFormat;
        public override string Title => "Invalid Parameter Error";

        public InvalidParameterException(string fieldName, string fieldValue)
            : base(
                DefaultMessageFormat
                    .Replace("{fieldName}", fieldName)
                    .Replace("{fieldValue}", fieldValue),
                HttpStatusCode.BadRequest)
        {
            MessageProps.Add("{fieldName}", fieldName);
            MessageProps.Add("{fieldValue}", fieldValue);
        }
    }
}
