using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Chronic
{
    public class OrdinalScanner : ITokenScanner
    {
        protected static readonly Regex EnglishPattern = new Regex(
            @"^(\d*)(st|nd|rd|th)$",
            RegexOptions.Singleline | RegexOptions.Compiled);
        protected static readonly Regex FrenchPattern = new Regex(
            @"^(\d*)(er|eme)$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "en")
                tokens.ForEach(token => token.Tag(
                new ITag[]
                    {
                        ScanOrdinal(token, options),
                        ScanOrdinalDay(token)
                    }.Where(
                        x => x != null).ToList()));
            else if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fr")
                tokens.ForEach(token => token.Tag(
                    new ITag[]
                    {
                        ScanOrdinalFrench(token, options),
                        ScanOrdinalDayFrench(token)
                    }.Where(
                        x => x != null).ToList()));
            return tokens;
        }

        public Ordinal ScanOrdinalFrench(Token token, Options options)
        {
            var match = FrenchPattern.Match(token.Value);

            if (match.Success)
            {
                return new Ordinal(int.Parse(match.Groups[1].Value));
            }
            return null;
        }

        public OrdinalDay ScanOrdinalDayFrench(Token token)
        {
            var match = FrenchPattern.Match(token.Value);

            if (match.Success)
            {
                var value = int.Parse(match.Groups[1].Value);
                if (value <= 31)
                    return new OrdinalDay(value);
            }
            return null;
        }

        public Ordinal ScanOrdinal(Token token, Options options)
        {
            var match = EnglishPattern.Match(token.Value);

            if (match.Success)
            {
                return new Ordinal(int.Parse(match.Groups[1].Value));
            }
            return null;
        }

        public OrdinalDay ScanOrdinalDay(Token token)
        {
            var match = EnglishPattern.Match(token.Value);

            if (match.Success)
            {
                var value = int.Parse(match.Groups[1].Value);
                if (value <= 31)
                    return new OrdinalDay(value);
            }
            return null;
        }

    }
}