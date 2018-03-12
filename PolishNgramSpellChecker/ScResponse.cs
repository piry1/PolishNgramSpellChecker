﻿using System.Collections.Generic;
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
        public List<string>[] WordsSugestions { get; }

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

        public ScResponse(string originalText, IEnumerable<string> words, bool isCorrect, IEnumerable<string> correctTextSugestios, List<string>[] wordsSugestions)
        {
            OriginalText = originalText;
            Words = words.ToArray();
            IsCorrect = isCorrect;
            CorrectTextSugestions = correctTextSugestios.ToArray();
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
                WordsScore[i] = WordsSugestions[i].Count == 0 ? 10 : 0;
        }
    }
}