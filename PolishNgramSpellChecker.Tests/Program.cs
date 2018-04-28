using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Tests.Modules;

namespace PolishNgramSpellChecker.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
    
            SpellChecker spellChecker = new SpellChecker();
            SpellCheckerParams param = new SpellCheckerParams()
            {
                Recursive = false,
                ScoreMulti = false,
                MaxN = 3,
                OrderedMatch = true,
                MinScoreSpace = 0.1,
                Method = "w",
                MinPoints = 0.1,
                CanSkip = true,
                ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
            };

            //var sentences = PreparationModule.LoadTestFile(@"Data/testLalka.txt");
            //var textList = MisspellsGenerationModule.GenerateMisspelledTexts(sentences, 2, "w", Nest.Fuzziness.Auto);
            //MisspellsGenerationModule.SerializeTexts(textList, @"Data/testLalkaSentences.json");
            //Console.WriteLine("Serialized");
            var textList = MisspellsGenerationModule.DeserializeText(@"Data/testLalkaSentences.json");

            Console.WriteLine(textList[0].Count());

            var watch = System.Diagnostics.Stopwatch.StartNew();
            TestModule.RunTests(textList, param);
            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds / 1000}s - time");

            Console.WriteLine("END");
            Console.ReadKey();
        }


    }
}
