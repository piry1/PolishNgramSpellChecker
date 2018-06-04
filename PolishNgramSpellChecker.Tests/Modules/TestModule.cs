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
        static string _resultPath = @"Data/TestResults/";

        static TestModule()
        {
            if (!Directory.Exists(_resultPath))
                Directory.CreateDirectory(_resultPath);
        }

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

        public static void CompareMethods(List<Sentence[]> testTextsList, object[] values, string name, params SpellCheckerParams[] spellArgs)
        {
            string f, tpr, fpr;
            f = tpr = fpr = string.Empty;

            foreach (var arg in spellArgs)
            {
                var res = RunTests(testTextsList, arg, values, name);
                f += string.Join(";", res.Select(x => x.ConfusionMatrix.F1score)) + "\n";
                tpr += string.Join(";", res.Select(x => x.ConfusionMatrix.TPR)) + "\n";
                fpr += string.Join(";", res.Select(x => x.ConfusionMatrix.FPR)) + "\n";
            }
            string path = _resultPath + "CompareMethods/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllText(path + "f.csv", f);
            File.WriteAllText(path + "tpr.csv", tpr);
            File.WriteAllText(path + "fpr.csv", fpr);
            File.WriteAllText(path + "param.csv", string.Join(";", values));
        }

        public static List<TestResult> RunTests(List<Sentence[]> testTextsList, SpellCheckerParams spellParams, object[] testParams, string paramName)
        {
            List<TestResult> results = new List<TestResult>();

            for (int i = 0; i < testParams.Length; ++i)
            {
                Console.WriteLine($"TEST {i + 1} out of {testParams.Length}");
                spellParams.GetType().GetProperty(paramName).SetValue(spellParams, testParams[i]);
                var result = RunTest(testTextsList, spellParams.GetCopy(), i == 0);
                results.Add(result);
            }

            SaveResults(_resultPath + paramName + "_test.csv", results, testParams, paramName);
            return results;//results.Select(x => new { F = x.ConfusionMatrix.F1score, TPR = x.ConfusionMatrix.TPR, FPR = x.ConfusionMatrix.FPR });
        }

        public static void RunCrossTests(List<Sentence[]> testTextsList, SpellCheckerParams spellParams, object[] a_params, string a_name, object[] b_params, string b_name)
        {
            List<List<TestResult>> results = new List<List<TestResult>>();
            for (int a = 0; a < a_params.Length; ++a)
            {
                results.Add(new List<TestResult>());
                spellParams.GetType().GetProperty(a_name).SetValue(spellParams, a_params[a]);
                for (int b = 0; b < b_params.Length; ++b)
                {
                    Console.WriteLine($"TEST {a + 1}:{b + 1} out of {a_params.Length}:{b_params.Length}");
                    spellParams.GetType().GetProperty(b_name).SetValue(spellParams, b_params[b]);
                    var result = RunTest(testTextsList, spellParams.GetCopy(), b == 0);
                    results[a].Add(result);
                }
            }
            SaveMatrixResults(_resultPath, results, a_params, a_name, b_params, b_name);
        }

        public static TestResult RunTest(List<Sentence[]> testTextsList, SpellCheckerParams spellParams, bool printStatus = true)
        {
            int n = testTextsList.Count();
            int sentenceCount = testTextsList.Count() * testTextsList[0].Length;
            var wholeTestResult = new TestResult();

            for (int i = 0; i < testTextsList.Count; ++i)
            {
                Console.Write($"\t:{i + 1} texts of {n}");
                var textResult = TestText(testTextsList[i], spellParams, printStatus);
                wholeTestResult.Add(textResult);
            }

            wholeTestResult.Reverse();
            wholeTestResult.ConfusionMatrix = new ConfusionMatrix(wholeTestResult.Target.ToArray(),
                wholeTestResult.Output.ToArray());
            wholeTestResult.SpellCheckerParams = spellParams;

            return wholeTestResult;
            // Console.WriteLine("Done test");          
        }

        private static TestResult TestText(Sentence[] sentences, SpellCheckerParams spellParams, bool printStats = true)
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
                if (printStats)
                    Console.Write($"\r{(i + 1) * 100 / count}%");
            }
            Console.WriteLine();
            return result;
        }

        private static TestResult SaveResults(string path, List<TestResult> results, object[] testParams, string paramName)
        {
            string text = string.Empty;
            string[] paramNames = new string[] { "ACC", "TPR", "TNR", "PPV", "NPV", "F1score", "LRplus" };

            foreach (var result in results)
                text += $"{result.SpellCheckerParams.ToCsvString()}\n\n\n" +
                    $"{result.ConfusionMatrix.ToString()}\n\n\n" +
                    $"{result.SidxToString()}\n\n" +
                    $"time;meanTime\n{result.Time / 1000};{result.MeanWordTime}\n\n\n";

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

            var maxF1value = results.Max(x => x.ConfusionMatrix.F1score);
            var bestCase = results
                .Where(x => x.ConfusionMatrix.F1score == maxF1value).First();

            text += $"\n\nBEST RESULT\n\n{bestCase.SpellCheckerParams.ToCsvString()}\n\n\n" +
                 $"{bestCase.ConfusionMatrix.ToString()}\n\n\n" +
                 $"{bestCase.SidxToString()}\n\n" +
                 $"time;meanTime\n{bestCase.Time / 1000};{bestCase.MeanWordTime}\n\n\n";

            File.WriteAllText(path, text);
            return bestCase;
        }

        private static void SaveMatrixResults(string path, List<List<TestResult>> results, object[] a_params, string a_name, object[] b_params, string b_name)
        {
            double[,] f1matrix = new double[a_params.Length, b_params.Length];

            string text = string.Empty;
            text += string.Join("\n", results.Select(x => string.Join(";", x.Select(y => y.ConfusionMatrix.F1score))));

            //var maxf1 = results.Max(x => x.Max(y => y.ConfusionMatrix.F1score));
            //var bestResult = results.Select(x => x.Where(y => y.ConfusionMatrix.F1score == maxf1)).First().First();

            //var text2 = $"\n\nBEST RESULT\n\n{bestResult.SpellCheckerParams.ToCsvString()}\n\n\n" +
            //  $"{bestResult.ConfusionMatrix.ToString()}\n\n\n" +
            //  $"{bestResult.SidxToString()}\n\n" +
            //  $"time;meanTime\n{bestResult.Time / 1000};{bestResult.MeanWordTime}\n\n\n";

            string matrixPath = _resultPath + "MatrixTest";
            if (!Directory.Exists(matrixPath))
                Directory.CreateDirectory(matrixPath);

            File.WriteAllText(matrixPath + "/matrix" + a_name + "-" + b_name + ".csv", text);
            // File.WriteAllText(matrixPath + "/best" + a_name + "-" + b_name + ".csv", text2);
        }
    }
}
