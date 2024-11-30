namespace Template.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Access forbidden.")
    {
    }

    public ForbiddenAccessException(string message) : base(message)
    {
    }
}
