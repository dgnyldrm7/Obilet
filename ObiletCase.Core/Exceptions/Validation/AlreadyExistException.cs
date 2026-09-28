using ObiletCase.Core.Exceptions.Base;
using System.Net;

namespace ObiletCase.Core.Exceptions.Validation;

public sealed class AlreadyExistException : CustomBaseException
{
    private const string DefaultMessageFormat = "{propName} : '{propValue}' ile bir {objectName} kaydı mevcut.";

    public override string MessageFormat => DefaultMessageFormat;
    public override string Title => "Already Exist Error";

    public AlreadyExistException(string propName, string propValue, string objectName)
        : base(
            DefaultMessageFormat
                .Replace("{propName}", propName)
                .Replace("{propValue}", propValue)
                .Replace("{objectName}", objectName),
            HttpStatusCode.BadRequest) // REST standartlarında çakışmalar için dilerseniz HttpStatusCode.Conflict (409) da tercih edebilirsiniz
    {
        MessageProps.Add("{propName}", propName);
        MessageProps.Add("{propValue}", propValue);
        MessageProps.Add("{objectName}", objectName);
    }
}