using System;
using PolishNgramSpellChecker.Params.Interfaces;

namespace PolishNgramSpellChecker.Params
{
    public class SpellCheckerParams : IScoringParams, ICorrectionParams
    {
        public int MinN { get; set; } = 2;
        public int MaxN { get; set; } = 5;
        public double MinScoreSpace { get; set; } = 0.35;
        public double MinPoints { get; set; } = 20;
        public string CorrectionMethod { get; set; } = "w";
        public bool Recursive { get; set; } = false;
        public bool OrderedMatch { get; set; } = false;
        public bool ScoreMulti { get; set; } = false;
        public bool UseDetection { get; set; } = true;
        public Func<double, double, double> ScoreCountFunc { get; set; } = (d, d1) => d;
        public string DetectionMethod { get; set; } = "w";

        public override string ToString()
        {
            return $"MinN: {MinN}, MaxN: {MaxN}, ordered: {OrderedMatch},  recursive: {Recursive}, score space: {MinScoreSpace}, method: {CorrectionMethod}";
        }

        public string ToCsvString()
        {
            return $"MinN;MaxN;MinScoreSpace;MinPoints;Method;Recursive;OrderedMatch;ScoreMulti;CanSkip\n" +
                $"{MinN};{MaxN};{MinScoreSpace};{MinPoints};{CorrectionMethod};{Recursive};{OrderedMatch};{ScoreMulti};{UseDetection}";
        }

        public SpellCheckerParams GetCopy()
        {
            return new SpellCheckerParams()
            {
                UseDetection = UseDetection,
                ScoreMulti = ScoreMulti,
                OrderedMatch = OrderedMatch,
                Recursive = Recursive,
                CorrectionMethod = CorrectionMethod,
                MinPoints = MinPoints,
                MaxN = MaxN,
                MinN = MinN,
                MinScoreSpace = MinScoreSpace,
                ScoreCountFunc = ScoreCountFunc
        };

    }
}
}
