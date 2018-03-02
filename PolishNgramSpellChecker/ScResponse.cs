using System.Collections.Generic;
using System.Linq;

namespace PolishNgramSpellChecker
{
    public class ScResponse : IScResponse
    {
        public string OriginalText { get; }
        public string[] Words { get; }
        public bool IsCorrect { get; }
        public int[] IncorrectWordsIndexes { get; }
        public string[] CorrectTextSugestios { get; }

        public ScResponse(string originalText, IEnumerable<string> words, bool isCorrect, IEnumerable<int> incorrectWordsIndexes, IEnumerable<string> correctTextSugestios)
        {
            OriginalText = originalText;
            Words = words.ToArray();
            IsCorrect = isCorrect;
            IncorrectWordsIndexes = incorrectWordsIndexes.ToArray();
            CorrectTextSugestios = correctTextSugestios.ToArray();
        }
    }
}