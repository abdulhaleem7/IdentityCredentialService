namespace IdentityCredentialService.BusinessLogic.Dtos
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse() { }

        public ApiResponse(bool success, T data, string message = null, int statusCode = 200)
        {
            Success = success;
            Data = data;
            Message = message;
            StatusCode = statusCode;
        }

        public static ApiResponse<T> Ok(T data, string message = "Request was successful.")
        {
            return new ApiResponse<T>(true, data, message, 200);
        }

        public static ApiResponse<T> NotFound(string message = "Resource not found.")
        {
            return new ApiResponse<T>(false, default, message, 404);
        }

        public static ApiResponse<T> BadRequest(string message = "Bad request.")
        {
            return new ApiResponse<T>(false, default, message, 400);
        }

        public static ApiResponse<T> InternalServerError(string message = "An unexpected error occurred.")
        {
            return new ApiResponse<T>(false, default, message, 500);
        }
    }
}
