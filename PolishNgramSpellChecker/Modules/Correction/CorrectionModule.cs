using System;
using System.Collections.Generic;
using System.Linq;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Params.Interfaces;

namespace PolishNgramSpellChecker.Modules.Correction
{
    public class CorrectionModule
    {
        private readonly Dictionary<string[], double[]> _results = new Dictionary<string[], double[]>();

        public IScResponse CheckText(string[] words, ICorrectionParams spellParams, bool[] sholudSkip = null)
        {
            _results.Clear();

            bool[] skip = sholudSkip ?? new bool[words.Length];
            var score = new double[words.Length];

            if (spellParams.Recursive)
            {
                CheckRecursive(words.ToArray(), score, 0, spellParams);
                return CreateScResponseForRecorsiveCheck(words);
            }
            else
            {
                var res = Check(words.ToArray(), score, skip, spellParams);
                return CreateScResponseForIterationCheck(words, res);
            }
        }

        private List<KeyValuePair<string[], double[]>> SotrtResults()
        {
            var results = _results.ToList();
            results.Sort((pair1, pair2) => pair1.Value.Sum().CompareTo(pair2.Value.Sum()));
            results.Reverse();
            return results;
        }

        #region CHECK BY ITERATIONS

        private Dictionary<string, double>[] Check(string[] words, double[] score, bool[] skip, ICorrectionParams spellParams)
        {
            var results = new Dictionary<string, double>[words.Length];

            for (int i = 0; i < words.Length; ++i)
            {
                if (skip[i])
                {
                    score[i] = 1;
                    results[i] = new Dictionary<string, double>();
                    results[i].Add(words[i], score[i]);
                    continue;
                }
                var suggestions = GetSuggestions(words, i, spellParams);    // get suggestions for this word
                results[i] = FillResultsI(words[i], suggestions,            // and fill them in results dictionary 
                    spellParams.MinScoreSpace, out score[i]);
            }

            return results;
        }

        // fill results for iteration method
        private Dictionary<string, double> FillResultsI(string word, Dictionary<string, double> suggestions, double minScoreSpace, out double score)
        {
            var results = new Dictionary<string, double>();

            double minScore = score = suggestions.ContainsKey(word)     // get score for start word
                 ? suggestions[word] : 0;

           // int n = 0;
            foreach (var possibleWord in suggestions)                   // add suggestions 
                if ((possibleWord.Value > minScore + minScoreSpace)     // if suggestion score is high enough
                                                                        /*&& (n <= 3 || minScore != 0)*/)
                {
                    results.Add(possibleWord.Key, possibleWord.Value);
                    //  ++n;
                }
            results.Add(word, minScore);                                // and add start word at the end

            return results;
        }

        #endregion

        public void CheckRecursive(string[] words, double[] score, int wordIndex, ICorrectionParams spellParams)
        {
            if (wordIndex == words.Length) // end of recursive algorithm
            {
                if (!_results.ContainsKey(words))
                    _results.Add(words, score);
                return;
            }

            var possibleWordReplacements = GetSuggestions(words, wordIndex, spellParams);

           // int n = 0;
            double minScore = possibleWordReplacements.ContainsKey(words[wordIndex])
                ? possibleWordReplacements[words[wordIndex]]
                : 0;

            score[wordIndex] = minScore;
            CheckRecursive(words.ToArray(), score.ToArray(), wordIndex + 1, spellParams); // go recursive for base line

            foreach (var possibleWord in possibleWordReplacements)
            {
                if (!(possibleWord.Value > minScore + spellParams.MinScoreSpace)) continue;
                //  if (n >= 2 && minScore == 0) continue;
                score[wordIndex] = possibleWord.Value;
                words[wordIndex] = possibleWord.Key;
                CheckRecursive(words.ToArray(), score.ToArray(), wordIndex + 1, spellParams); // go recursive 
                                                                                              //  ++n;
            }
        }

