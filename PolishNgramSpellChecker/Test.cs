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
            var snc = new SimpleNgramDetection();
            var mnd = new MultiNgramDetection();
            while (true)
            {
                var text = Console.ReadLine();


                //var res = Elastic.NgramValue(text);
                //Console.WriteLine(res);
                //res = Elastic.NgramNoOrderValue(text);
                //Console.WriteLine(res);
                SpellCheckerParams spellParams = new SpellCheckerParams()
                {
                    OrderedMatch = false,
                    N = 2
                };
                var result = mnd.CheckText(text, spellParams);

                Console.WriteLine("-----------------------------------------");
                Console.WriteLine($"result: {result.IsCorrect}");
                for (int i = 0; i < result.Words.Length; ++i)
                {
                    //if (res.IncorrectWordsIndexes.Contains(i))
                    //    Console.ForegroundColor = ConsoleColor.Red;

                    if (result.WordsScore[i] == 0)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else if (result.WordsScore[i] <= 1)
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    else if (result.WordsScore[i] <= 10)
                        Console.ForegroundColor = ConsoleColor.Yellow;


                    Console.Write(result.Words[i] + /*$" {result.WordsScore[i]} " +*/ " ");
                    Console.ResetColor();

                    if (i < result.JointsScore.Length)
                    {
                        if (result.JointsScore[i] == 0)
                            Console.ForegroundColor = ConsoleColor.Red;
                        else if (result.JointsScore[i] <= 1)
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        else if (result.JointsScore[i] <= 10)
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        else
                            Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(/*result.JointsScore[i] + */"* ");
                        Console.ResetColor();
                    }


                }
                Console.Write("\n");


                //var res = snc.CheckText(text);
                //Console.WriteLine("-----------------------------------------");
                //Console.WriteLine($"result: {res.IsCorrect}");
                //for (int i = 0; i < res.Words.Length; ++i)
                //{
                //    if (res.IncorrectWordsIndexes.Contains(i))
                //        Console.ForegroundColor = ConsoleColor.Red;
                //    Console.Write(res.Words[i] + " ");
                //    Console.ResetColor();
                //}
                //Console.Write("\n");
            }


            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
