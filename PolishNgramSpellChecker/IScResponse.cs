using System.Collections.Generic;

namespace PolishNgramSpellChecker
{
    public interface IScResponse
    {
        string OriginalText { get; }
        string[] Words { get; }
        bool IsCorrect { get; }
        int[] IncorrectWordsIndexes { get; }
        string[] CorrectTextSugestions { get; }
        double[] JointsScore { get; }
        double[] WordsScore { get; }
    }
}