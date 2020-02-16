namespace Vowelette
{
    /// <summary>
    /// Represents a simple result of a validation
    /// </summary>
    public struct ValidationResult
    {
        public bool IsSuccessful { get; }

        public string? ErrorMessage { get; }

        /// <summary>
        /// Get a successful validation result
        /// <para>A successful validation result has <see cref="ErrorMessage"/> equal to null</para>
        /// </summary>
        public static readonly ValidationResult Success = new ValidationResult(true);

        /// <summary>
        /// Creates a validation result that fails with the specified message
        /// </summary>
        /// <param name="message">The message to fail the validation result with</param>
        /// <returns>
        /// The validation result that failed with <see cref="ErrorMessage"/>
        /// that was provided by <paramref name="message"/>
        /// </returns>
        public static ValidationResult Failure(string message) => new ValidationResult(false, message);

        private ValidationResult(bool isSuccessful, string? errorMessage = null)
        {
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage;
        }
    }
}
