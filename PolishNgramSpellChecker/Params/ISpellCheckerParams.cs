using System;

namespace PolishNgramSpellChecker.Params
{
    public interface ISpellCheckerParams
    {
        int N { get; set; }
        int MinN { get; set; }
        int MaxN { get; set; }
        bool OrderedMatch { get; set; }
        Func<double, double, double> ScoreCountFunc { get; set; }
    }
}