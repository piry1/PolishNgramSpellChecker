using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;

namespace PolishNgramSpellChecker
{
    public class ScResponse : IScResponse
    {
        public string OriginalText { get; }
        public string[] Words { get; }
        public bool IsCorrect { get; }
        public int[] IncorrectWordsIndexes { get; }
        public string[] CorrectTextSugestions { get; }
        public double[] JointsScore { get; }
        public double[] WordsScore { get; private set; }

        public ScResponse(string originalText, IEnumerable<string> words, bool isCorrect, IEnumerable<int> incorrectWordsIndexes, IEnumerable<string> correctTextSugestios)
        {
            OriginalText = originalText;
            Words = words.ToArray();
            IsCorrect = isCorrect;
            IncorrectWordsIndexes = incorrectWordsIndexes.ToArray();
            CorrectTextSugestions = correctTextSugestios.ToArray();
        }

        public ScResponse(string originalText, IEnumerable<string> words, bool isCorrect, IEnumerable<double> jointsScore, IEnumerable<string> correctTextSugestios)
        {
            OriginalText = originalText;
            Words = words.ToArray();
            IsCorrect = isCorrect;
            JointsScore = jointsScore.ToArray();
            CorrectTextSugestions = correctTextSugestios.ToArray();
            CountWordsScore();
        }


        private void CountWordsScore()
        {
            WordsScore = new double[Words.Length];
            if (WordsScore.Length == 1)
                return;

            for (int i = 0; i < WordsScore.Length; ++i)
            {
                if (i == 0)
                    WordsScore[i] = JointsScore[i];
                else if (i == WordsScore.Length - 1)
                    WordsScore[i] = JointsScore.Last();
                else
                    WordsScore[i] = (JointsScore[i - 1] + JointsScore[i]) / 2;
            }
        }
    }
}