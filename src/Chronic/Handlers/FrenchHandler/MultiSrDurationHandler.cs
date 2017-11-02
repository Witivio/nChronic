using System;
using System.Collections.Generic;
using System.Linq;
using Chronic.Tags.Repeaters;

namespace Chronic.Handlers
{
    public class MultiSrDurationHandler : IHandler
    {
        public Span Handle(IList<Token> tokens, Options options)
        {
            int repeaterHourIndex;
            var duration = GetDuration(tokens, out repeaterHourIndex);
            Span outerSpan;

            try
            {
                outerSpan = new MultiSRHandler().Handle(tokens.Skip(repeaterHourIndex + 1).ToList(), options);
            }
            catch (InvalidOperationException e)
            {
                outerSpan = new RHandler().Handle(tokens.Skip(repeaterHourIndex + 1).ToList(), options);
            }
            if (!duration.HasValue)
                return outerSpan;
            outerSpan.Duration = duration;
            return outerSpan;
        }

        private TimeSpan? GetDuration(IList<Token> tokens, out int repeaterHourIndex)
        {
            repeaterHourIndex = -1;
            if (tokens.Any(t => t.IsTaggedAs<RepeaterHour>()))
            {
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
                    minutes = tokens[repeaterHourIndex + 1].GetTag<Scalar>().Value;
                return new TimeSpan(0, hours, minutes, 0);
            }
           else if (tokens.Any(t => t.IsTaggedAs<IRepeaterDayPortion>()))
            {
                for (var index = 0; index < tokens.Count; index++)
                {
                    var token = tokens[index];
                    if (token.IsTaggedAs<IRepeaterDayPortion>())
                    {
                        repeaterHourIndex = index;
                        break;
                    }
                }

                var tick = tokens[repeaterHourIndex - 1].GetTag<RepeaterTime>().Value;
                return tick.ToTimeSpan();
            }
            return null;
        }
    }
}