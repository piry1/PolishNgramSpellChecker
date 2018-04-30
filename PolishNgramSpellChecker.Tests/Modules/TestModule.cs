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
            public List<bool> Target { get; set; } = new List<bool>();
            public List<bool> Output { get; set; } = new List<bool>();
            public ConfusionMatrix ConfusionMatrix { get; set; }
            public SpellCheckerParams SpellCheckerParams { get; set; }
            public List<int> SuggestionIndex { get; set; } = new List<int>();
            public double Time { get; set; } = 0;
            public double MeanWordTime { get; set; } = 0;

            public void Add(TestResult testResult)
            {
                Target.AddRange(testResult.Target);
                Output.AddRange(testResult.Output);
                SuggestionIndex.AddRange(testResult.SuggestionIndex);
                Time += testResult.Time;
                MeanWordTime = Time / Target.Count();
            }

            public void Add(IEnumerable<bool> target, IEnumerable<bool> output, IEnumerable<int> idx, double time)
            {
                Target.AddRange(target);
                Output.AddRange(output);
                SuggestionIndex.AddRange(idx);
                Time += time;
                MeanWordTime = Time / Target.Count();
            }

            public void Reverse()
            {
                for (int i = 0; i < Target.Count(); ++i)
                {
                    Target[i] = !Target[i];
                    Output[i] = !Output[i];
                }
            }

            public static List<int> GetSuggestionIndex(bool[] target, bool[] output, string[] words, Dictionary<string, double>[] wordsSugestions)
            {
                List<int> result = new List<int>();

                for (int i = 0; i < target.Count(); ++i)
                {
                    if (!target[i] && !output[i])
                    {
                        string word = words[i].ToLower();
                        if (wordsSugestions[i].ContainsKey(word))
                            result.Add(wordsSugestions[i].Keys.ToList().IndexOf(word) + 1);
                        else
                        {
                            result.Add(-1);
                            //Console.WriteLine(word);
                        }
                    }
                }

                return result;
            }

            public string SidxToString()
            {
                string text = string.Empty;
                int n = SuggestionIndex.Count();

                text += "null;1;2;3;4_5;6_7;8_10;11_15;16_\n";
                text += $"{(float)SuggestionIndex.Count(x => x == -1) / n};";
                text += $"{(float)SuggestionIndex.Count(x => x == 1) / n};";
                text += $"{(float)SuggestionIndex.Count(x => x == 2) / n};";
                text += $"{(float)SuggestionIndex.Count(x => x == 3) / n};";
                text += $"{(float)SuggestionIndex.Count(x => x >= 4 && x <= 5) / n};";
                text += $"{(float)SuggestionIndex.Count(x => x >= 6 && x <= 7) / n};";
                text += $"{(float)SuggestionIndex.Count(x => x >= 8 && x <= 10) / n};";
                text += $"{(float)SuggestionIndex.Count(x => x >= 11 && x <= 15) / n};";
                text += $"{(float)SuggestionIndex.Count(x => x >= 16) / n};";
                return text;
            }
        }

        public static void RunTests(List<Sentence[]> testTextsList, SpellCheckerParams spellParams, object[] testParams, string paramName)
        {
            List<TestResult> results = new List<TestResult>();

            for (int i = 0; i < testParams.Length; ++i)
            {
                Console.WriteLine($"TEST {i + 1} out of {testParams.Length}");
                spellParams.GetType().GetProperty(paramName).SetValue(spellParams, testParams[i]);
                var result = RunTest(testTextsList, spellParams.GetCopy());
                results.Add(result);
            }

            SaveResults(@"Data/" + paramName + "_test.csv", results, testParams, paramName);
        }

        public static TestResult RunTest(List<Sentence[]> testTextsList, SpellCheckerParams spellParams)
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

            wholeTestResult.Reverse();
            wholeTestResult.ConfusionMatrix = new ConfusionMatrix(wholeTestResult.Target.ToArray(),
                wholeTestResult.Output.ToArray());
            wholeTestResult.SpellCheckerParams = spellParams;

            return wholeTestResult;
            // Console.WriteLine("Done test");          
        }

        private static TestResult TestText(Sentence[] sentences, SpellCheckerParams spellParams)
        {
            var result = new TestResult();
            var count = sentences.Length;

            for (int i = 0; i < count; ++i)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var res = spellChecker.CheckSentence(string.Join(" ", sentences[i].Words), spellParams);
                watch.Stop();
                #region display if error
                if (res.Words.Count() != sentences[i].Words.Count())
                {
                    Console.WriteLine(string.Join(" ", sentences[i].OriginalWords));
                    Console.WriteLine(string.Join(" ", res.Words));
                }
                #endregion

                var idx = TestResult.GetSuggestionIndex(sentences[i].IsWordCorrect, res.IsWordCorrect,
                    sentences[i].OriginalWords, res.WordsSugestions);
                result.Add(sentences[i].IsWordCorrect, res.IsWordCorrect, idx, watch.ElapsedMilliseconds);
                Console.Write($"\r{(i + 1) * 100 / count}%");
            }
            Console.WriteLine();
            return result;
        }

        private static void SaveResults(string path, List<TestResult> results, object[] testParams, string paramName)
        {
            string text = string.Empty;
            string[] paramNames = new string[] { "ACC", "TPR", "TNR", "PPV", "NPV", "F1score" };

            foreach (var result in results)
                text += $"{result.SpellCheckerParams.ToCsvString()}\n\n\n" +
                    $"{result.ConfusionMatrix.ToString()}\n\n\n" +
                    $"{result.SidxToString()}\n\n" +
                    $"time;meanTime\n{result.Time/1000};{result.MeanWordTime}\n\n\n";

            foreach (var param in paramNames)
            {
                text += $"\n{param}";
                results.ForEach(x => text += $";{x.ConfusionMatrix.GetType().GetProperty(param).GetValue(x.ConfusionMatrix, null)}");
            }

            text += "\n\nmean T";
            results.ForEach(x => text += $";{x.MeanWordTime}");

            text += $"\n\n{paramName}";
            for (int i = 0; i < testParams.Length; ++i)
                text += $";{testParams[i]}";

            File.WriteAllText(path, text);
        }



    }


}
