namespace EnterprisePOS.Features.POS.Validators;

public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T entity);
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string[] Errors { get; private set; } = [];

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Failure(params string[] errors) => new() { IsValid = false, Errors = errors };
}
