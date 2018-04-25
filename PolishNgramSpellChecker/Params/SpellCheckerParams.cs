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
        public string Method { get; set; } = "w";     
        public bool Recursive { get; set; } = false;
        public bool OrderedMatch { get; set; } = false;
        public bool ScoreMulti { get; set; } = false;
        public bool CanSkip { get; set; } = false;
        public Func<double, double, double> ScoreCountFunc { get; set; } = (d, d1) => d;
        

        public override string ToString()
        {
            return $"MinN: {MinN}, MaxN: {MaxN}, ordered: {OrderedMatch},  recursive: {Recursive}, score space: {MinScoreSpace}, method: {Method}";
        }
    }
}
