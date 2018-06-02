using System.Collections.Generic;
using System.Linq;

namespace PolishNgramSpellChecker.Tests.Model
{
    internal class Sentence
    {
        public string[] OriginalWords { get; set; }
        public string[] Words { get; set; }
        public bool[] IsWordCorrect { get; set; }
        public List<string>[] Suggestions { get; set; }
        public int[] CorrectSuggestionPossition { get; set; }

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

        public void SetIsCorrect()
        {
            for (int i = 0; i < IsWordCorrect.Length; ++i)
                IsWordCorrect[i] = OriginalWords[i] == Words[i];
        }

    }
}
