using System;
using Xunit;

namespace Chronic.Tests.Parsing
{
    public class CompleteFrenchTests : ParsingTestsBase
    {
        static readonly DateTime Date = new DateTime(2017, 10, 31);
        static readonly TimeSpan TimeOfDay = new TimeSpan(15, 34, 44);
        protected override DateTime Now()
        {
            return Date.Add(TimeOfDay);
        }

        [Fact]
        public void french_Sentences_are_parsed_correctly()
        {
            Parse("il y a 7 jours", "fr").AssertStartsAt(Time.New(2017, 10, 24, TimeOfDay));
            Parse("dans 7 jours", "fr").AssertStartsAt(Time.New(2017, 11, 7, TimeOfDay));
            Parse("dans 1 semaine", "fr").AssertStartsAt(Time.New(2017, 11, 7, TimeOfDay));
            Parse("dans 2 semaines", "fr").AssertStartsAt(Time.New(2017, 11, 14, TimeOfDay));
            Parse("dans 15 jours", "fr").AssertStartsAt(Time.New(2017, 11, 15, TimeOfDay));
            Parse("la semaine prochaine", "fr").AssertStartsAt(Time.New(2017, 11, 5));
            Parse("2h", "fr").AssertStartsAt(Time.New(2017, 10, 31, 2));
            Parse("2h30", "fr").AssertStartsAt(Time.New(2017, 10, 31, 2, 30));
            Parse("2 heures", "fr").AssertStartsAt(Time.New(2017, 10, 31, 2));
            Parse("2 heures 30", "fr").AssertStartsAt(Time.New(2017, 10, 31, 2, 30));
        }

        [Fact]
        public void french_Sentences_full()
        {
            Parse("une réunion il y a 7 jours", "fr").AssertStartsAt(Time.New(2017, 10, 24, TimeOfDay));
            Parse("trouves moi un creneau dans 7 jours", "fr").AssertStartsAt(Time.New(2017, 11, 7, TimeOfDay));
            Parse("trouves moi un creneau dans 1 semaine", "fr").AssertStartsAt(Time.New(2017, 11, 7, TimeOfDay));
            Parse("trouves moi un creneau dans 2 semaines", "fr").AssertStartsAt(Time.New(2017, 11, 14, TimeOfDay));
            Parse("trouves moi un creneau dans 15 jours", "fr").AssertStartsAt(Time.New(2017, 11, 15, TimeOfDay));
            Parse("trouves moi un creneau la semaine prochaine", "fr").AssertStartsAt(Time.New(2017, 11, 5));
            Parse("trouves moi un creneau demain", "fr").AssertStartsAt(Time.New(2017, 11, 1));
            Parse("trouves moi un creneau de 2 heures demain", "fr").AssertStartsAtWithDuration(Time.New(2017, 11, 1), new TimeSpan(0, 2, 0, 0));
            Parse("trouves moi un creneau de 2 heures 30 demain", "fr").AssertStartsAtWithDuration(Time.New(2017, 11, 1), new TimeSpan(0, 2, 30, 0));
            Parse("trouves moi un creneau de 3h30 la semaine prochaine", "fr").AssertStartsAtWithDuration(Time.New(2017, 11, 5), new TimeSpan(0, 3, 30, 0));
            Parse("trouves moi un creneau de 3h la semaine prochaine", "fr").AssertStartsAtWithDuration(Time.New(2017, 11, 5), new TimeSpan(0, 3, 0, 0));
            Parse("trouves moi un creneau de 1h30 dans 1 semaine", "fr").AssertStartsAtWithDuration(Time.New(2017, 11, 7, TimeOfDay), new TimeSpan(0, 1, 30, 0));
            Parse("trouves moi un creneau avec Nico et Alice de 1h30 dans 1 semaine", "fr").AssertStartsAt(Time.New(2017, 11, 7, TimeOfDay));
            Parse("trouves moi un creneau avec titi@toro.Fr et toto@toro.fr de 1h30 dans 1 semaine", "fr").AssertStartsAt(Time.New(2017, 11, 7, TimeOfDay));
        }
    }
}