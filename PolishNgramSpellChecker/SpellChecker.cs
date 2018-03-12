using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Detection;
using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker
{
    public class SpellChecker : ISpellChecker
    {
        public IScResponse CheckSentence(string text, SpellCheckerParams spellCheckerParams)
        {            
            var spellChecker = new SimpleNgramDetection();
            var detectionResponse = spellChecker.CheckText(text, spellCheckerParams);    
            return detectionResponse;
        }

        public SpellChecker()
        {
            Elastic.SetConnection();
        }
    }
}
