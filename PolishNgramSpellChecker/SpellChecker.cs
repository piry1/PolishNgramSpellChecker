using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Detection;
using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker
{
    public class SpellChecker : ISpellChecker
    {
        public IScResponse CheckSentence(string text, SpellCheckerParams spellCheckerParams)
        {
            IScResponse detectionResponse = null;
            switch (spellCheckerParams.DetectionAlgorithm)
            {
                case DetectionAlgorithm.Simple:
                    var spellChecker = new SimpleNgramDetection();
                    detectionResponse = spellChecker.CheckText(text, spellCheckerParams);
                    break;
                case DetectionAlgorithm.Multi:
                    var multiChecker = new MultiNgramDetection();
                    detectionResponse = multiChecker.CheckText(text, spellCheckerParams);
                    break;
            }

            return detectionResponse;
        }

        public SpellChecker()
        {
        //    Elastic.SetConnection();
        }
    }
}
