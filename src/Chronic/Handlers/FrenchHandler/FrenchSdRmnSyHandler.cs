using System;
using System.Collections.Generic;
using System.Linq;
using Chronic.Tags.Repeaters;

namespace Chronic.Handlers
{
    public class FrenchSdRmnSyHandler : IHandler
    {
        public Span Handle(IList<Token> tokens, Options options)
        {
            var day = tokens[0].GetTag<ScalarDay>().Value;
            var month = (int)tokens[1].GetTag<RepeaterMonthName>().Value;
            var year = tokens[2].GetTag<ScalarYear>().Value;

            Span span = null;
            try
            {
                var oldTimeToken = tokens.Skip(3).ToList();
                int hours = 0;
                if (oldTimeToken.Count >= 2)
                {
                    hours = oldTimeToken[0].GetTag<Scalar>().Value;
                }
                int minutes = 0;
                if (oldTimeToken.Count == 3)
                    minutes = oldTimeToken[2].GetTag<Scalar>().Value;

                if (hours > 12)
                {
                    hours = hours - 12;
                }

                var dayStart = Time.New(year, month, day, hours, minutes);
                span = new Span(dayStart, dayStart.AddDays(1));
            }
            catch (ArgumentException e)
            {
                span = null;
            }
            return span;
        }
    }
}