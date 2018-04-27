using System;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Modules.Correction;
using PolishNgramSpellChecker.Modules.Scoring;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Modules.Preprocessing;
using PolishNgramSpellChecker.Modules.Orthography;

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
            var ortographyCorrect = orthographyModule.IsCorrect(words);

            //foreach (var oc in ortographyCorrect)
             //   Console.WriteLine(oc);

            //Console.WriteLine("------------------------------");
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
