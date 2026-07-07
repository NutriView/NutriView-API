namespace NutriView.API.Exceptions
{
    /// <summary>
    /// Thrown by services when a request references data that does not exist or
    /// otherwise fails a business rule. Controllers map this to HTTP 400.
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}