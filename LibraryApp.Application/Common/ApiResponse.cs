
namespace LibraryApp.Application.Common
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public ApiResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static ApiResponse Success(string message) => new ApiResponse(true, message);
        public static ApiResponse Failure(string message) => new ApiResponse(false, message);
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }

        public ApiResponse(bool isSuccess, string message, T data) : base(isSuccess, message)
        {
            Data = data;
        }

        public static ApiResponse<T> Success(T data) => new ApiResponse<T>(true, "Successfully", data);
        public static ApiResponse<T> Failure(string message) => new ApiResponse<T>(false, message, default);
    }
}