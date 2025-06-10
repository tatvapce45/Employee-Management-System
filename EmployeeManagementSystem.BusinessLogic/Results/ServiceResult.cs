using EmployeeManagementSystem.BusinessLogic.Dtos;

namespace EmployeeManagementSystem.BusinessLogic.Results
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }

        public int StatusCode { get; set; } = 200;

        public string? Message { get; set; }

        public List<string>? ValidationErrors { get; set; }

        public Exception? Exception { get; set; }

        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data, string? message = "") =>
            new() { Success = true, StatusCode = 200, Data = data, Message = message };

        public static ServiceResult<T> Created(T data, string? message = "") =>
            new() { Success = true, StatusCode = 201, Data = data, Message = message };

        public static ServiceResult<T> BadRequest(string message, List<string>? validationErrors = null) =>
            new()
            {
                Success = false,
                StatusCode = 400,
                Message = message,
                ValidationErrors = validationErrors
            };

        public static ServiceResult<T> NotFound(string message) =>
            new()
            {
                Success = false,
                StatusCode = 404,
                Message = message
            };

        public static ServiceResult<T> Unauthorized(string message = "Unauthorized access.") =>
            new()
            {
                Success = false,
                StatusCode = 401,
                Message = message
            };

        public static ServiceResult<T> InternalError(string message, Exception? ex = null) =>
            new()
            {
                Success = false,
                StatusCode = 500,
                Message = message,
                Exception = ex
            };
    }
}
