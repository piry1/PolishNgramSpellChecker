using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Modules.Orthography;

namespace PolishNgramSpellChecker.Tests.Modules
{
    internal static class PreparationModule
    {
        static OrthographyModule orthography = new OrthographyModule();

        private static SpellCheckerParams spellCheckerParams = new SpellCheckerParams()
        {
            Recursive = false,
            ScoreMulti = false,
            MaxN = 2,
            OrderedMatch = true,
            MinScoreSpace = 0.0,
            CorrectionMethod = "w",
            MinPoints = 0.1,
            UseDetection = false,
            ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
        };

        public static List<string[]> LoadTestFile(string path, SpellChecker spellChecker = null)
        {
            string localPath = $"Data//{Path.GetFileNameWithoutExtension(path)}_lines.txt";
            var result = CheckFileIfExist(localPath);
            if (result.Count != 0) return result;
            List<string> wrongWords = new List<string>();
        
            var text = File.ReadAllText(path);
            var sentences = text.Split(new char[] { '.', ':', ';', '"', '”', '“', '„', '(', ')', '-', '–', '?', '!' });

            for (int i = 0; i < sentences.Length; ++i)
            {
                
                sentences[i] = sentences[i].Replace(',', ' ').Trim();
                if (sentences[i].Length == 0) continue;
                var words = sentences[i].Split().ToList();
                words.RemoveAll(x => x.Length == 0);

                var cor = orthography.IsCorrect(words.ToArray());
                for(int x = 0; x < cor.Length; ++x)              
                    if (!cor[x] && !wrongWords.Contains(words[x]))
                        wrongWords.Add(words[x]);            

                if (words.Count() != 0)
                    if (spellChecker != null)
                    {
                        if (CheckWords(words, spellChecker))
                            result.Add(words.ToArray());
                    }
                    else
                        result.Add(words.ToArray());
            }
            Console.WriteLine("SADSADSADS");
            File.WriteAllLines(@"Data/wrongWords.txt", wrongWords);
            SaveToFile(localPath, result);
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

        private static bool CheckWords(List<string> words, SpellChecker spellChecker)
        {
            string text = string.Join(" ", words);
            var res = spellChecker.CheckSentence(text, spellCheckerParams);
            int falseCount = res.IsWordCorrect.Count(x => x == false);
            int length = res.IsWordCorrect.Length;
            return (float)falseCount / length < 0.25;
        }
    }
}
