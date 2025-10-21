using Serilog;

namespace SharedLibrary.Services;

public static class LogException
{
    public static void LogExceptions(Exception ex)
    {
        Log.Error($"Exception occurred: {ex.Message}");
        Log.Error($"Stack trace: {ex.StackTrace}");
        
        // Log inner exceptions if any
        var innerException = ex.InnerException;
        while (innerException != null)
        {
            Log.Error($"Inner exception: {innerException.Message}");
            Log.Error($"Inner exception stack trace: {innerException.StackTrace}");
            innerException = innerException.InnerException;
        }
    }
}