        // Get suggestions for word at wordIndex
        private Dictionary<string, double> GetSuggestions(string[] words, int wordIndex, ICorrectionParams spellParams)
        {
            Dictionary<string, double> results = new Dictionary<string, double>();

            for (int n = spellParams.MaxN; n > 1 && results.Count == 0; n--)    // search from biggest n-gram to smallest
            {
                var nGrams = GetSurroundingWords(words, wordIndex, n);          // get n-grams with WORD at different positions
                var suggestionsList = GetSuggestions(nGrams, spellParams);      // get suggestions using these n-grams
                results = MergeResults(suggestionsList);                        // merge results to single dictionary with suggestions
            }

            return results;
        }

        // get suggestions for word by n-grams with this word at different positions
        private List<Dictionary<string, double>> GetSuggestions(List<KeyValuePair<int, string[]>> nGrams, ICorrectionParams spellParams)
        {
            var suggestionsList = new List<Dictionary<string, double>>();
            foreach (var nGram in nGrams)
            {
                var suggestions = Elastic.GetSimilarWords(nGram.Key, nGram.Value, spellParams.OrderedMatch, spellParams.CorrectionMethod);
                suggestionsList.Add(suggestions);
            }
            return suggestionsList;
        }

        private List<KeyValuePair<int, string[]>> GetSurroundingWords(string[] words, int pos, int max)
        {
            var result = new List<KeyValuePair<int, string[]>>();

            var count = max - 1;
            var start = pos - count < 0 ? 0 : pos - count;
            var end = pos + max > words.Length ? words.Length : pos + max;

            var wordsInRange = new List<string>();

            int pom = 0;
            for (int i = start; i < end; ++i)
            {
                wordsInRange.Add(words[i]);
                if (pos == i)
                    pom = wordsInRange.Count - 1;
            }
            pos = pom;

            for (int i = 0; i <= wordsInRange.Count - max; ++i)
            {
                string[] tmp = new string[max];
                for (int k = i; k < i + max; ++k)
                    tmp[k - i] = wordsInRange[k];
                result.Add(new KeyValuePair<int, string[]>(pos - i, tmp));
            }

            return result;
        }

        private Dictionary<string, double> MergeResults(List<Dictionary<string, double>> suggestions)
        {
            var result = new Dictionary<string, double>();

            foreach (var dictionary in suggestions)
            {
                foreach (var word in dictionary)
                {
                    if (!result.ContainsKey(word.Key))
                        result.Add(word.Key, word.Value);
                    else
                        result[word.Key] += word.Value;
                }
            }

            int n = suggestions.Count;
            for (int i = 0; i < result.Count; ++i)
                result[result.Keys.ElementAt(i)] /= n;

            return result;
        }

        private ScResponse CreateScResponseForRecorsiveCheck(string[] words)
        {
            var isCorrect = _results.Count == 1;
            var results = SotrtResults();
            var suggestions = GetSuggestedWords(results);
            var suggestedText = GetSuggestedText(results);
            throw new NotImplementedException();
            //var response = new ScResponse(words, isCorrect, suggestedText, suggestions);
            //return response;
        }

        private ScResponse CreateScResponseForIterationCheck(string[] words, Dictionary<string, double>[] suggestions)
        {
            bool[] isCorrect = new bool[words.Length];
            for (int i = 0; i < words.Length; ++i)
                isCorrect[i] = suggestions[i].Keys.Count() == 1;

            var response = new ScResponse(words, isCorrect, null, suggestions);
            return response;
        }

        private Dictionary<string, double>[] GetSuggestedWords(List<KeyValuePair<string[], double[]>> resultDic)
        {
            if (resultDic.Count == 0) return new Dictionary<string, double>[0];
            Dictionary<string, double>[] results = new Dictionary<string, double>[resultDic.First().Key.Length];

            for (int x = 0; x < results.Length; ++x)
            {
                results[x] = new Dictionary<string, double>();
                foreach (var dic in resultDic)
                {
                    var word = dic.Key[x];
                    var value = dic.Value[x];

                    if (!results[x].ContainsKey(word) /*&& resultDic.Last().Key[x] != word*/)
                        results[x].Add(word, value);
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