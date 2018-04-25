using System;

namespace PolishNgramSpellChecker.Params.Interfaces
{
    interface IScoringParams
    {
        int MinN { get; set; }
        int MaxN { get; set; }
        bool OrderedMatch { get; set; }
        double MinPoints { get; set; }
        bool ScoreMulti { get; set; }
        Func<double, double, double> ScoreCountFunc { get; set; }
    }
}
