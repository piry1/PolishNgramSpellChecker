using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNgramSpellChecker.PreFilters
{
    internal static class TextPreprocesor
    {
        public static string[] Process(string text)
        {
            var words = text.Trim().Split(' ');

            for (int i = 0; i < words.Length; ++i)
            {
                words[i] = words[i].Trim('.', ',');
                words[i] = CheckNumber(words[i]);
                words[i] = CheckNames(words[i]);
                words[i] = words[i].ToLower();
            }

            return words;
        }

        // Check if word is name
        private static string CheckNames(string word)
        {
            if (char.IsUpper(word.First()))
            {
                var tokens = NamesFilter.GetTokens(word);
                if (tokens != null)
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