using System;

namespace PolishNgramSpellChecker.Params
{
    public class SpellCheckerParams : ISpellCheckerParams
    {
        public int N { get; set; } = 2;
        public int MinN { get; set; } = 2;
        public int MaxN { get; set; } = 5;
        public bool OrderedMatch { get; set; } = false;
        public Func<double, double, double> ScoreCountFunc { get; set; } = (d, d1) => d;
        public DetectionAlgorithm DetectionAlgorithm { get; set; }
    }
}
