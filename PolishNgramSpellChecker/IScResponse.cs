using System.Collections.Generic;

namespace PolishNgramSpellChecker
{
    public interface IScResponse
    {
        string OriginalText { get; }
        List<string> Words { get; }
        bool IsCorrect { get; }
        List<int> IncorrectWordsIndexes { get; }
        List<string> CorrectTextSugestios { get; }
    }
}