namespace EnterprisePOS.Helpers;

public class Result
{
	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;
	public string Error { get; }
	public string? ErrorCode { get; }

	protected Result(bool isSuccess, string error, string? errorCode = null)
	{
		if (isSuccess && error != string.Empty)
			throw new ArgumentException();
		if (!isSuccess && error == string.Empty)
			throw new ArgumentException();

		IsSuccess = isSuccess;
		Error = error;
		ErrorCode = errorCode;
	}

	public static Result Success() => new(true, string.Empty);
	public static Result Failure(string error, string? errorCode = null) => new(false, error, errorCode);
}

public class Result<T> : Result
{
	public T? Value { get; }

	protected Result(bool isSuccess, T? value, string error, string? errorCode = null)
		: base(isSuccess, error, errorCode)
	{
		if (isSuccess && value == null)
			throw new ArgumentException();

		Value = value;
	}

	public static Result<T> Success(T value) => new(true, value, string.Empty);
	public static new Result<T> Failure(string error, string? errorCode = null) => new(false, default, error, errorCode);
}
