using System.Text.RegularExpressions;
using System.Threading;

namespace Chronic
{
    public static class Numerizer
    {
        #region en
        static readonly dynamic[,] ENGLISH_DIRECT_NUMS = new dynamic[,]
            {
                {"eleven", "11"},
                {"twelve", "12"},
                {"thirteen", "13"},
                {"fourteen", "14"},
                {"fifteen", "15"},
                {"sixteen", "16"},
                {"seventeen", "17"},
                {"eighteen", "18"},
                {"nineteen", "19"},
                {"ninteen", "19"}, // Common mis-spelling
                {"zero", "0"},
                {"one", "1"},
                {"two", "2"},
                {"three", "3"},
                {@"four(\W|$)", "4$1"},
                // The weird regex is so that it matches four but not fourty
                {"five", "5"},
                {@"six(\W|$)", "6$1"},
                {@"seven(\W|$)", "7$1"},
                {@"eight(\W|$)", "8$1"},
                {@"nine(\W|$)", "9$1"},
                {"ten", "10"},
                {@"\ba[\b^$]", "1"}
                // doesn"t make sense for an "a" at the end to be a 1
            };

        static readonly dynamic[,] ENGLISH_ORDINALS = new dynamic[,]
            {
                {"first", "1"},
                {"third", "3"},
                {"fourth", "4"},
                {"fifth", "5"},
                {"sixth", "6"},
                {"seventh", "7"},
                {"eighth", "8"},
                {"ninth", "9"},
                {"tenth", "10"}
            };

        static readonly dynamic[,] ENGLISH_TEN_PREFIXES = new dynamic[,]
            {
                {"twenty", 20},
                {"thirty", 30},
                {"forty", 40},
                {"fourty", 40}, // Common mis-spelling
                {"fifty", 50},
                {"sixty", 60},
                {"seventy", 70},
                {"eighty", 80},
                {"ninety", 90}
            };

        static readonly dynamic[,] ENGLISH_BIG_PREFIXES = new dynamic[,]
            {
                {"hundred", 100},
                {"thousand", 1000},
                {"million", 1000000},
                {"billion", 1000000000},
                {"trillion", 1000000000000},
            };
        #endregion

        #region fr

        static readonly dynamic[,] FRENCH_DIRECT_NUMS = new dynamic[,]
            {
                {"onze", "11"},
                {"douze", "12"},
                {"treize", "13"},
                {"quatorze", "14"},
                {"quinze", "15"},
                {"seize", "16"},
                {"dix sept", "17"},
                {"dix huit", "18"},
                {"dix neuf", "19"},
//                {"ninteen", "19"}, // Common mis-spelling
                {"zero", "0"},
                {@"\bun\b", "1"},
                {@"\bdeux\b", "2"},
                {@"\btrois\b", "3"},
                {@"\bquatre\b", "4"},
                {@"\bcinq\b", "5"},
                {@"\bsix\b", "6"},
                {@"\bsept\b", "7"},
                {@"\bhuit\b", "8"},
                {@"\bneuf\b", "9"},
                {"dix", "10"},
//                {@"vingt (\W|$)", "2$1"},
//                {@"trente (\W|$)", "3$1"},
//                {@"quarante (\W|$)", "4$1"},
//                {@"cinquante (\W|$)", "5$1"},
//                {@"soixante (\W|$)", "6$1"},
//                {@"soixante dix (\W|$)", "7$1"},
//                {@"quatre vingt (\W|$)", "8$1"},
//                {@"quatre vingt dix (\W|$)", "9$1"},
            };

        static readonly dynamic[,] FRENCH_ORDINALS = new dynamic[,]
            {
                {"premier", "1"},
                {"deuxieme", "3"},
                {"quatrieme", "4"},
                {"cinquieme", "5"},
                {"sixieme", "6"},
                {"septieme", "7"},
                {"huitieme", "8"},
                {"neuvieme", "9"},
                {"dixieme", "10"}
            };

        static readonly dynamic[,] FRENCH_TEN_PREFIXES = new dynamic[,]
            {
                {"vingt", 20},
                {"trente", 30},
                {"quarante", 40},
                {"cinquante", 50},
                {"soixante", 60},
                {"soixante dix", 70},
                {"quatre vingt", 80},
                {"quatre vingt dix", 90}
            };

        static readonly dynamic[,] FRENCH_BIG_PREFIXES = new dynamic[,]
            {
                {"cent(?:s)?", 100},
                {"mille(?:s)?", 1000},
                {"million(?:s)?", 1000000},
                {"milliard(?:s)?", 1000000000},
                {"millier de milliard(?:s)?", 1000000000000},
            };

