using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Chronic
{
    public static class StringExtensions
    {
        public static string ReplaceAll(this string @this, string pattern, string replacement)
        {
            return Regex.Replace(@this, pattern, replacement);            
        }

        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static Regex Compile(this string @this)
        {
            return new Regex(@this, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public static string LastCharacters(this string @this, int numberOfCharsToTake)
        {
            if (@this == null)
            {
                return null;
            }

            if (@this.Length <= numberOfCharsToTake)
            {
                return @this;
            }

            return @this.Substring(@this.Length - numberOfCharsToTake);
        }

        public static string Numerize(this string @this)
        {
            return Numerizer.Numerize(@this);
        }
    }
}
