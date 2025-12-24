namespace CloudNet.Application.Common.Exceptions;

public sealed class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Forbidden") : base(message)
    {
    }
}
