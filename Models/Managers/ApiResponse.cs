namespace Vindi.Webhook.Models.Managers
{
    public sealed class ApiResponse<T>
    {
        #region Propriedades

        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        #endregion

        #region Construtores

        public static ApiResponse<T> Success(T? data)
        {
            return new ApiResponse<T> { 
                IsSuccess = true,
                Data = data
            };
        }

        public static ApiResponse<T> Failure(string message) 
        {
            return new ApiResponse<T>()
            {
                IsSuccess = false,
                Message = message
            };
        }

        public static ApiResponse<T> Failure(string? message, T data)
        {
            return new ApiResponse<T>()
            {
                IsSuccess = false,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Failure(string? message, T? data, IEnumerable<string> errors)
        {
            return new ApiResponse<T>()
            {
                IsSuccess = false,
                Message = message,
                Data = data,
                Errors = errors
            };
        }

        #endregion
    }
}
