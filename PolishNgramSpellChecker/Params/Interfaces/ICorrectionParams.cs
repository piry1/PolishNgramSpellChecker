using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
