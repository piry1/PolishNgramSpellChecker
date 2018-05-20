using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Modules.Correction;
using PolishNgramSpellChecker.Modules.Scoring;
using PolishNgramSpellChecker.Params;
using Elasticsearch = PolishNgramSpellChecker.Database.Elastic;
using PolishNgramSpellChecker.Modules.Orthography;

namespace PolishNgramSpellChecker
{
    static class Test
    {
        static void Main()
        {
            Console.WriteLine("Start");
            Elastic.SetConnection();
            SpellChecker spellChecker = new SpellChecker();
            var fuzzy = new CorrectionModule();
            SpellCheckerParams param = new SpellCheckerParams
            {
                ScoreMulti = false,
                MaxN = 2,
                OrderedMatch = true,
                MinScoreSpace = 0,
                CorrectionMethod = "d",
                DetectionMethod = "w",
                CanSkip = true,
                MinPoints = 1,
                ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
            };
            while (true)
            {
                var text = Console.ReadLine();
                Console.WriteLine("-----------------------------------------");
                
                Stopwatch stopwatch = Stopwatch.StartNew();

                var R = spellChecker.CheckSentence(text, param);
                stopwatch.Stop();
                Console.WriteLine("T: " + stopwatch.ElapsedMilliseconds + "\n");

                foreach (var rr in R.WordsSugestions)
                {
                    foreach (var wowo in rr)
                        Console.WriteLine(wowo.Value + " " + wowo.Key);

                    Console.WriteLine("--------------------------------\n");
                }

                //foreach (var co in R.IsWordCorrect)
                //    Console.WriteLine(co);

                //for(int i =0; i < R.Words.Count(); ++i)
                //{
                //    Console.WriteLine($"{R.WordsScore[i]} -- {R.Words[i]}");
                //}

                //var res = TextPreprocesor.Process(text);
                //foreach (var r in res)
                //    Console.WriteLine(r);

            }


            Console.WriteLine("End");
            Console.ReadLine();
        }

        private static void WriteResult(IScResponse result)
        {
            for (int i = 0; i < result.Words.Length; ++i)
            {
                //if (res.IncorrectWordsIndexes.Contains(i))
                //    Console.ForegroundColor = ConsoleColor.Red;

                if (result.WordsScore[i] <= 0)
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (result.WordsScore[i] <= 1)
                    Console.ForegroundColor = ConsoleColor.Magenta;
                else if (result.WordsScore[i] <= 10)
                    Console.ForegroundColor = ConsoleColor.Yellow;


                Console.Write(result.Words[i] + /*$" {result.WordsScore[i]} " +*/ " ");
                Console.ResetColor();

                if (i < result.JointsScore.Length)
                {
                    if (result.JointsScore[i] <= 0)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else if (result.JointsScore[i] <= 1)
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    else if (result.JointsScore[i] <= 10)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else
                        Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write( /*result.JointsScore[i] + */"* ");
                    Console.ResetColor();
                }
            }
            Console.Write("\n");
        }
    }
}
