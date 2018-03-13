using System;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Correction;
using PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Detection;
using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker
{
    public class SpellChecker : ISpellChecker
    {
        public IScResponse CheckSentence(string text, SpellCheckerParams spellCheckerParams)
        {
            switch (spellCheckerParams.DetectionAlgorithm)
            {
                case DetectionAlgorithm.Fuzzy:
                case DetectionAlgorithm.FuzzyI:
                    var fuzzy = new FuzzySpellCheck();
                    return fuzzy.CheckText(text, spellCheckerParams);
                default:
                    var spellChecker = new SimpleNgramDetection();
                    return spellChecker.CheckText(text, spellCheckerParams);
            }
        }

        public SpellChecker()
        {
            Elastic.SetConnection();
        }
    }
}
