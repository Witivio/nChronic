using System;
using System.Collections.Generic;
using System.Linq;
using Chronic.Tags.Repeaters;

namespace Chronic.Handlers
{
    public class DurationHandler : IHandler
    {
        public Span Handle(IList<Token> tokens, Options options)
        {
            int repeaterHourIndex = -1;
            for (var index = 0; index < tokens.Count; index++)
            {
                var token = tokens[index];
                if (token.IsTaggedAs<RepeaterHour>())
                {
                    repeaterHourIndex = index;
                    break;
                }
            }

            var hours = tokens[repeaterHourIndex - 1].GetTag<Scalar>().Value;
            int minutes = 0;
            if (tokens[repeaterHourIndex + 1].IsTaggedAs<Scalar>())
            {
                minutes = tokens[repeaterHourIndex + 1].GetTag<Scalar>().Value;
                repeaterHourIndex++;
            }
            var duration = new TimeSpan(0, hours, minutes, 0);

            var outerSpan = new RHandler().Handle(tokens.Skip(repeaterHourIndex +1).ToList(), options);


            return new Span(outerSpan.Start.Value, outerSpan.End.Value, duration);
        }

    }
}