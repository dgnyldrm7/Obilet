using System.Net;

namespace ObiletCase.Core.Exceptions.Base;

public abstract class CustomBaseException : Exception
{
    protected CustomBaseException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public virtual string MessageFormat => Message;
    public virtual string Title => "Operation Error";
    public virtual HttpStatusCode StatusCode { get; }
    public virtual Dictionary<string, string> MessageProps { get; set; } = new();
}