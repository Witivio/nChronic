using System.Collections.Generic;

namespace Chronic.Handlers.FrenchHandler
{
    public class FrenchHourHanlder : IHandler
    {
        public Span Handle(IList<Token> tokens, Options options)
        {
            var now = options.Clock();
            int hours = 0;
            if (tokens.Count >= 2)
            {
                hours = tokens[0].GetTag<Scalar>().Value;
            }
            int minutes = 0;
            if (tokens.Count == 3)
                minutes = tokens[2].GetTag<Scalar>().Value;

            if (hours > 12)
            {
                hours = hours - 12;
            }

            var dayStart = Time.New(now.Year, now.Month, now.Day, hours, minutes);
            return new Span(dayStart, dayStart.AddHours(1));
        }
    }
}