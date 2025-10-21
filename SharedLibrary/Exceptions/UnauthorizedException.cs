namespace SharedLibrary.Exceptions;

public class UnauthorizedException : BaseCustomException
{
    public UnauthorizedException() 
        : base("UNAUTHORIZED", "You are not authorized to perform this action", 401, "Unauthorized")
    {
    }

    public UnauthorizedException(string message) 
        : base("UNAUTHORIZED", message, 401, "Unauthorized")
    {
    }
}
