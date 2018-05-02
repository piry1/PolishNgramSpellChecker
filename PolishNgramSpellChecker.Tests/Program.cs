using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Tests.Model;
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
                MaxN = 2,
                OrderedMatch = true,
                MinScoreSpace = 0.0,
                Method = "w",
                MinPoints = 0.1,
                CanSkip = true,
                ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
            };

            string paramName = nameof(SpellCheckerParams.MinScoreSpace);
            Console.WriteLine(paramName);
            object[] paramVector = new object[80]; //{ 0.0, 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4 };

            for (int i = 0; i < 80; ++i)
                paramVector[i] = 0.01 * i;
            
            var textList = new List<Sentence[]>();
            if (File.Exists(@"Data/testLalkaSentences.json"))
                textList = MisspellsGenerationModule.DeserializeText(@"Data/testLalkaSentences.json");
            else
            {
                var sentences = PreparationModule.LoadTestFile(@"Data/testLalka.txt");
                textList = MisspellsGenerationModule.GenerateMisspelledTexts(sentences, 2, "w", Nest.Fuzziness.Auto);
                MisspellsGenerationModule.SerializeTexts(textList, @"Data/testLalkaSentences.json");
            }

            Console.WriteLine("Start Testing");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            TestModule.RunTests(textList, param, paramVector, paramName);
            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds / 1000}s - time");

            Console.WriteLine("END");
            Console.ReadKey();
        }


    }
}
