using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker.NgramSpellCheckAlgorithms
{
    public class FuzzySpellCheck : ISpellCheckAlgorithm
    {
        private Dictionary<string[], double> Results = new Dictionary<string[], double>();

        public IScResponse CheckText(string text, ISpellCheckerParams spellParams)
        {
            var words = text.Trim().Split(' ');
            var score = new double[words.Length];
            var jointsScore = new double[words.Length - 1];
            Results.Clear();
            Check(words, score);
            // jointsScore = Check(words, jointsScore, spellParams);

            var myList = Results.ToList();

            myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            myList.Reverse();

            foreach (var result in myList)
            {
                Console.Write(result.Value + " - ");
                foreach (var s in result.Key)
                {
                    Console.Write(s + " ");
                }
                Console.Write("\n");

            }
            var isCorrect = true;// IsSentenceCorrect(jointsScore);
            return new ScResponse(text, words, isCorrect, jointsScore, new List<string>());
        }

        public void Check(string[] words, double[] score, int pos = 0, int max = 3)
        {
            if (pos == words.Length)
            {
                if (!Results.ContainsKey(words))
                    Results.Add(words, score.Sum());
                return;
            }

            var sideWords = new List<string>();
            var prob = new Dictionary<string, double>();


            while (max > 1 && prob.Count == 0)
            {           
                var sideWordsList = GetSideWords(words, pos, max);
                List<Dictionary<string, double>> probabilities = new List<Dictionary<string, double>>();

                foreach (var stringse in sideWordsList)                
                    probabilities.Add(Elastic.NgramFuzzyMatch(words[pos], stringse));
                          
                prob = MergeProb(probabilities);
                max--;
            }

            double minScore = 0;
            if (prob.ContainsKey(words[pos]))
                minScore = prob[words[pos]];

            score[pos] = minScore;
            Check(words.ToArray(), score.ToArray(), pos + 1);

            int n = 0;
            foreach (var p in prob)
            {
                if (p.Value > minScore + 0.35)
                {
                    if (n < 2)
                    {
                        score[pos] = p.Value;
                        words[pos] = p.Key;
                        Check(words.ToArray(), score.ToArray(), pos + 1);
                    }

                    if (minScore == 0)
                        ++n;
                }            
            }

        }

        private List<string[]> GetSideWords(string[] words, int pos, int max)
        {
            List<string[]> result = new List<string[]>();
            var count = max - 1;
            var start = pos - count < 0 ? 0 : pos - count;
            var end = pos + max > words.Length ? words.Length : pos + max;

            List<string> wwords = new List<string>();

            for (int i = start; i < end; ++i)
            {
                if (i != pos)
                    wwords.Add(words[i]);
            }

            for (int i = 0; i <= wwords.Count - count; ++i)
            {
                string[] tmp = new string[count];
                for (int k = i; k < i + count; ++k)
                    tmp[k - i] = wwords[k];
                result.Add(tmp);
            }

            return result;
        }

        private Dictionary<string, double> MergeProb(List<Dictionary<string, double>> probabilities)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            foreach (var dictionary in probabilities)
            {
                foreach (var word in dictionary)
                {
                    if (!result.ContainsKey(word.Key))
                        result.Add(word.Key, word.Value);
                    else
                        result[word.Key] += word.Value;
                }
            }


            for (int i = 0; i < result.Count; ++i)
            {
                int n = 0;
                foreach (var dictionary in probabilities)
                {
                    if (dictionary.ContainsKey(result.Keys.ElementAt(i)))
                        n++;
                }
                //   if (n != probabilities.Count)
                result[result.Keys.ElementAt(i)] /= n;
            }

            return result;
        }

        private ScResponse setScResponse(string[] words, List<Dictionary<string, double>> probabilities)
        {
            var score = new double[words.Length];

            for (int i = 0; i < words.Length; ++i)
            {
                if (probabilities[i].ContainsKey(words[i]))
                    score[i] = (probabilities[i])[words[i]];
                else
                    score[i] = 0;
            }

            foreach (var d in score)
                Console.WriteLine(d);

            return null;
        }

    }
}