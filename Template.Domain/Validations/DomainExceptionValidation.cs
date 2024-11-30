using System.Text.RegularExpressions;

namespace Template.Domain.Validation
{
    public class DomainExceptionValidation : Exception
    {
        public DomainExceptionValidation(string error) : base(error)
        { }

        public static void When(bool hasError, string errorMessage)
        {
            if (hasError)
                throw new DomainExceptionValidation(errorMessage);
        }

        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static void ValidateRequiredString(string? value, string errorMessage)
        {
            When(string.IsNullOrWhiteSpace(value), errorMessage);
        }

        public static void ValidateMaxLength(string? value, int maxLength, string errorMessage)
        {
            When(value != null && value.Length > maxLength, errorMessage);
        }

        public static void ValidateFormat(Func<string, bool> formatValidator, string value, string errorMessage)
        {
            When(!formatValidator(value), errorMessage);
        }

        public static void ValidateEmailFormat(string? email, string errorMessage)
        {
            if (!string.IsNullOrEmpty(email))
            {
                When(!EmailRegex.IsMatch(email), errorMessage);
            }
        }
    }
}