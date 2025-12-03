namespace ForgeFit.MAUI.Models;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    
    public static ServiceResponse<T> Ok(T data)
    {
        return new ServiceResponse<T>
        {
            Data = data, 
            Success = true, 
            StatusCode = 200
        };
    }
    
    public static ServiceResponse<T> Error(string message, int statusCode = 0)
    {
        return new ServiceResponse<T>
        {
            Success = false, 
            Message = message, 
            StatusCode = statusCode
        };
    }
}
