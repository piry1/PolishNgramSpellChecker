using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;

namespace PolishNgramSpellChecker
{
    public class ScResponse : IScResponse
    {
        public string OriginalText { get; set; }
        public string[] Words { get; set; }
        public bool IsCorrect { get; set; }
        public int[] IncorrectWordsIndexes { get; set; }
        public string[] CorrectTextSugestions { get; set; }
        public double[] JointsScore { get; set; }
        public double[] WordsScore { get; set; }
        public Dictionary<string, double>[] WordsSugestions { get; set; }

        public ScResponse(string[] words)
        {
            Words = words.ToArray();
            IncorrectWordsIndexes = new int[words.Length];
            JointsScore = new double[words.Length -1];
            WordsScore = new double[words.Length];
        }

        public ScResponse(IEnumerable<string> words, bool isCorrect, IEnumerable<int> incorrectWordsIndexes, IEnumerable<string> correctTextSugestios)
        {
            Words = words.ToArray();
            IsCorrect = isCorrect;
            IncorrectWordsIndexes = incorrectWordsIndexes.ToArray();
            CorrectTextSugestions = correctTextSugestios.ToArray();
        }

        public ScResponse(IEnumerable<string> words, bool isCorrect, IEnumerable<double> jointsScore, IEnumerable<string> correctTextSugestios)
        {
            Words = words.ToArray();
            IsCorrect = isCorrect;
            JointsScore = jointsScore.ToArray();
            CorrectTextSugestions = correctTextSugestios.ToArray();
            CountWordsScore();
        }

        public ScResponse(IEnumerable<string> words, bool isCorrect, IEnumerable<string> correctTextSugestios, Dictionary<string, double>[] wordsSugestions)
        {
            Words = words.ToArray();
            IsCorrect = isCorrect;
            //CorrectTextSugestions = correctTextSugestios.ToArray();
            WordsSugestions = wordsSugestions;
            CountWordsScore2();
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

        private void CountWordsScore2()
        {
            WordsScore = new double[Words.Length];
            for (int i = 0; i < Words.Length; ++i)
                WordsScore[i] = WordsSugestions[i].Count == 1 ? 10 : 0;
        }
    }
}