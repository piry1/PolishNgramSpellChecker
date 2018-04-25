using System;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Correction;
using PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Detection;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.PreFilters;

namespace PolishNgramSpellChecker
{
    public class SpellChecker : ISpellChecker
    {
        public IScResponse CheckSentence(string text, SpellCheckerParams spellCheckerParams)
        {
            var words = TextPreprocesor.Process(text);

            switch (spellCheckerParams.DetectionAlgorithm)
            {
                case DetectionAlgorithm.Fuzzy:
                case DetectionAlgorithm.FuzzyI:
                    var fuzzy = new FuzzySpellCheck();
                    return fuzzy.CheckText(words, spellCheckerParams);
                default:
                    var spellChecker = new SimpleNgramDetection();
                    return spellChecker.CheckText(words, spellCheckerParams);
            }
        }

        public SpellChecker()
        {
            Elastic.SetConnection();
        }
    }
}
