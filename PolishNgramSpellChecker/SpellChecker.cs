using System;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Modules.Correction;
using PolishNgramSpellChecker.Modules.Scoring;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Modules.Preprocessing;

namespace PolishNgramSpellChecker
{
    public class SpellChecker
    {
        public IScResponse CheckSentence(string text, SpellCheckerParams spellCheckerParams)
        {
            var words = PreprocessingModule.Process(text);

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
