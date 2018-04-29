using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Tests.Model;
using PolishNgramSpellChecker.Tests.Modules;

namespace PolishNgramSpellChecker.Tests.Modules
{
    internal static class TestModule
    {
        static SpellChecker spellChecker = new SpellChecker();

        internal class TestResult
        {
            public List<bool> target { get; set; } = new List<bool>();
            public List<bool> output { get; set; } = new List<bool>();

            public void Add(TestResult testResult)
            {
                target.AddRange(testResult.target);
                output.AddRange(testResult.output);
            }

            public void Add(IEnumerable<bool> target, IEnumerable<bool> output)
            {
                this.target.AddRange(target);
                this.output.AddRange(output);
            }
        }

        public static void RunTests(List<Sentence[]> testTextsList, SpellCheckerParams spellParams)
        {
            int n = testTextsList.Count();
            int sentenceCount = testTextsList.Count() * testTextsList[0].Length;
            var wholeTestResult = new TestResult();

            for (int i = 0; i < testTextsList.Count; ++i)
            {
                Console.Write($"\t:{i + 1} texts of {n}");
                var textResult = TestText(testTextsList[i], spellParams);
                wholeTestResult.Add(textResult);
            }

            var confusionMatrix = new ConfusionMatrix(wholeTestResult.target.ToArray(),
                wholeTestResult.target.ToArray());
            Console.WriteLine("Done test");
            File.WriteAllText("Data/res.csv", confusionMatrix.ToString());
        }

        private static TestResult TestText(Sentence[] sentences, SpellCheckerParams spellParams)
        {
            var result = new TestResult();
            var count = sentences.Length;

            for (int i = 0; i < count; ++i)
            {
                var res = spellChecker.CheckSentence(string.Join(" ", sentences[i].Words), spellParams);
                #region display if error
                if (res.Words.Count() != sentences[i].Words.Count())
                {
                    Console.WriteLine(string.Join(" ", sentences[i].OriginalWords));
                    Console.WriteLine(string.Join(" ", res.Words));
                }
                #endregion
                result.Add(sentences[i].IsWordCorrect, res.IsWordCorrect);
                Console.Write($"\r{(i+1) * 100 / count}%");
            }
            Console.WriteLine();
            return result;
        }

    }


}
