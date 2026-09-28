using ObiletCase.Core.Exceptions.Base;
using System.Net;

namespace ObiletCase.Core.Exceptions.Validation;

public sealed class ClientSideException : CustomBaseException
{
    private const string DefaultMessageFormat = "Client : {clientName} - {processName} işlemi sırasında bir hata oluştu.";

    public override string MessageFormat => DefaultMessageFormat;
    public override string Title => "Client Side Error";

    public ClientSideException(string clientName, string processName)
        : base(
            DefaultMessageFormat
                .Replace("{clientName}", clientName)
                .Replace("{processName}", processName),
            HttpStatusCode.InternalServerError)
    {
        MessageProps.Add("{clientName}", clientName);
        MessageProps.Add("{processName}", processName);
    }
}