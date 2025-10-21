namespace SharedLibrary.Exceptions;

public class NotFoundException : BaseCustomException
{
    public NotFoundException(string entityName, object key) 
        : base("NOT_FOUND", $"{entityName} with key '{key}' was not found", 404, "Not Found")
    {
    }

    public NotFoundException(string message) 
        : base("NOT_FOUND", message, 404, "Not Found")
    {
    }
}
