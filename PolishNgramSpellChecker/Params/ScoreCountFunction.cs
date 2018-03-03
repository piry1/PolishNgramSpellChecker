using System;

namespace PolishNgramSpellChecker.Params
{
    public static class ScoreCountFunction
    {
        public static Func<double, double, double> GetFunc(ScoreCountFunctions functions)
        {
            switch (functions)
            {
                case ScoreCountFunctions.Pow10ByN:
                    return (d, n) => Math.Pow(10, n) * d / 1000;
                case ScoreCountFunctions.Standard:
                default:
                    return (d, n) => d;
            }
        }
    }
}