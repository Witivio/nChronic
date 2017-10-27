using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Chronic.Tests
{
    public abstract class ParsingTestsBase
    {
        protected abstract DateTime Now();

        protected void SetThreadCulture(string culture)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
        }

        protected Span Parse(string input, string culture = "en")
        {
            SetThreadCulture(culture);
            Parser.IsDebugMode = true;
            var parser = new Parser(new Options
            {
                Clock = () => Now(),
            });
            return parser.Parse(input);
        }

        protected Span Parse(string input, dynamic options, string culture = "en")
        {
            SetThreadCulture(culture);
            Parser.IsDebugMode = true;
            var aggregatedOptions = TestingExtensions
                .Extend(new Options() { Clock = Now }, options);
            var parser = new Parser(aggregatedOptions);
            return parser.Parse(input);
        }
    }
}