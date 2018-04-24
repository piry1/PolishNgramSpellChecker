using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.NgramSpellCheckAlgorithms;
using PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Correction;
using PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Detection;
using PolishNgramSpellChecker.Params;
using Elasticsearch = PolishNgramSpellChecker.Database.Elastic;

namespace PolishNgramSpellChecker
{
    static class Test
    {
        static void Main()
        {
            Console.WriteLine("Start");
            Elastic.SetConnection();
            SpellChecker spellChecker = new SpellChecker();


            var spellParamsList = new List<SpellCheckerParams>()
            {
                new SpellCheckerParams
                {
                    OrderedMatch = true,
                    N = 2,
                    MinN = 2,
                    MaxN = 5,
                    DetectionAlgorithm = DetectionAlgorithm.Simple,
                    ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Standard)
                },
                new SpellCheckerParams
                {
                    OrderedMatch = false,
                    N = 2,
                    MinN = 2,
                    MaxN = 5,
                    DetectionAlgorithm = DetectionAlgorithm.Simple,
                    ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Standard)
                },
                new SpellCheckerParams
                {
                    OrderedMatch = true,
                    N = 2,
                    MinN = 2,
                    MaxN = 5,
                    DetectionAlgorithm = DetectionAlgorithm.Multi,
                    ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
                },
                new SpellCheckerParams
                {
                    OrderedMatch = false,
                    N = 2,
                    MinN = 2,
                    MaxN = 5,
                    DetectionAlgorithm = DetectionAlgorithm.Multi,
                    ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
                }
            };
            var fuzzy = new FuzzySpellCheck();


            SpellCheckerParams param = new SpellCheckerParams();
            param.DetectionAlgorithm = DetectionAlgorithm.Fuzzy;
            param.MaxN = 3;
            param.OrderedMatch = true;
            param.MinScoreSpace = 0;
            param.Method = "f";

             while (true)
            {
                var text = Console.ReadLine();
                Console.WriteLine("-----------------------------------------");
                var R = fuzzy.CheckText(text, param);

                foreach(var rr in R.WordsSugestions)
                {
                    foreach (var wowo in rr)
                        Console.WriteLine(wowo.Value + " " + wowo.Key);

                    Console.WriteLine("\n--------------------------------\n");
                }               

                //string method = "f";

                //var re = Elastic.NgramOrderedFuzzyMatch(0, new[] { "zdrowi", "dzieci" }, method);

                //foreach (var d in re)
                //{
                //    Console.WriteLine(d);
                //}
                Console.WriteLine("\n--------------------------------\n");              
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
