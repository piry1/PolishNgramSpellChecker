using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Tests.Model;
using Newtonsoft.Json;
using System.IO;

namespace PolishNgramSpellChecker.Tests.Modules
{
    internal static class MisspellsGenerationModule
    {
        private static Random rnd = new Random();

        public static List<Sentence[]> GenerateMisspelledTexts(List<string[]> oryginalSentences, int count, string method, Fuzziness fuzziness)
        {
            List<Sentence[]> results = new List<Sentence[]>();

            for (int i = 0; i < count; ++i)
            {
                Console.WriteLine($"{i + 1} out of {count}");
                results.Add(GenerateMisspelledText(oryginalSentences, method, fuzziness));
            }
            return results;
        }

        public static Sentence[] GenerateMisspelledText(List<string[]> oryginalSentences, string method, Fuzziness fuzziness)
        {
            int n = oryginalSentences.Count();
            var results = new Sentence[n];
            
            for (int i = 0; i < n; ++i)
            {
                results[i] = GeneratMisspells(oryginalSentences[i], method, fuzziness);
                Console.Write("\r{0}%", (i + 1) * 100 / n);
            }
            Console.WriteLine();
            return results;
        }

        public static Sentence GeneratMisspells(string[] words, string method, Fuzziness fuzziness)
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

        public static void SerializeTexts(List<Sentence[]> data, string path)
        {
            var text = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, text);
        }

        public static List<Sentence[]> DeserializeText(string path)
        {
            var text = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Sentence[]>>(text);
        }

        private static int HowManyMisspells(int wordsCount)
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
