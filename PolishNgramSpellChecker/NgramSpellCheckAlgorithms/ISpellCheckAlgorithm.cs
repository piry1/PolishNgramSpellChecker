using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker.NgramSpellCheckAlgorithms
{
    internal interface ISpellCheckAlgorithm
    {
        IScResponse CheckText(string[] words, ISpellCheckerParams spellParams);
    }
}