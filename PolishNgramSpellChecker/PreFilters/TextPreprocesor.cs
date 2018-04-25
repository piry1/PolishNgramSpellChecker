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
                words[i] = words[i].TrimEnd('.', ',');

                if (char.IsUpper(words[i].First()))
                {
                    var tokens = NamesFilter.GetTokens(words[i]);
                    if (tokens != null)
                        words[i] = tokens;
                }
                else
                {
                    int tmp;
                    bool isNumeric = int.TryParse(words[i], out tmp);
                    if (isNumeric)
                        words[i] = "_number";
                }

                words[i] = words[i].ToLower();
            }

            return words;
        }
    }
}