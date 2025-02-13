namespace Store_API.DTOs
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Data { get; }
        public List<string> Errors { get; }

        private Result(bool isSuccess, T data, List<string> errors)
        {
            IsSuccess = isSuccess;
            Data = data;
            Errors = errors ?? new List<string>();
        }

        public static Result<T> Success(T data) => new Result<T>(true, data, null);

        public static Result<T> Failure(List<string> errors) => new Result<T>(false, default, errors);

        public static Result<T> Failure(string error) => new Result<T>(false, default, new List<string> { error });
    }
}
