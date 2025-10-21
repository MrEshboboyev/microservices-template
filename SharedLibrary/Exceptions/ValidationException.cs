namespace SharedLibrary.Exceptions;

public class ValidationException(Dictionary<string, string[]> errors) : BaseCustomException("VALIDATION_ERROR", "One or more validation errors occurred", 400, "Validation Error")
{
    public Dictionary<string, string[]> Errors { get; } = errors;
}
