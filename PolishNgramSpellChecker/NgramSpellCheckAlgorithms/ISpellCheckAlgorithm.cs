namespace PolishNgramSpellChecker.NgramSpellCheckAlgorithms
{
    internal interface ISpellCheckAlgorithm
    {
        IScResponse CheckText(string text);


    }
}