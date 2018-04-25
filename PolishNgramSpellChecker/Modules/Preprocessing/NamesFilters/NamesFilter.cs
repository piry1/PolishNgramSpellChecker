using System.Collections.Generic;
using System.IO;

namespace PolishNgramSpellChecker.Modules.Preprocessing.NamesFilters
{
    internal static class NamesFilter
    {
        public static Dictionary<string, Name> Names { get; set; } = new Dictionary<string, Name>();

        static NamesFilter()
        {
            LoadDictionatry(@"Data\namesDictionary.txt");
        }

        private static void LoadDictionatry(string path)
        {
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var data = line.Split(',');
                if (data.Length != 3) continue;
                for (int i = 0; i < data.Length; ++i)
                    data[i] = data[i].Trim();
                Names.Add(data[0], new Name(data[0], data[1], data[2]));
            }
        }

        public static string GetTokens(string word)
        {
            word = word.ToLower();
            if (!Names.ContainsKey(word))
                return null;
            return GenerateTokens(word);
        }

        private static string GenerateTokens(string word)
        {
            List<string> result = new List<string>();
            var name = Names[word];

            for (int i = 0; i < name.MaleCases.Length; ++i)
                if (name.MaleCases[i] == 1)
                    result.Add($"_nm{i}");

            for (int i = 0; i < name.FemaleCases.Length; ++i)
                if (name.FemaleCases[i] == 1)
                    result.Add($"_nf{i}");

            return ArrayToString(result);
        }

        private static string ArrayToString(IEnumerable<string> array)
        {
            string result = string.Empty;

            foreach (var elem in array)
                result += elem + " ";

            result = result.TrimEnd(' ');
            return result.Trim();
        }
    }
}
