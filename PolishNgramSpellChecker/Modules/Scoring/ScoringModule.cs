using System;
using System.Collections.Generic;
using System.Linq;
using PolishNgramSpellChecker.Params.Interfaces;

namespace PolishNgramSpellChecker.Modules.Scoring
{
    internal class ScoringModule
    {
        public double[] Score(string[] words, IScoringParams scoringParams)
        {
            var jointsScore = new double[words.Length - 1];

            jointsScore = scoringParams.ScoreMulti
                ? CheckMulti(words, jointsScore, scoringParams)
                : Check(words, jointsScore, scoringParams);

            return CountWordsScore(jointsScore);
        }

        private double[] Check(string[] words, double[] jointsScore, IScoringParams scoringParams, int? N = null)
        {
            int n = N ?? scoringParams.MinN;
            for (int i = 0; i < words.Length + 1 - n; ++i)
            {
                string sentence = string.Empty;
                for (int j = i; j < i + n; ++j) // first order
                    sentence += words[j] + " ";

                sentence = sentence.TrimEnd();
                var nCount = Database.Elastic.NgramNvalue(sentence, scoringParams.OrderedMatch);
                var score = scoringParams.ScoreCountFunc(nCount, n);
                SetJointsScore(ref jointsScore, i, n - 1, score);
            }

            return jointsScore;
        }

        private double[] CheckMulti(string[] words, double[] jointsScore, IScoringParams scoringParams)
        {
            for (int i = scoringParams.MaxN; i >= scoringParams.MinN; --i)
                jointsScore = Check(words, jointsScore, scoringParams, i);

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

        private double[] CountWordsScore(double[] jointsScore)
        {
            var wordsScore = new double[jointsScore.Length + 1];
            if (wordsScore.Length == 1)
                return null;

            for (int i = 0; i < wordsScore.Length; ++i)
            {
                if (i == 0)
                    wordsScore[i] = jointsScore[i];
                else if (i == wordsScore.Length - 1)
                    wordsScore[i] = jointsScore.Last();
                else
                    wordsScore[i] = (jointsScore[i - 1] + jointsScore[i]) / 2;
            }

            return wordsScore;
        }
    }
}