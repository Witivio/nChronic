using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Chronic.Tags.Repeaters;

namespace Chronic.Tags.Repeaters
{
    public class RepeaterScanner : ITokenScanner
    {
        static readonly List<Func<Token, Options, ITag>> _scanners = new List
            <Func<Token, Options, ITag>>
            {
                //ScanSeasonNames(token, options),
                ScanMonthNames,
                ScanDayNames,
                ScanDayPortions,
                ScanTimes,
                ScanUnits,
            };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(token =>
                {
                    foreach (var scanner in _scanners)
                    {
                        var tag = scanner(token, options);
                        if (tag != null)
                        {
                            token.Tag(tag);
                            break;
                        }
                    }
                });

            return tokens;
        }

        static ITag ScanUnits(Token token, Options options)
        {
            ITag tag = null;
            UnitPatterns.ForEach(item =>
            {
                if (item.Pattern.IsMatch(token.Value))
                {
                    var type = (Type)item.Unit;
                    var hasCtorWithOptions = type.GetConstructors().Any(ctor =>
                    {
                        var parameters = ctor.GetParameters().ToArray();
                        return
                            parameters.Length == 1
                            && parameters.First().ParameterType == typeof(Options);
                    });
                    var ctorParameters = hasCtorWithOptions
                        ? new[] { options }
                        : new object[0];

                    tag = Activator.CreateInstance(
                        type,
                        ctorParameters) as ITag;

                    return;
                }
            });
            return tag;
        }

        static ITag ScanTimes(Token token, Options options)
        {
            var match = _timePattern.Match(token.Value);
            if (match.Success)
            {
                return new RepeaterTime(match.Value);
            }
            return null;
        }

        static ITag ScanDayPortions(Token token, Options options)
        {
            ITag tag = null;
            DayPortionPatterns.ForEach(item =>
                {
                    if (item.Pattern.IsMatch(token.Value))
                    {
                        tag = new EnumRepeaterDayPortion(item.Portion);
                        return;
                    }
                });
            return tag;
        }

        static ITag ScanDayNames(Token token, Options options)
        {
            ITag tag = null;
            DayPatterns.ForEach(item =>
                {
                    if (item.Pattern.IsMatch(token.Value))
                    {
                        tag = new RepeaterDayName(item.Day);
                        return;
                    }
                });
            return tag;
        }

        static ITag ScanMonthNames(Token token, Options options)
        {
            ITag tag = null;
            MonthPatterns.ForEach(item =>
                {
                    if (item.Pattern.IsMatch(token.Value))
                    {
                        tag = new RepeaterMonthName(item.Month);
                        return;
                    }
                });
            return tag;
        }

        static ITag ScanSeasonNames(Token token, Options options)
        {
            throw new NotImplementedException();
        }

        static readonly Regex _timePattern =
            @"^\d{1,2}(:?\d{2})?([\.:h]?\d{2})?$".Compile();

        static readonly List<dynamic> DayPortionPatterns = new List<dynamic>
            {
                new {Pattern = "^ams?$".Compile(), Portion = DayPortion.AM},
                new {Pattern = "^pms?$".Compile(), Portion = DayPortion.PM},
                new {Pattern = "^mornings?$".Compile(), Portion = DayPortion.MORNING},
                new {Pattern = "^afternoons?$".Compile(), Portion = DayPortion.AFTERNOON},
                new {Pattern = "^evenings?$".Compile(), Portion = DayPortion.EVENING},
                new {Pattern = "^(night|nite)s?$".Compile(), Portion = DayPortion.NIGHT},
            };

        static readonly List<dynamic> DayPatterns = new List<dynamic>
            {
                new {Pattern ="^m[ou]n(day)?$".Compile(), Day = DayOfWeek.Monday},
                new {Pattern = "^t(ue|eu|oo|u|)s(day)?$".Compile(), Day = DayOfWeek.Tuesday},
                new {Pattern = "^tue$".Compile(), Day = DayOfWeek.Tuesday},
                new {Pattern = "^we(dnes|nds|nns)day$".Compile(), Day = DayOfWeek.Wednesday},
                new {Pattern = "^wed$".Compile(), Day = DayOfWeek.Wednesday},
                new {Pattern = "^th(urs|ers)day$".Compile(), Day = DayOfWeek.Thursday},
                new {Pattern = "^thu$".Compile(), Day = DayOfWeek.Thursday},
                new {Pattern = "^fr[iy](day)?$".Compile(), Day = DayOfWeek.Friday},
                new {Pattern = "^sat(t?[ue]rday)?$".Compile(), Day = DayOfWeek.Saturday},
                new {Pattern = "^su[nm](day)?$".Compile(), Day = DayOfWeek.Sunday},
            };

