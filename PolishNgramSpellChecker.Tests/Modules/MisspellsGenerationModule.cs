using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Tests.Model;

namespace PolishNgramSpellChecker.Tests.Modules
{
    internal class MisspellsGenerationModule
    {
        private static Random rnd = new Random();

        public Sentence GeneratMisspells(string[] words, string method, Fuzziness fuzziness)
        {
            int length = words.Length;
            int misspellCount = HowManyMisspells(length);
            var sentence = new Sentence(words);
            
            for (int i = 0; sentence.IsWordCorrect.Count(x => x == false) < misspellCount && i < 1000; ++i)
            {
                int idx = 0;

                do idx = rnd.Next(0, length);
                while (!sentence.IsWordCorrect[idx] || sentence.OriginalWords[idx].Length < 3);

                var similarWords = Elastic.GetSimilarWords(sentence.OriginalWords[idx], method, fuzziness);
                if (similarWords.Count() == 0) continue;

                sentence.Words[idx] = similarWords.First().Key;
                sentence.IsWordCorrect[idx] = false;
            }

            return sentence;
        }

        private int HowManyMisspells(int wordsCount)
        {
            if (wordsCount < 3)
                return 0;
            if (wordsCount < 6)
                return 1;
            if (wordsCount < 9)
                return 2;
            return 3;
        }

    }
}
