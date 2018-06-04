using Nest;

namespace PolishNgramSpellChecker.Params.Interfaces
{
    public interface ICorrectionParams
    {
        int MinN { get; set; }
        int MaxN { get; set; }
        bool OrderedMatch { get; set; }
        double MinScoreSpace { get; set; }
        bool Recursive { get; set; }
        string CorrectionMethod { get; set; }
        bool UseDetection { get; set; }
        Fuzziness Fuzziness { get; set; }
    }
}
