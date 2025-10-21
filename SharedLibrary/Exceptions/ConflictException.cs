namespace SharedLibrary.Exceptions;

public class ConflictException(string message) : BaseCustomException("CONFLICT", message, 409, "Conflict")
{
}
