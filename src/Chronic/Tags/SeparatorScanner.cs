using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Chronic
{
    public class SeparatorScanner : ITokenScanner
    {
        static readonly dynamic[] Patterns = new dynamic[]
            {
                new { Pattern = @"^,$".Compile(), Tag = new SeparatorComma() },
                new { Pattern = @"^and$".Compile(), Tag = new SeparatorComma() },
                new { Pattern = @"^(at|@)$".Compile(), Tag = new SeparatorAt() },
                new { Pattern = @"^in$".Compile(), Tag = new SeparatorIn() },
                new { Pattern = @"^/$".Compile(), Tag = new SeparatorDate(Separator.Type.Slash) },
                new { Pattern = @"^-$".Compile(), Tag = new SeparatorDate(Separator.Type.Dash) },
                new { Pattern = @"^on$".Compile(), Tag = new SeparatorOn() },
            };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(ApplyTags);
            return tokens;
        }

        static void ApplyTags(Token token)
        {
            foreach (var pattern in Patterns)
            {
                if (pattern.Pattern.IsMatch(token.Value))
                {
                    token.Tag(pattern.Tag);
                }
            }
        }
    }

    public class FrenchSeparatorScanner : ITokenScanner
    {
        static readonly dynamic[] Patterns = new dynamic[]
            {
                new { Pattern = @"^,$".Compile(), Tag = new SeparatorComma() },
                new { Pattern = @"^et$".Compile(), Tag = new SeparatorComma() },
                new { Pattern = @"^(a)$".Compile(), Tag = new SeparatorAt() },
                new { Pattern = @"^dans$".Compile(), Tag = new SeparatorIn() },
                new { Pattern = @"^de$".Compile(), Tag = new SeparatorIn() },
                new { Pattern = @"^en$".Compile(), Tag = new SeparatorIn() },
                new { Pattern = @"^/$".Compile(), Tag = new SeparatorDate(Separator.Type.Slash) },
                new { Pattern = @"^-$".Compile(), Tag = new SeparatorDate(Separator.Type.Dash) },
                new { Pattern = @"^sur$".Compile(), Tag = new SeparatorOn() },
            };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(ApplyTags);
            return tokens;
        }

        static void ApplyTags(Token token)
        {
            foreach (var pattern in Patterns)
            {
                if (pattern.Pattern.IsMatch(token.Value))
                {
                    token.Tag(pattern.Tag);
                }
            }
        }
    }
}