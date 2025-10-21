namespace SharedLibrary.Exceptions;

public class ForbiddenException : BaseCustomException
{
    public ForbiddenException() 
        : base("FORBIDDEN", "You are not allowed to perform this action", 403, "Forbidden")
    {
    }

    public ForbiddenException(string message) 
        : base("FORBIDDEN", message, 403, "Forbidden")
    {
    }
}