        #endregion
        public static string Numerize(string value)
        {
            var result = value;
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "en")
                result = NumerizeEnglish(result);
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fr")
                result = NumerizeFrench(result);
            return result;
        }

        /// <summary>
        /// limitation à 69
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string NumerizeFrench(string result)
        {
            result = @" +|([^\d])-([^\d])".Compile().Replace(result, "$1 $2");
            result = result.Replace("et demi", "etDemi");
            FRENCH_DIRECT_NUMS.ForEach<string, string>(
                (p, r) =>
                    result =
                        Regex.Replace(
                            result,
                            p,
                            "<num>" + r));

            FRENCH_ORDINALS.ForEach<string, string>(
                (p, r) =>
                    result =
                        Regex.Replace(
                            result,
                            p,
                            "<num>" + r +
                            p.
                                LastCharacters
                                (2)));

            // ten, twenty, etc.

            FRENCH_TEN_PREFIXES.ForEach<string, int>(
                (p, r) =>
                    result =
                        Regex.Replace(
                            result,
                            "(?:" + p + @")(?: et)? *<num>(\d(?=[^\d]|$))*",
                            match => "<num>" + (r + int.Parse(match.Groups[1].Value))));

            FRENCH_TEN_PREFIXES.ForEach<string, int>(
                (p, r) => result = Regex.Replace(result, p, "<num>" + r.ToString()));

            // hundreds, thousands, millions, etc.

            FRENCH_BIG_PREFIXES.ForEach<string, long>(
                (p, r) =>
                {
                    result = Regex.Replace(result, @"(?:<num>)?(\d*) *" + p, match => "<num>" + (r * int.Parse(match.Groups[1].Value == string.Empty ? "1" : match.Groups[1].Value)).ToString());
                    result = Andition(result);
                });
            // fractional addition
            // I'm not combining this with the previous block as using float addition complicates the strings
            // (with extraneous .0"s and such )
            result = Regex.Replace(result, @"(\d +)(?: |et | -)*etDemi", match => (float.Parse(match.Groups[1].Value) + 0.5).ToString());
            result = result.Replace("<num>", "");
            return result;
        }

        private static string NumerizeEnglish(string result)
        {
            // preprocess
            result = @" +|([^\d])-([^\d])".Compile().Replace(result, "$1 $2");
            // will mutilate hyphenated-words but shouldn't matter for date extraction
            result = result.Replace("a half", "haAlf");
            // take the 'a' out so it doesn't turn into a 1, save the half for the end

            // easy/direct replacements

            ENGLISH_DIRECT_NUMS.ForEach<string, string>(
                (p, r) =>
                    result =
                    Regex.Replace(
                        result,
                        p,
                        "<num>" + r));

            ENGLISH_ORDINALS.ForEach<string, string>(
                (p, r) =>
                    result =
                    Regex.Replace(
                        result,
                        p,
                        "<num>" + r +
                            p.
                            LastCharacters
                            (2)));

            // ten, twenty, etc.

            ENGLISH_TEN_PREFIXES.ForEach<string, int>(
                (p, r) =>
                    result =
                    Regex.Replace(
                        result,
                        "(?:" + p + @") *<num>(\d(?=[^\d]|$))*",
                        match => "<num>" + (r + int.Parse(match.Groups[1].Value))));

            ENGLISH_TEN_PREFIXES.ForEach<string, int>(
                (p, r) => result = Regex.Replace(result, p, "<num>" + r.ToString()));

            // hundreds, thousands, millions, etc.

            ENGLISH_BIG_PREFIXES.ForEach<string, long>(
                (p, r) =>
                {
                    result = Regex.Replace(result, @"(?:<num>)?(\d*) *" + p, match => "<num>" + (r * int.Parse(match.Groups[1].Value)).ToString());
                    result = Andition(result);
                });


            // fractional addition
            // I'm not combining this with the previous block as using float addition complicates the strings
            // (with extraneous .0"s and such )
            result = Regex.Replace(result, @"(\d +)(?: |and | -)*haAlf", match => (float.Parse(match.Groups[1].Value) + 0.5).ToString());
            result = result.Replace("<num>", "");
            return result;
        }

        static string Andition(string value)
        {
            var result = value;
            var pattern = @"<num>(\d+)( | and )<num>(\d+)(?=[^\w]|$)".Compile();
            while (true)
            {
                var match = pattern.Match(result);
                if (match.Success == false)
                    break;
                result = result.Substring(0, match.Index) + 
                    "<num>" + ((int.Parse(match.Groups[1].Value) + int.Parse(match.Groups[3].Value)).ToString()) +
                    result.Substring(match.Index + match.Length);
            }
            return result;
        }
    }
}