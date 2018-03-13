using System;

namespace PolishNgramSpellChecker.Params
{
    public class SpellCheckerParams : ISpellCheckerParams
    {
        public int N { get; set; } = 2;
        public int MinN { get; set; } = 2;
        public int MaxN { get; set; } = 5;
        public bool OrderedMatch { get; set; } = false;
        public double MinScoreSpace { get; set; } = 0.35;
        public Func<double, double, double> ScoreCountFunc { get; set; } = (d, d1) => d;
        public DetectionAlgorithm DetectionAlgorithm { get; set; } = DetectionAlgorithm.Simple;

        public override string ToString()
        {
            return $"N: {N}, MinN: {MinN}, MaxN: {MaxN}, ordered: {OrderedMatch},  detection algorithm: {DetectionAlgorithm}, score space: {MinScoreSpace}";
        }
    }
}
