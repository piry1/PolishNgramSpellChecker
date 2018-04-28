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

            MisspellsGenerationModule missModule = new MisspellsGenerationModule();
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

            var sentences = PreparationModule.LoadTestFile(@"Data/testLalka.txt");

            Console.WriteLine(sentences.Count());

            int misspels = 0;
            int allWords = 0;
            int generatedMisspells = 0;
            int correcrAsMisspell = 0;
            int correctAsCorrect = 0;
            int misspellAsMisspell = 0;
            int misspellAsCorrect = 0;


            for (int i = 0; i <sentences.Count(); ++i)
            {
                var ss = missModule.GeneratMisspells(sentences[i].Split(' '), "w", Nest.Fuzziness.Auto);
                generatedMisspells += ss.IsWordCorrect.Count(x => x == false);
                var result = spellChecker.CheckSentence(string.Join(" ", ss.Words), param);

                allWords += result.Words.Count();
                misspels += result.IsWordCorrect.Count(x => x == false);

                for (int j = 0; j < result.IsWordCorrect.Length; ++j)
                {
                    if (result.IsWordCorrect[j] && ss.IsWordCorrect[j])
                        correctAsCorrect++;
                    else if (!result.IsWordCorrect[j] && !ss.IsWordCorrect[j])
                        misspellAsMisspell++;
                    else if (result.IsWordCorrect[j] && !ss.IsWordCorrect[j])
                        misspellAsCorrect++;
                    else if (!result.IsWordCorrect[j] && ss.IsWordCorrect[j])
                        correcrAsMisspell++;
                }

                Console.Write("\r{0}   ", i + 1);
            }

            Console.WriteLine($"{allWords} - all words");
            Console.WriteLine($"{misspels} - misspelled words");
            Console.WriteLine($"{generatedMisspells} - generatedMisspells");
            Console.WriteLine($"{100 - ((float)misspels / (float)allWords) * 100 }% - correct percent");

            Console.WriteLine($"{((float)generatedMisspells / (float)allWords) * 100 }% - generated misspells percent");
            Console.WriteLine($"{((float)misspels / (float)allWords) * 100 }% - mis percent");

            Console.WriteLine($"{(float)correctAsCorrect / (allWords - generatedMisspells) * 100} - correct as correct");
            Console.WriteLine($"{(float)correcrAsMisspell / (allWords - generatedMisspells) * 100} - correct as misspell");
            Console.WriteLine($"{(float)misspellAsCorrect / generatedMisspells * 100} - misspell as correct");
            Console.WriteLine($"{(float)misspellAsMisspell / generatedMisspells * 100} - misspell as misspell");
            Console.WriteLine();

            Console.WriteLine($"{(float)(misspellAsMisspell + correctAsCorrect) / allWords * 100}% - CORRECT classification");
            Console.WriteLine($"{(float)(correcrAsMisspell + misspellAsCorrect) / allWords * 100}% - WRONG classification");

            Console.WriteLine("END");
            Console.ReadKey();
        }


    }
}
