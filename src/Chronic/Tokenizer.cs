using System;
using System.Collections.Generic;
using System.Linq;
using Chronic.Tags.Repeaters;
using System.Threading;

namespace Chronic
{
    public class Tokenizer
    {
        static readonly List<ITokenScanner> _scanners = new List<ITokenScanner>
        {
            new RepeaterScanner(),
            new GrabberScanner(),
            new PointerScanner(),
            new ScalarScanner(),
            new OrdinalScanner(),
            new SeparatorScanner(),
            new TimeZoneScanner(),
        };

        static readonly List<ITokenScanner> _frenchScanners = new List<ITokenScanner>
        {
            new FrenchRepeaterScanner(),
            new FrenchGrabberScanner(),
            new FrenchPointerScanner(),
            new FrenchScalarScanner(),
            new OrdinalScanner(),
            new FrenchSeparatorScanner(),
            new TimeZoneScanner(),
        };

        IList<Token> TokenizeInternal(string phrase, Options options)
        {
            var tokens = phrase
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => new Token(part))
                .ToList();
            return tokens;
        }

        public IList<Token> Tokenize(string phrase, Options options)
        {
            options.OriginalPhrase = phrase;
            Logger.Log(() => phrase);

            if ( Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "en")
                phrase = NormalizeEnglish(phrase);
            else if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fr")
                phrase = NormalizeFrench(phrase);
            Logger.Log(() => phrase);

            var tokens = TokenizeInternal(phrase, options);

            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "en")
                _scanners.ForEach(scanner => scanner.Scan(tokens, options));

            else if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fr")
                _frenchScanners.ForEach(scanner => scanner.Scan(tokens, options));

            var taggedTokens = tokens.Where(token => token.HasTags()).ToList();
            Logger.Log(() => String.Join(",", taggedTokens.Select(t => t.ToString())));
             
            return taggedTokens;
        }

        public static string NormalizeEnglish(string phrase)
        {
            var normalized = phrase.ToLower();
            normalized = normalized
                .ReplaceAll(@"([/\-,@])", " " + "$1" + " ")
                .ReplaceAll(@"['""\.,]", "")
                .ReplaceAll(@"\bsecond (of|day|month|hour|minute|second)\b", "2nd $1")
                .Numerize()
                .ReplaceAll(@" \-(\d{4})\b", " tzminus$1")
                .ReplaceAll(@"(?:^|\s)0(\d+:\d+\s*pm?\b)", "$1")
                .ReplaceAll(@"\btoday\b", "this day")
                .ReplaceAll(@"\btomm?orr?ow\b", "next day")
                .ReplaceAll(@"\byesterday\b", "last day")
                .ReplaceAll(@"\bnoon\b", "12:00")
                .ReplaceAll(@"\bmidnight\b", "24:00")
                .ReplaceAll(@"\bbefore now\b", "past")
                .ReplaceAll(@"\bnow\b", "this second")
                .ReplaceAll(@"\b(ago|before)\b", "past")
                .ReplaceAll(@"\bthis past\b", "last")
                .ReplaceAll(@"\bthis last\b", "last")
                .ReplaceAll(@"\b(?:in|during) the (morning)\b", "$1")
                .ReplaceAll(@"\b(?:in the|during the|at) (afternoon|evening|night)\b", "$1")
                .ReplaceAll(@"\btonight\b", "this night")
                .ReplaceAll(@"(\d)([ap]m|oclock)\b", "$1 $2")
                .ReplaceAll(@"\b(hence|after|from)\b", "future")
                ;

            return normalized;
        }

        public static string NormalizeFrench(string phrase)
        {
            var normalized = phrase.ToLower();
            normalized = normalized
                .ReplaceAll(@"([/\-,@])", " " + "$1" + " ")
                .ReplaceAll(@"['""\.,]", "")
                .ReplaceAll(@"\bseconde (du|jour|mois|heure|minute|seconde)\b", "2nd $1")
                .Numerize()
                .ReplaceAll(@" \-(\d{4})\b", " tzminus$1")
                .ReplaceAll(@"(?:^|\s)0(\d+:\d+\s*pm?\b)", "$1")
                .ReplaceAll(@"\baujourd\b", "ce jour")
                .ReplaceAll(@"\bdemain\b", "suivant jour")
                .ReplaceAll(@"\bhier\b", "precedent jour")
                .ReplaceAll(@"\bmidi\b", "12:00")
                .ReplaceAll(@"\bminuit\b", "24:00")
                .ReplaceAll(@"\bavant\b", "avant")
                .ReplaceAll(@"\bmaintenant\b", "cette seconde")
                .ReplaceAll(@"\b(avant)\b", "avant")
                .ReplaceAll(@"\bthis past\b", "avant")
                .ReplaceAll(@"\bce dernier\b", "avant")
                .ReplaceAll(@"\b(?:in|during) the (morning)\b", "$1")
                .ReplaceAll(@"\b(?:in the|during the|at) (apres midi|hier|nuit)\b", "$1")
                .ReplaceAll(@"\bcette nuit\b", "cette nuit")
                //.ReplaceAll(@"(\d)([ap]m|oclock)\b", "$1 $2")
                .ReplaceAll(@"\b(dans|ici|from)\b", "future")
                ;

            return normalized;
        }
    }
}