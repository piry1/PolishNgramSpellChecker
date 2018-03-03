using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker
{
    public interface ISpellChecker
    {
        IScResponse CheckSentence(string text, SpellCheckerParams spellCheckerParams);
    }
}