        static readonly List<dynamic> MonthPatterns = new List<dynamic>
            {
                new {Pattern = "^jan\\.?(uary)?$".Compile(), Month = MonthName.January},
                new {Pattern = "^feb\\.?(ruary)?$".Compile(), Month = MonthName.February},
                new {Pattern = "^mar\\.?(ch)?$".Compile(), Month = MonthName.March},
                new {Pattern = "^apr\\.?(il)?$".Compile(), Month = MonthName.April},
                new {Pattern = "^may$".Compile(), Month = MonthName.May},
                new {Pattern = "^jun\\.?e?$".Compile(), Month = MonthName.June},
                new {Pattern = "^jul\\.?y?$".Compile(), Month = MonthName.July},
                new {Pattern = "^aug\\.?(ust)?$".Compile(), Month = MonthName.August},
                new
                    {
                        Pattern = "^sep\\.?(t\\.?|tember)?$".Compile(),
                        Month = MonthName.September
                    },
                new {Pattern = "^oct\\.?(ober)?$".Compile(), Month = MonthName.October},
                new {Pattern = "^nov\\.?(ember)?$".Compile(), Month = MonthName.November},
                new {Pattern = "^dec\\.?(ember)?$".Compile(), Month = MonthName.December},
            };

        static readonly List<dynamic> UnitPatterns = new List<dynamic>
            {
                new { Pattern = "^years?$".Compile(), Unit = typeof(RepeaterYear) },
                new { Pattern = "^seasons?$".Compile(), Unit = typeof(RepeaterSeason) },
                new { Pattern = "^months?$".Compile(), Unit = typeof(RepeaterMonth) },
                new { Pattern = "^fortnights?$".Compile(), Unit = typeof(RepeaterFortnight) },
                new { Pattern = "^weeks?$".Compile(), Unit = typeof(RepeaterWeek) },
                new { Pattern = "^weekends?$".Compile(), Unit = typeof(RepeaterWeekend) },
                new { Pattern = "^days?$".Compile(), Unit = typeof(RepeaterDay) },
                new { Pattern = "^hours?$".Compile(), Unit = typeof(RepeaterHour) },
                new { Pattern = "^minutes?$".Compile(), Unit = typeof(RepeaterMinute) },
                new { Pattern = "^seconds?$".Compile(), Unit = typeof(RepeaterSecond) }
            };
    }

    public class FrenchRepeaterScanner : ITokenScanner
    {
        static readonly List<Func<Token, Options, ITag>> _scanners = new List
            <Func<Token, Options, ITag>>
            {
                //ScanSeasonNames(token, options),
                ScanMonthNames,
                ScanDayNames,
                ScanDayPortions,
                ScanTimes,
                ScanUnits,
            };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(token =>
            {
                foreach (var scanner in _scanners)
                {
                    var tag = scanner(token, options);
                    if (tag != null)
                    {
                        token.Tag(tag);
                        break;
                    }
                }
            });

            return tokens;
        }

        static ITag ScanUnits(Token token, Options options)
        {
            ITag tag = null;
            UnitPatterns.ForEach(item =>
            {
                if (item.Pattern.IsMatch(token.Value))
                {
                    var type = (Type)item.Unit;
                    var hasCtorWithOptions = type.GetConstructors().Any(ctor =>
                    {
                        var parameters = ctor.GetParameters().ToArray();
                        return
                            parameters.Length == 1
                            && parameters.First().ParameterType == typeof(Options);
                    });
                    var ctorParameters = hasCtorWithOptions
                        ? new[] { options }
                        : new object[0];

                    tag = Activator.CreateInstance(
                        type,
                        ctorParameters) as ITag;

                    return;
                }
            });
            return tag;
        }

        static ITag ScanTimes(Token token, Options options)
        {
            var match = _timePattern.Match(token.Value);
            if (match.Success)
            {
                return new RepeaterTime(match.Value);
            }
            return null;
        }

        static ITag ScanDayPortions(Token token, Options options)
        {
            ITag tag = null;
            DayPortionPatterns.ForEach(item =>
            {
                if (item.Pattern.IsMatch(token.Value))
                {
                    tag = new EnumRepeaterDayPortion(item.Portion);
                    return;
                }
            });
            return tag;
        }

        static ITag ScanDayPortionsFrench(Token token, Options options)
        {
            ITag tag = null;

            _timeSepratorFrench.ForEach(item =>
            {
                
            });


            return tag;
        }


        static ITag ScanDayNames(Token token, Options options)
        {
            ITag tag = null;
            DayPatterns.ForEach(item =>
            {
                if (item.Pattern.IsMatch(token.Value))
                {
                    tag = new RepeaterDayName(item.Day);
                    return;
                }
            });
            return tag;
        }

        static ITag ScanMonthNames(Token token, Options options)
        {
            ITag tag = null;
            MonthPatterns.ForEach(item =>
            {
                if (item.Pattern.IsMatch(token.Value))
                {
                    tag = new RepeaterMonthName(item.Month);
                    return;
                }
            });
            return tag;
        }

        static ITag ScanSeasonNames(Token token, Options options)
        {
            throw new NotImplementedException();
        }

        static readonly Regex _timePattern =
            @"^\d{1,2}(:?\d{2})?([\.:h]?\d{2})?$".Compile();

        static readonly List<dynamic> DayPortionPatterns = new List<dynamic>
            {
                new {Pattern = "^ams?$".Compile(), Portion = DayPortion.AM},
                new {Pattern = "^pms?$".Compile(), Portion = DayPortion.PM},
                new {Pattern = "^matin?$".Compile(), Portion = DayPortion.MORNING},
                new {Pattern = "^apres midi?$".Compile(), Portion = DayPortion.AFTERNOON},
                new {Pattern = "^soir?$".Compile(), Portion = DayPortion.EVENING},
                new {Pattern = "^(nuit|nite)s?$".Compile(), Portion = DayPortion.NIGHT},
            };

        private static readonly string[] _timeSepratorFrench = new string[]
        {
            ":",
            "h",
            "heures",
        };

        static readonly List<dynamic> DayPatterns = new List<dynamic>
            {
                new {Pattern ="^lundi$".Compile(), Day = DayOfWeek.Monday},
                new {Pattern = "^mardi$".Compile(), Day = DayOfWeek.Tuesday},
                new {Pattern = "^mar$".Compile(), Day = DayOfWeek.Tuesday},
                new {Pattern = "^mercredi$".Compile(), Day = DayOfWeek.Wednesday},
                new {Pattern = "^mer$".Compile(), Day = DayOfWeek.Wednesday},
                new {Pattern = "^jeudi$".Compile(), Day = DayOfWeek.Thursday},
                new {Pattern = "^jeu$".Compile(), Day = DayOfWeek.Thursday},
                new {Pattern = "^vendredi$".Compile(), Day = DayOfWeek.Friday},
                new {Pattern = "^ven$".Compile(), Day = DayOfWeek.Friday},
                new {Pattern = "^samedi$".Compile(), Day = DayOfWeek.Saturday},
                new {Pattern = "^sam$".Compile(), Day = DayOfWeek.Saturday},
                new {Pattern = "^dimanche$".Compile(), Day = DayOfWeek.Sunday},
                new {Pattern = "^dim$".Compile(), Day = DayOfWeek.Sunday},
            };

        static readonly List<dynamic> MonthPatterns = new List<dynamic>
            {
                new {Pattern = "^jan\\.?(vier)?$".Compile(), Month = MonthName.January},
                new {Pattern = "^fev\\.?(rier)?$".Compile(), Month = MonthName.February},
                new {Pattern = "^mar\\.?(s)?$".Compile(), Month = MonthName.March},
                new {Pattern = "^avr\\.?(il)?$".Compile(), Month = MonthName.April},
                new {Pattern = "^mai$".Compile(), Month = MonthName.May},
                new {Pattern = "^juin\\.?e?$".Compile(), Month = MonthName.June},
                new {Pattern = "^jul\\.?(let)?$".Compile(), Month = MonthName.July},
                new {Pattern = "^ao\\.?(ut)?$".Compile(), Month = MonthName.August},
                new
                    {
                        Pattern = "^sep\\.?(t\\.?|tembre)?$".Compile(),
                        Month = MonthName.September
                    },
                new {Pattern = "^oct\\.?(obre)?$".Compile(), Month = MonthName.October},
                new {Pattern = "^nov\\.?(embre)?$".Compile(), Month = MonthName.November},
                new {Pattern = "^dec\\.?(embre)?$".Compile(), Month = MonthName.December},
            };

        static readonly List<dynamic> UnitPatterns = new List<dynamic>
            {
                new { Pattern = "^annees?$".Compile(), Unit = typeof(RepeaterYear) },
                new { Pattern = "^lannee?$".Compile(), Unit = typeof(RepeaterYear) },
                new { Pattern = "^ans?$".Compile(), Unit = typeof(RepeaterYear) },
                new { Pattern = "^seasons?$".Compile(), Unit = typeof(RepeaterSeason) },
                new { Pattern = "^mois$".Compile(), Unit = typeof(RepeaterMonth) },
                new { Pattern = "^fortnights?$".Compile(), Unit = typeof(RepeaterFortnight) },
                new { Pattern = "^semaines?$".Compile(), Unit = typeof(RepeaterWeek) },
                new { Pattern = "^weekends?$".Compile(), Unit = typeof(RepeaterWeekend) },
                new { Pattern = "^jours?$".Compile(), Unit = typeof(RepeaterDay) },
                new { Pattern = "^jrs?$".Compile(), Unit = typeof(RepeaterDay) },
                new { Pattern = "^heures?$".Compile(), Unit = typeof(RepeaterHour) },
                new { Pattern = "h{1,1}$".Compile(), Unit = typeof(RepeaterHour) },
//                new { Pattern = "h?ui*$".Compile(), Unit = typeof(RepeaterDay) },
                new { Pattern = "^h?$".Compile(), Unit = typeof(RepeaterHour) },
                new { Pattern = "min?$".Compile(), Unit = typeof(RepeaterMinute) },
                new { Pattern = "^min?$".Compile(), Unit = typeof(RepeaterMinute) },
                new { Pattern = "^minutes?$".Compile(), Unit = typeof(RepeaterMinute) },
                new { Pattern = "^secondes?$".Compile(), Unit = typeof(RepeaterSecond) }
            };
    }
}