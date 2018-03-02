using System.Collections.Generic;

namespace PolishNgramSpellChecker
{
    public interface IScResponse
    {
        string OriginalText { get; }
        string[] Words { get; }
        bool IsCorrect { get; }
        int[] IncorrectWordsIndexes { get; }
        string[] CorrectTextSugestios { get; }
    }
}