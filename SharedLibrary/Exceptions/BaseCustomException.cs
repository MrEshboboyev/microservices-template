namespace SharedLibrary.Exceptions;

public abstract class BaseCustomException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }
    public string Title { get; }

    protected BaseCustomException(string errorCode, string message, int statusCode, string title) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        Title = title;
    }

    protected BaseCustomException(string errorCode, string message, int statusCode, string title, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        Title = title;
    }
}
