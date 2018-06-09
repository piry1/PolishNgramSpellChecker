using System;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Modules.Correction;
using PolishNgramSpellChecker.Modules.Scoring;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Modules.Preprocessing;
using PolishNgramSpellChecker.Modules.Orthography;
using PolishNgramSpellChecker.Modules.AfterCorrection;

namespace PolishNgramSpellChecker
{
    public class SpellChecker
    {
        ScoringModule scoringModule = new ScoringModule();
        CorrectionModule correctionModule = new CorrectionModule();
        OrthographyModule orthographyModule = new OrthographyModule();

        public IScResponse CheckSentence(string text, SpellCheckerParams spellCheckerParams)
        {
            var words = PreprocessingModule.Process(text);
            var score = scoringModule.Score(words, spellCheckerParams);

            var shouldSkip = spellCheckerParams.UseDetection ?
                scoringModule.ShouldSkip(score, spellCheckerParams.MinPoints)
                : null;

            if (spellCheckerParams.UseDetection && spellCheckerParams.DetectionMethod == "p")
                for (int i = 0; i < shouldSkip.Length; ++i)
                    shouldSkip[i] = !shouldSkip[i];

            var response = correctionModule.CheckText(words, spellCheckerParams, shouldSkip);
            response = AfterCorrectionModule.Check(response);
            return response;
        }

        public SpellChecker() => Elastic.SetConnection();

    }
}
