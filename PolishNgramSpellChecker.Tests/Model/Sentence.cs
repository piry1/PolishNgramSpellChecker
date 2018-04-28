using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNgramSpellChecker.Tests.Model
{
    internal class Sentence
    {
        public string[] OriginalWords { get; private set; }
        public string[] Words { get; private set; }
        public bool[] IsWordCorrect { get; private set; }
        public List<string>[] Suggestions { get; private set; }
        public int[] CorrectSuggestionPossition { get; private set; }

        public Sentence(string[] words)
        {
            OriginalWords = words.ToArray();
            Words = words.ToArray();
            IsWordCorrect = new bool[words.Length];
            Suggestions = new List<string>[words.Length];
            CorrectSuggestionPossition = new int[words.Length];

            for (int i = 0; i < words.Length; ++i)
            {
                IsWordCorrect[i] = true;
                Suggestions[i] = new List<string>();
                CorrectSuggestionPossition[i] = 0;
            }
        }

    }
}
