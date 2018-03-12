using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Detection
{
    internal class MultiNgramDetection : ISpellCheckAlgorithm
    {
        public MultiNgramDetection()
        {
           // Elastic.SetConnection();
        }

        public IScResponse CheckText(string text, ISpellCheckerParams spellParams)
        {
            var words = text.Trim().Split(' ');
            var jointsScore = new double[words.Length - 1];
            jointsScore = Check2(words, jointsScore, spellParams);
            var isCorrect = IsSentenceCorrect(jointsScore);
            return new ScResponse(text, words, isCorrect, jointsScore, new List<string>());
           // return Check(text, spellParams);
        }

        private double[] Check2(string[] words, double[] jointsScore, ISpellCheckerParams spellParams)
        {
            SimpleNgramDetection snd = new SimpleNgramDetection();
            for (int i = spellParams.MaxN; i >= spellParams.MinN; --i)
            {
                spellParams.N = i;
                jointsScore = snd.Check(words, jointsScore, spellParams);
            }

            return jointsScore;
        }

        private bool IsSentenceCorrect(double[] jointsScore)
        {
            return !jointsScore.Contains(0);
        }


        private IScResponse Check(string text, ISpellCheckerParams spellParams, int max = 4)
        {
            var words = text.Trim().Split(' ');
            List<int> errorIndexes = new List<int>();
            double[] jointsScore = new double[words.Length - 1];
            //bool lastResult = true;

            for (int i = 0; i < words.Length - 1; ++i)
            {
                for (int k = max; k > 0; --k)
                {
                    if (i + k < words.Length)
                    {
                        string data = "";
                        for (int z = i; z <= i + k; ++z)
                            data += words[z] + " ";
                        var score = Elastic.NgramNvalue(data.Trim(), spellParams.OrderedMatch);
                        SetJointsScore(ref jointsScore, i, k, score);
                    }
                }
            }

            bool finalResult = true;
            foreach (var score in jointsScore)
                if (score == 0)
                {
                    finalResult = false;
                    break;
                }

            return new ScResponse(text, words, finalResult, jointsScore, new List<string>());
        }

        private static void SetJointsScore(ref double[] jointsScore, int start, int count, double score)
        {
            for (int i = start; i < count + start; ++i)
                if (jointsScore[i] < score)
                    jointsScore[i] = score;
        }
    }
}