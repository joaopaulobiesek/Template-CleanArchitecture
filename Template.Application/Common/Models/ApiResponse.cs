namespace Template.Application.Common.Models;

public abstract class ApiResponse<T> where T : notnull
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    protected ApiResponse(bool sucesso, string message, T? data)
    {
        Success = sucesso;
        Message = message;
        Data = data;
    }
}

public class SuccessResponse<T> : ApiResponse<T> where T : notnull
{
    public SuccessResponse(string message, T? data = default) : base(true, message, data)
    {
    }
}

public class ErrorResponse<T> : ApiResponse<T> where T : notnull
{
    public int StatusCode { get; }
    public ICollection<NotificationError>? Errors { get; private set; }

    public ErrorResponse(string message, int statusCode = 400, T? data = default, ICollection<NotificationError>? erros = null) : base(false, message, data)
    {
        StatusCode = statusCode;
        Errors = erros;
    }

    public void AddError(string key, string message)
    {
        Errors ??= new List<NotificationError>();
        Errors.Add(new NotificationError(key, message));
    }
}

public class NotificationError
{
    public string Key { get; set; }

    public string Message { get; set; }

    public NotificationError()
    {
    }

    public NotificationError(string key, string message)
    {
        Key = key;
        Message = message;
    }
}