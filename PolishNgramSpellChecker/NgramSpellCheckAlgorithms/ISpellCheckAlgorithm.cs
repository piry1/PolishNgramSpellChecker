using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker.NgramSpellCheckAlgorithms
{
    internal interface ISpellCheckAlgorithm
    {
        IScResponse CheckText(string text, ISpellCheckerParams spellParams);
    }
}