using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chronic;

namespace Chronic.Handlers
{
    public class SdSmHandler : IHandler
    {
        public virtual Span Handle(IList<Token> tokens, Options options)
        {
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fr")
            {
                var day = (int)tokens[0].GetTag<ScalarDay>().Value;
                var month = (int)tokens[1].GetTag<ScalarMonth>().Value;
                var now = options.Clock();
                var start = Time.New(now.Year, month, day);
                var end = start.AddDays(1);
                return new Span(start, end);
            }
            return null;
        }
    }
}