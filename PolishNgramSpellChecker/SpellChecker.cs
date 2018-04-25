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
        ScoringModule scoringModule = new ScoringModule();
        CorrectionModule correctionModule = new CorrectionModule();

        public IScResponse CheckSentence(string text, SpellCheckerParams spellCheckerParams)
        {
            var words = PreprocessingModule.Process(text);
            var score = scoringModule.Score(words, spellCheckerParams);

            var shouldSkip = spellCheckerParams.CanSkip ?
                scoringModule.ShouldSkip(score, spellCheckerParams.MinPoints)
                : null;

            //if(shouldSkip != null)
            //for (int i = 0; i < words.Length; ++i)            
            //    Console.WriteLine($"{shouldSkip[i]} -- {words[i]}");

            var response = correctionModule.CheckText(words, spellCheckerParams, shouldSkip);
            return response;
        }

        public SpellChecker() => Elastic.SetConnection();

    }
}
