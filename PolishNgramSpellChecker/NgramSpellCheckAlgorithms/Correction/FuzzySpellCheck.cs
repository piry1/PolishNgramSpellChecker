using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using Nest;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker.NgramSpellCheckAlgorithms.Correction
{
    public class FuzzySpellCheck : ISpellCheckAlgorithm
    {
        private readonly Dictionary<string[], double[]> _results = new Dictionary<string[], double[]>();
        private readonly Dictionary<string[], Dictionary<string, double>> _memory =
            new Dictionary<string[], Dictionary<string, double>>();

        public IScResponse CheckText(string text, ISpellCheckerParams spellParams)
        {
            _results.Clear();
            _memory.Clear();
            var words = text.Trim().ToLower().Split(' ');

            for (int i = 0; i < words.Length; ++i)
                words[i] = words[i].TrimEnd('.', ',');

            var score = new double[words.Length];
            CheckRecursive(words.ToArray(), score);
            //var results = SotrtResults();

            //foreach (var result in results)
            //{
            //    Console.Write(result.Value.Sum() + " - ");
            //    foreach (var s in result.Key)
            //    {
            //        Console.Write(s + " ");
            //    }
            //    Console.Write("\n");
            //}

            return CreateScResponse(text, text.Trim().Split(' '));
        }

        private List<KeyValuePair<string[], double[]>> SotrtResults()
        {
            var results = _results.ToList();
            results.Sort((pair1, pair2) => pair1.Value.Sum().CompareTo(pair2.Value.Sum()));
            results.Reverse();
            return results;
        }

        public void CheckRecursive(string[] words, double[] score, int wordIndex = 0, int maxNgram = 3)
        {
            if (wordIndex == words.Length) // end of recursive algorithm
            {
                if (!_results.ContainsKey(words))
                    _results.Add(words, score);
                return;
            }

            var possibleWordReplacements = GetPossibleWordsReplacements(words, wordIndex, ref maxNgram);


            int n = 0;
            double minScoreSpace = 0.35;
            double minScore = possibleWordReplacements.ContainsKey(words[wordIndex])
                ? possibleWordReplacements[words[wordIndex]]
                : 0;

            score[wordIndex] = minScore;
            CheckRecursive(words.ToArray(), score.ToArray(), wordIndex + 1); // go recursive for base line

            foreach (var possibleWord in possibleWordReplacements)
            {
                if (!(possibleWord.Value > minScore + minScoreSpace)) continue;
                if (n >= 2 && minScore == 0) continue;
                score[wordIndex] = possibleWord.Value;
                words[wordIndex] = possibleWord.Key;
                CheckRecursive(words.ToArray(), score.ToArray(), wordIndex + 1); // go recursive 
                ++n;
            }
        }

        private Dictionary<string, double> GetPossibleWordsReplacements(string[] words, int wordIndex, ref int maxNgram)
        {
            Dictionary<string, double> possibleWordReplacements = new Dictionary<string, double>();
            while (maxNgram > 1 && possibleWordReplacements.Count == 0)
            {
                var probabilities = new List<Dictionary<string, double>>();
                var sideWordsList = GetSurroundingWords(words, wordIndex, maxNgram);

                foreach (var stringse in sideWordsList)
                {
                    var test = stringse.ToList();
                    test.Add(words[wordIndex]);
                    var t = test.ToArray();

                    if (!_memory.ContainsKey(t))
                    {
                        var probab = Elastic.NgramFuzzyMatch(words[wordIndex], stringse);
                        probabilities.Add(probab);
                        _memory.Add(t, probab);
                    }
                    else
                        probabilities.Add(_memory[t]);
                }

                possibleWordReplacements = MergeResults(probabilities);
                maxNgram--;
            }

            return possibleWordReplacements;
        }

        private List<string[]> GetSurroundingWords(string[] words, int pos, int max)
        {
            var result = new List<string[]>();
            var wordsInRange = new List<string>();
            var count = max - 1;
            var start = pos - count < 0 ? 0 : pos - count;
            var end = pos + max > words.Length ? words.Length : pos + max;

            for (int i = start; i < end; ++i)
            {
                if (i != pos)
                    wordsInRange.Add(words[i]);
            }

            for (int i = 0; i <= wordsInRange.Count - count; ++i)
            {
                string[] tmp = new string[count];
                for (int k = i; k < i + count; ++k)
                    tmp[k - i] = wordsInRange[k];
                result.Add(tmp);
            }

            return result;
        }

        private Dictionary<string, double> MergeResults(List<Dictionary<string, double>> probabilities)
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

        private ScResponse CreateScResponse(string text, string[] words)
        {
            var isCorrect = _results.Count == 1;
            var results = SotrtResults();
            var suggestions = GetSuggestedWords(results);
            var suggestedText = GetSuggestedText(results);
            var response = new ScResponse(text, words, isCorrect, suggestedText, suggestions);
            return response;
        }

        private List<string>[] GetSuggestedWords(List<KeyValuePair<string[], double[]>> resultDic)
        {
            if (resultDic.Count == 0) return new List<string>[0];
            List<string>[] results = new List<string>[resultDic.First().Key.Length];

            for (int x = 0; x < results.Length; ++x)
            {
                results[x] = new List<string>();
                for (int i = 0; i < resultDic.Count; ++i)
                {
                    var word = resultDic[i].Key[x];
                    if (!results[x].Contains(word) /*&& resultDic.Last().Key[x] != word*/)
                        results[x].Add(word);
                }
            }

            return results;
        }

        private List<string> GetSuggestedText(List<KeyValuePair<string[], double[]>> resultDic)
        {
            List<string> result = new List<string>();
            foreach (var item in resultDic)
                result.Add(ArrayToString(item.Key));
            result.Remove(result.Last());
            return result;
        }

        private string ArrayToString(string[] words)
        {
            string result = string.Empty;
            foreach (var word in words)
                result += word + " ";
            result = result.TrimEnd();
            return result;
        }
    }
}