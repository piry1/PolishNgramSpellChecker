using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Modules.Orthography;
using PolishNgramSpellChecker.Modules.Preprocessing;
using PolishNgramSpellChecker.Database;

namespace PolishNgramSpellChecker.Tests.Modules
{
    internal static class PreparationModule
    {
        static OrthographyModule orthography = new OrthographyModule();
        static List<string> WrongSentences = new List<string>();
        static PreparationModule() => Elastic.SetConnection();

        //private static SpellCheckerParams spellCheckerParams = new SpellCheckerParams()
        //{
        //    Recursive = false,
        //    ScoreMulti = false,
        //    MaxN = 2,
        //    OrderedMatch = true,
        //    MinScoreSpace = 0.0,
        //    CorrectionMethod = "w",
        //    MinPoints = 0.1,
        //    UseDetection = false,
        //    ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
        //};

        public static List<string[]> LoadTestFile(string path, bool checkSentences)
        {
            WrongSentences.Clear();
            string localPath = $"Data//{Path.GetFileNameWithoutExtension(path)}_lines.txt";
            var result = CheckFileIfExist(localPath);
            if (result.Count != 0) return result;
            List<string> wrongWords = new List<string>();



            var text = File.ReadAllText(path);
            
            var sentences = text.Split(new char[] { '.', '—', '…', '«', '»', ':', ';', '"', '”', '“', '„', '(', ')', '?', '!', '\n' });
            int len = sentences.Length;
            for (int i = 0; i < sentences.Length; ++i)
            {

                Console.Write($"\r{i} of {len}");

                sentences[i] = sentences[i].Replace(',', ' ');
                sentences[i] = sentences[i].Replace('/', ' ');
                sentences[i] = sentences[i].Replace('*', ' ');
                sentences[i] = sentences[i].Replace('[', ' ');           
                sentences[i] = sentences[i].Replace(']', ' ');
                sentences[i] = sentences[i].Replace('°', ' ').Trim().Trim(new char[]{'-', '–'});
                if (sentences[i].Length == 0) continue;
                var words = sentences[i].Split().ToList();
                words.RemoveAll(x => x.Length == 0);

                var cor = orthography.IsCorrect(words.ToArray());
                for (int x = 0; x < cor.Length; ++x)
                    if (!cor[x] && !wrongWords.Contains(words[x]))
                        wrongWords.Add(words[x]);

                if (words.Count() != 0)
                    if (checkSentences)
                    {
                        if (CheckSentence(words))
                            result.Add(words.ToArray());
                    }
                    else
                        result.Add(words.ToArray());
            }
            Console.WriteLine("\nDone preparing file.\nSaving...");
            File.WriteAllLines(@"Data/wrongWords.txt", wrongWords);
            File.WriteAllLines(@"Data/wrongSentences.txt", WrongSentences);
            SaveToFile(localPath, result);
            Console.WriteLine("Saved");
            return result;
        }

        private static List<string[]> CheckFileIfExist(string path)
        {
            List<string[]> result = new List<string[]>();

            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path).ToList();
                lines.ForEach(x => result.Add(x.Split()));
            }

            return result;
        }

        private static void SaveToFile(string path, List<string[]> result)
        {
            var tmp = new List<string>();
            result.ForEach(x => tmp.Add(string.Join(" ", x)));
            File.WriteAllLines(path, tmp);
        }

        //private static bool CheckWords(List<string> words, SpellChecker spellChecker)
        //{

        //    string text = string.Join(" ", words);
        //    var res = spellChecker.CheckSentence(text, spellCheckerParams);
        //    int falseCount = res.IsWordCorrect.Count(x => x == false);
        //    int length = res.IsWordCorrect.Length;
        //    return (float)falseCount / length < 0.25;
        //}

        private static bool CheckSentence(IEnumerable<string> line)
        {
            var line2 = PreprocessingModule.Process(string.Join(" ", line));
            for (int i = 0; i < line2.Length; ++i)
            {
                var res = Elastic.CheckWord(line2[i]);
                if (res == 0)
                {
                    WrongSentences.Add(string.Join(" ", line));
                    return false;
                }
            }

            return true;
        }
    }
}
