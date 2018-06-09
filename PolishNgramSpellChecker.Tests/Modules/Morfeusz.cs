using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PolishNgramSpellChecker.Tests.Modules
{
    internal static class Morfeusz
    {
        static Dictionary<string, List<string>> _lemmasDic = new Dictionary<string, List<string>>();

        static Morfeusz() => LoadDic();

        static void LoadDic()
        {
            var lines = File.ReadAllLines(@"Data/lemaUnigrams.txt");
            foreach (var line in lines)
            {
                var words = line.Split();
                _lemmasDic.Add(words.First(), words.Skip(1).ToList());
            }
        }

        public struct InterpMorf
        {
            public int p;
            public int k;
            public string Word;
            public string Lemma;
            public string MorhpTags;
        }

        private static List<string> GetLemmas(string word)
        {
            string url = "http://localhost:5005/" + word;
            string html;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            var result = JsonConvert.DeserializeObject<List<InterpMorf>>(html);

            var s = result.GroupBy(p => p.p, l => l.Lemma, (key, g) => new { Index = key, Lemmas = g.ToList() });
            return s.Select(x => x.Lemmas.First()).ToList();
        }

        public static List<string> GetSimillarWords(string word)
        {
            word = word.ToLower();
            List<string> result = new List<string>();
            List<string> suggestions = new List<string>();
            var lemmas = GetLemmas(word);
            lemmas.RemoveAll(x => x == null);
            foreach (var lem in lemmas)
            {
                if (!_lemmasDic.ContainsKey(lem)) continue;
                var words = _lemmasDic[lem];
                words.ForEach(x => suggestions.Add(CheckWord(word, x)));
                suggestions.RemoveAll(x => x == null);
            }

            return suggestions;
        }

        private static string CheckWord(string word, string suggestion)
        {
            var len = word.Length;
            int maxEdit = 2;
            if (len < 3) return null;
            if (len < 6) maxEdit = 1;
            var editDist = EditDistance(word, suggestion);
            return editDist > 0 && editDist <= maxEdit && word[0] == suggestion[0] ? suggestion : null;
        }

        private static int EditDistance(string original, string modified)
        {
            if (original == modified)
                return 0;

            int len_orig = original.Length;
            int len_diff = modified.Length;
            if (len_orig == 0 || len_diff == 0)
                return len_orig == 0 ? len_diff : len_orig;

            var matrix = new int[len_orig + 1, len_diff + 1];

            for (int i = 1; i <= len_orig; i++)
            {
                matrix[i, 0] = i;
                for (int j = 1; j <= len_diff; j++)
                {
                    int cost = modified[j - 1] == original[i - 1] ? 0 : 1;
                    if (i == 1)
                        matrix[0, j] = j;

                    var vals = new int[] {
                    matrix[i - 1, j] + 1,
                    matrix[i, j - 1] + 1,
                    matrix[i - 1, j - 1] + cost
                };
                    matrix[i, j] = vals.Min();
                    if (i > 1 && j > 1 && original[i - 1] == modified[j - 2] && original[i - 2] == modified[j - 1])
                        matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
                }
            }
            return matrix[len_orig, len_diff];
        }
    }



}
