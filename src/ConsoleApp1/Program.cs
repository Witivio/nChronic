using Chronic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr");

           var parser = new Chronic.Parser();

            Span parsedObj = parser.Parse("dans 1h");

            DateTime? parsedDateTime = parsedObj.Start;


            Console.ReadLine();

        }

        private async static void DoParser()
        {
            // var reader = File.OpenText(@"C:\Users\human\Downloads\cities1000\cities1000.txt");
            //var lines = File.ReadAllLines(@"C:\Users\human\Downloads\cities1000\cities1000.txt");
            //StringBuilder sb = new StringBuilder();
            //while (!reader.EndOfStream)
            //{
            //    var line = await reader.ReadLineAsync();
            //    var idx = line.IndexOf('\t');
            //    var str = line.Substring(0, idx);



            //    // line.Split('\t')[0];
            //}

            // System.Collections.Concurrent.ConcurrentBag<string> c = new System.Collections.Concurrent.ConcurrentBag<string>();

            string sentence = "je suis a lyon";
            string city;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(File.ReadLines(@"C:\Users\human\Downloads\cities1000\cities1000.txt"), (line, state) =>
            {
                //var line = await reader.ReadLineAsync();
                string cityName = line.Split('\t')[1];
                // c.Add(cityName);
                if (sentence.IndexOf(cityName, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    city = cityName;
                    state.Stop();
                }

            });
            var elapsed = sw.Elapsed;

        }
    }
}
