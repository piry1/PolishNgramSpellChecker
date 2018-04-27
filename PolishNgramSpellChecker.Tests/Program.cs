using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                ScoreMulti = true,
                MaxN = 3,
                OrderedMatch = false,
                MinScoreSpace = 0.35,
                Method = "w",
                CanSkip = true,
                ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
            };
          
            var sentences = PreparationModule.LoadTestFile(@"Data/testLalka.txt");

            Console.WriteLine(sentences.Count());

            int misspels = 0;
            int allWords = 0;

            for (int i = 0; i < sentences.Count(); ++i)
            {
                var result = spellChecker.CheckSentence(sentences[i], param);
                allWords += result.Words.Count();
                misspels += result.IsWordCorrect.Count(x => x == false);
                Console.Write("\r{0}   ", i + 1);         
            }

            Console.WriteLine($"{allWords} - all words");
            Console.WriteLine($"{misspels} - misspelled words");
            Console.WriteLine($"{100 - ((float)misspels / (float)allWords) * 100 }% - correct percent");
            Console.WriteLine($"{((float)misspels / (float)allWords) * 100 }% - mis percent");

            Console.WriteLine("END");
            Console.ReadKey();
        }


    }
}
