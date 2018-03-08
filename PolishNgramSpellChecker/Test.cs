using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.Database;
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


            while (true)
            {
                var text = Console.ReadLine();
                Console.WriteLine("-----------------------------------------");
                Elastic.NgramFuzzyMatch("zdrowie",  new []{"dzieci"});

                //foreach (var spellParams in spellParamsList)
                //{
                //    var result = spellChecker.CheckSentence(text, spellParams);
                //    //Console.WriteLine($"result: {result.IsCorrect}");
                //    WriteResult(result);
                //}

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
