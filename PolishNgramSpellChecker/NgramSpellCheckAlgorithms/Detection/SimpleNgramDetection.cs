using System;
using System.Collections.Generic;
using Elasticsearch.Net;
using System.Linq;
using Nest;
using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Detection
{
    internal class SimpleNgramDetection : ISpellCheckAlgorithm
    {

        public SimpleNgramDetection()
        {
          //  Database.Elastic.SetConnection();
        }

        public IScResponse CheckText(string text, ISpellCheckerParams spellParams)
        {
            var words = text.Trim().Split(' ');
            var jointsScore = new double[words.Length - 1];
            jointsScore = Check2(words, jointsScore, spellParams);
            var isCorrect = IsSentenceCorrect(jointsScore);
            return new ScResponse(text, words, isCorrect, jointsScore, new List<string>());
        }

        public double[] Check2(string[] words, double[] jointsScore, ISpellCheckerParams spellParams)
        {
            var n = spellParams.N;
            for (int i = 0; i < words.Length + 1 - n; ++i)
            {
                string sentence = string.Empty;
                for (int j = i; j < i + n; ++j) // first order
                    sentence += words[j] + " ";

                sentence = sentence.TrimEnd();
                var nCount = Database.Elastic.NgramNvalue(sentence, spellParams.OrderedMatch);
                var score = spellParams.ScoreCountFunc(nCount, n);
                SetJointsScore(ref jointsScore, i, n - 1, score);
            }

            return jointsScore;
        }

        private bool IsSentenceCorrect(double[] jointsScore)
        {
            return !jointsScore.Contains(0);
        }

        private static void SetJointsScore(ref double[] jointsScore, int start, int count, double score)
        {
            for (int i = start; i < count + start; ++i)
                if (jointsScore[i] < score)
                    jointsScore[i] = score;
        }

        #region Old unused methods

        private IScResponse Check(string text, int n, int space = 1)
        {
            var words = text.Split(' ');
            List<int> errorIndexes = new List<int>();
            bool lastResult = true;

            for (int i = 0; i < words.Length + 1 - n; ++i)
            {
                var ngrams = GetAllCombinations(n, space, words, i);
                bool result = false;

                foreach (var ngram in ngrams)
                {
                    result = Database.Elastic.NgramNoOrderExist(ArrayToString(ngram));
                    if (result) break;
                }

                if (!result)
                {
                    lastResult = false;
                    errorIndexes.Add(i);
                }
            }

            return new ScResponse(text, words, lastResult, errorIndexes, new List<string>());
        }

        private string ArrayToString(string[] words)
        {
            string result = string.Empty;
            foreach (var word in words)
                result += word + " ";
            return result;
        }

        private List<string[]> GetAllCombinations(int n, int space, string[] words, int i)
        {
            var ngrams = new List<string[]>();
            string[] ngram = new string[n];

            for (int j = i; j < i + n; ++j) // first order
                ngram[j - i] = words[j];

            ngrams.Add(ngram.ToArray());
            space = GetMaxSpace(n, space, words, i);

            if (space != 1) return ngrams;

            for (int k = n - 1; k > 0; --k)
            {
                ngram[k] = words[i + k + 1];
                ngrams.Add(ngram.ToArray());
            }

            return ngrams;
        }

        private static int GetMaxSpace(int n, int space, string[] words, int i)
        {
            while (space > 0)
            {
                if (i + n + space - 1 < words.Length)
                    return space;
                space--;
            }
            return space;
        }

        #endregion
    }
}