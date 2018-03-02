using System.Collections.Generic;
using System.Linq;

namespace PolishNgramSpellChecker
{
    public class ScResponse : IScResponse
    {
        public string OriginalText { get; }
        public List<string> Words { get; }
        public bool IsCorrect { get; }
        public List<int> IncorrectWordsIndexes { get; }
        public List<string> CorrectTextSugestios { get; }

        public ScResponse(string originalText, List<string> words, bool isCorrect, List<int> incorrectWordsIndexes, List<string> correctTextSugestios)
        {
            OriginalText = originalText;
            Words = words.ToList();
            IsCorrect = isCorrect;
            IncorrectWordsIndexes = incorrectWordsIndexes.ToList();
            CorrectTextSugestios = correctTextSugestios.ToList();
        }
    }
}