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
        private static string _mistakeDir = @"Data/MistakeFiles";
        static Dictionary<char, char> _dia = new Dictionary<char, char>();
        static HashSet<string> _uniDia = new HashSet<string>();

        static int a = 0;
        static int b = 0;
        static int c = 0;

        #region Initialization

        static MisspellsGenerationModule()
        {
            InitDia();
            LoadUniDia();
            if (!Directory.Exists(_mistakeDir))
                Directory.CreateDirectory(_mistakeDir);
        }

        static void LoadUniDia()
        {
            var lines = File.ReadAllLines(@"Data/uni_dia.txt").ToList();
            lines.ForEach(x => _uniDia.Add(x));
        }

        static void InitDia()
        {
            _dia.Add('ą', 'a');
            _dia.Add('ć', 'c');
            _dia.Add('ę', 'e');
            _dia.Add('ł', 'l');
            _dia.Add('ń', 'n');
            _dia.Add('ó', 'o');
            _dia.Add('ś', 's');
            _dia.Add('ź', 'z');
            _dia.Add('ż', 'z');
        }

        #endregion

        public enum MistakeType
        {
            Clean,
            Misspells,
            NoDia,
            NoDiaAndOrt,
            HeavyMisspells,
            NoDiaHeavyMisspells,
            BazForm,
            HeavyNoDia,
            Shuffle
        }

        public static List<Sentence[]> GetMisspeledSet(string fileName, MistakeType mistake)
        {
            string name = $@"{_mistakeDir}/{Path.GetFileNameWithoutExtension(fileName)}_{mistake}.json";
            if (File.Exists(name))
                return DeserializeText(name);

            return GenerateMistakeFile(fileName, name, mistake);
        }

        private static List<Sentence[]> GenerateMistakeFile(string fileName, string resultName, MistakeType mistake)
        {
            var lines = PreparationModule.LoadTestFile(@"Data/" + fileName, true);
            var res = GenerateMisspelledTexts(lines, 1, Nest.Fuzziness.Auto, mistake);
            SerializeTexts(res, resultName);
            return res;
        }

        private static List<Sentence[]> GenerateMisspelledTexts(List<string[]> oryginalSentences, int count, Fuzziness fuzziness, MistakeType mistake)
        {
            List<Sentence[]> results = new List<Sentence[]>();

            for (int i = 0; i < count; ++i)
            {
                Console.WriteLine($"{i + 1} out of {count}");
                results.Add(GenerateMisspelledText(oryginalSentences, mistake));
            }
            return results;
        }

        private static Sentence[] GenerateMisspelledText(List<string[]> oryginalSentences, MistakeType mistake)
        {
            int n = oryginalSentences.Count();
            var results = new Sentence[n];
            bool removeDia = false;
            bool bazMis = false;
            Func<string, string> func = (x) => x;
            Func<string[], string, Fuzziness, bool, bool, bool, Sentence> func2 = (x, b, c, d, e, f) => throw new NotImplementedException();
            string method = "w";
            Fuzziness fuzziness = Fuzziness.Auto;
            bool shuffle = false;
            switch (mistake)
            {
                case MistakeType.Clean: func = (x) => x; break;
                case MistakeType.NoDia: func = RemoveDia; break;
                case MistakeType.NoDiaAndOrt: func = RemoveDiaOrt; break;
                case MistakeType.HeavyNoDia: func = RemoveHeavyDia; break;
                case MistakeType.Misspells: func2 = GenerateMisspells; method = "w"; break;
                case MistakeType.HeavyMisspells: func2 = GenerateMisspells; method = "d"; break;
                case MistakeType.NoDiaHeavyMisspells: func2 = GenerateMisspells; method = "d"; removeDia = true; break;
                case MistakeType.BazForm: func2 = GenerateMisspells; bazMis = true; break;
                case MistakeType.Shuffle: func2 = GenerateMisspells; shuffle = true; break;
            }
            if (mistake == MistakeType.Clean || mistake == MistakeType.NoDia ||
                mistake == MistakeType.NoDiaAndOrt || mistake == MistakeType.HeavyNoDia)
                for (int i = 0; i < n; ++i)
                {
                    results[i] = MakeMistakes(oryginalSentences[i], func); // GenerateMisspells(oryginalSentences[i], method, fuzziness);
                    Console.Write("\r{0}%", (i + 1) * 100 / n);
                }
            else
            {
                for (int i = 0; i < n; ++i)
                {
                    results[i] = func2(oryginalSentences[i], method, fuzziness, removeDia, bazMis, shuffle); // GenerateMisspells(oryginalSentences[i], method, fuzziness);
                    Console.Write("\r{0}%", (i + 1) * 100 / n);
                }
            }
            Console.WriteLine($"BazForm: {a}\nMisspells: {b}\nHeavy: {c}");
            Console.WriteLine();
            return results;
        }

        private static Sentence GenerateMisspells(string[] words, string method, Fuzziness fuzziness, bool removeDia, bool useBaseForm, bool shuffle = false)
        {
            int length = words.Length;
            int misspellCount = HowManyMisspells(length);
            var sentence = new Sentence(words);

          

            for (int i = 0; sentence.IsWordCorrect.Count(x => x == false) < misspellCount && i < 20; ++i)
            {
                int idx = 0;
                int tryCount = 0;
                do
                {
                    idx = rnd.Next(0, length);
                    tryCount++;
                }
                while ((!sentence.IsWordCorrect[idx] || sentence.OriginalWords[idx].Length < 3) && tryCount < 20);


                if (shuffle)
                {
                    switch (rnd.Next(0, 100) % 4)
                    {

                        case 0:
                            useBaseForm = false;
                            method = "w";
                            break;
                        case 1:
                            useBaseForm = false;
                            method = "d";
                            break;
                        case 2:
                        case 3:     
                            useBaseForm = true;
                            break;
                    }
                }

                if (!useBaseForm)
                {
                    var similarWords = Elastic.GetSimilarWords(sentence.OriginalWords[idx], method, fuzziness);
                    if (similarWords.Count() == 0) continue;
                    sentence.Words[idx] = similarWords.First().Key;
                    sentence.IsWordCorrect[idx] = false;
                    if (method == "w")
                        b++;
                    else
                        c++;
                }
                else
                {
                    var similarWords = Morfeusz.GetSimillarWords(sentence.OriginalWords[idx]);
                    if (similarWords.Count() == 0) continue;
                    sentence.Words[idx] = similarWords.ElementAt(rnd.Next(0, similarWords.Count()));
                    sentence.IsWordCorrect[idx] = false;
                    a++;
                }
            }

            if (removeDia)
                for (int i = 0; i < sentence.Words.Length; ++i)
                    sentence.Words[i] = RemoveDia(sentence.Words[i]);

            sentence.SetIsCorrect();
       

            return sentence;
        }

        private static Sentence MakeMistakes(string[] words, Func<string, string> MistakesGenerator)
        {
            Sentence sentence = new Sentence(words);
            for (int i = 0; i < words.Length; ++i)
                sentence.Words[i] = MistakesGenerator(words[i]);

            sentence.SetIsCorrect();
            return sentence;
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

        private static string MakeFuzzyMisspell(string word, params string[] par)
        {
            var similarWords = Elastic.GetSimilarWords(word, par[0], Fuzziness.Auto);
            if (similarWords.Count() != 0)
                return similarWords.First().Key;
            return word;
        }

        static string RemoveDia(string word)
        {
            string resWord = "";
            foreach (var letter in word)
                resWord += _dia.ContainsKey(letter) ? _dia[letter] : letter;
            return resWord;
        }

        static string RemoveDiaOrt(string word)
        {
            string[] a = new string[] { "ż", "rz", "ch", "h", "ó", "u" };
            string[] b = new string[] { "rz", "ż", "h", "ch", "u", "ó" };

            for (int i = 0; i < a.Length; ++i)
            {
                var tmp = word.Replace(a[i], b[i]);
                if (word != tmp && (i == 0 || i == 2 || i == 4))
                    ++i;
                word = tmp;
            }

            return RemoveDia(word);
        }

        static string RemoveHeavyDia(string word)
        {
            var noDiaWord = RemoveDia(word);
            return _uniDia.Contains(noDiaWord) ?
                noDiaWord : word;
        }

        #region Serialization and deserialization

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

        #endregion
    }
}
