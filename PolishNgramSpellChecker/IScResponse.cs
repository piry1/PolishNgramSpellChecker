using System.Collections.Generic;

namespace PolishNgramSpellChecker
{
    public interface IScResponse
    {
        string OriginalText { get; }
        string[] Words { get; }
        string[] CorrectTextSugestions { get; }
        double[] JointsScore { get; }
        double[] WordsScore { get; }
        Dictionary<string, double>[] WordsSugestions { get; }
        bool[] IsWordCorrect { get; set; }
    }
}