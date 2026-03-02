namespace Claims.Services;

/// <summary>
/// Thrown when a business validation rule is violated.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
