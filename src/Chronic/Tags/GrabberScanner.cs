using System.Collections.Generic;

namespace Chronic
{
    public class GrabberScanner : ITokenScanner
    {
        static readonly dynamic[] _matches = new dynamic[]
            {
                new { Pattern = "last", Tag = new Grabber(Grabber.Type.Last) },
                new { Pattern = "next", Tag = new Grabber(Grabber.Type.Next) },
                new { Pattern = "this", Tag = new Grabber(Grabber.Type.This) }
            };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(ApplyGrabberTags);
            return tokens;
        }

        static void ApplyGrabberTags(Token token)
        {
            foreach (var match in _matches)
            {
                if (match.Pattern == token.Value)
                {
                    token.Tag(match.Tag);
                }
            }
        }
    }

    public class FrenchGrabberScanner : ITokenScanner
    {
        static readonly dynamic[] _matches = new dynamic[]
            {
                new { Pattern = "precedent", Tag = new Grabber(Grabber.Type.Last) },
                new { Pattern = "precedente", Tag = new Grabber(Grabber.Type.Last) },
                new { Pattern = "suivant", Tag = new Grabber(Grabber.Type.Next) },
                new { Pattern = "suivante", Tag = new Grabber(Grabber.Type.Next) },
                new { Pattern = "prochain", Tag = new Grabber(Grabber.Type.Next) },
                new { Pattern = "prochaine", Tag = new Grabber(Grabber.Type.Next) },
                new { Pattern = "ce", Tag = new Grabber(Grabber.Type.This) },
                new { Pattern = "cette", Tag = new Grabber(Grabber.Type.This) }
            };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(ApplyGrabberTags);
            return tokens;
        }

        static void ApplyGrabberTags(Token token)
        {
            foreach (var match in _matches)
            {
                if (match.Pattern == token.Value)
                {
                    token.Tag(match.Tag);
                }
            }
        }
    }
}