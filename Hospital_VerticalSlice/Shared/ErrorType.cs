namespace Hospital_VerticalSlice.Shared
{
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Unauthorized = 3,
        Forbidden = 4,
        InvalidCredentials = 5,
        Conflict = 6,
    }

    public class Error
    {
        public string Code { get; }

        public string Description { get; }
        public ErrorType Type { get; }
        private Error(string code, string description, ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }

        public static Error Failure(string Code = "General Failure", string Description = "General Failure Occured !")
        {
            return new Error(Code, Description, ErrorType.Failure);
        }
        public static Error Validation(string Code = "Vaildation Error", string Description = "Vaildation Error Occured !")
        {
            return new Error(Code, Description, ErrorType.Validation);
        }

        public static Error NotFound(string Code = "Not Found !", string Description = "Error 464 Not Found !")
        {
            return new Error(Code, Description, ErrorType.NotFound);
        }

        public static Error Unauthorized(string Code = "Unauthorized Error !", string Description = "Unauthorized Occured !")
        {
            return new Error(Code, Description, ErrorType.Unauthorized);
        }

        public static Error Forbidden(string Code = "Forbidden Error", string Description = "Forbidden Error Occured !")
        {
            return new Error(Code, Description, ErrorType.Forbidden);
        }

        public static Error InvalidCredentials(string Code = "Invalid Credentials Error", string Description = "Invalid Credentials Error Occured !")
        {
            return new Error(Code, Description, ErrorType.InvalidCredentials);
        }

        public static Error Conflict(string Code = "Conflict Error", string Description = "There Is Conflict Occured !")
        {
            return new Error(Code, Description, ErrorType.Conflict);
        }
    }

    public class Result
    {
        protected readonly List<Error> _errors = [];
        public bool IsSuccess => _errors.Count == 0;
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<Error> Errors => _errors;
        public static implicit operator Result(Error error) => new(error);
        protected Result()
        {

        }
        protected Result(Error error)
        {
            _errors.Add(error);
        }
        protected Result(List<Error> errors)
        {
            _errors.AddRange(errors);
        }

        public static Result Ok() => new Result();

        public static Result Fail(Error error) => new Result(error);

        public static Result Fail(List<Error> errors) => new Result(errors);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue _value;
        public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access the value of a failed result.");

        private Result(TValue value)
        {
            _value = value;
        }
        private Result(Error error) : base(error)
        {
            _value = default!;
        }
        private Result(List<Error> errors) : base(errors)
        {
            _value = default!;
        }

        public static Result<TValue> Ok(TValue value) => new Result<TValue>(value);
        public static new Result<TValue> Fail(Error error) => new Result<TValue>(error);
        public static new Result<TValue> Fail(List<Error> errors) => new Result<TValue>(errors);

        public static implicit operator Result<TValue>(TValue value) => Ok(value);
        public static implicit operator Result<TValue>(Error error) => Fail(error);
        public static implicit operator Result<TValue>(List<Error> errors) => Fail(errors);
    }
}
