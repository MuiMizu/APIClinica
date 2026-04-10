using System.Linq;

namespace APIClinica.Help
{
    public static class Validation
    {
        public static bool IsOnlyLetters(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '-' || c == '\'');
        }

        public static bool IsOnlyDigits(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return input.All(char.IsDigit);
        }

        public static bool IsSafeDescription(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            return input.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || 
                                 c == ',' || c == '.' || c == '-' || c == '_' || 
                                 c == '(' || c == ')');
        }
    }
}
