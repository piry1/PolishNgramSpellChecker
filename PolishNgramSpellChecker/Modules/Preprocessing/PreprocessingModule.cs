using System.Linq;
using PolishNgramSpellChecker.Modules.Preprocessing.NamesFilters;

namespace PolishNgramSpellChecker.Modules.Preprocessing
{
    public static class PreprocessingModule
    {
        public static string[] Process(string text, bool useTags = true)
        {
            var words = text.Trim().Split(' ');

            for (int i = 0; i < words.Length; ++i)
                words[i] = words[i].Trim('.', ',', '-', ':', ';');

            var tmp = words.ToList();
            tmp.RemoveAll(x => x.Length == 0);
            words = tmp.ToArray();

            for (int i = 0; i < words.Length; ++i)
            {
                if (useTags)
                {
                    words[i] = CheckNumber(words[i]);
                    words[i] = CheckNames(words[i]);
                }
                words[i] = words[i].ToLower();
            }

            tmp = words.ToList();
            tmp.RemoveAll(x => x.Length == 0);
            words = tmp.ToArray();

            return words;
        }

        // Check if word is name
        private static string CheckNames(string word)
        {
            if (char.IsUpper(word.First()))
            {
                var tokens = NamesFilter.GetTokens(word);
                if (tokens != null && tokens != "")
                    return tokens;
            }
            return word;
        }

        // Check if word is number
        private static string CheckNumber(string word)
        {
            bool isNumeric = int.TryParse(word, out int _);
            if (isNumeric)
                return "_number";
            return word;
        }
    }
}
