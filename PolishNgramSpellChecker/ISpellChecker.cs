namespace PolishNgramSpellChecker
{
    public interface ISpellChecker
    {
        IScResponse CheckSentence(string text);
    }
}