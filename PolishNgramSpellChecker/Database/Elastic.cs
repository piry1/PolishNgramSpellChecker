using System;
using System.Collections.Generic;
using System.Linq;
using Nest;

namespace PolishNgramSpellChecker.Database
{
    internal static class Elastic
    {
        private static ElasticClient _client;
        public static string Url { get; private set; } = "http://localhost:9200";

        public static void SetConnection(string url = null)
        {
            if (url != null) Url = url;
            var node = new Uri(Url);
            var settings = new ConnectionSettings(node);
            //settings.DefaultIndex(indexName);
            _client = new ElasticClient(settings);
        }

        public static bool NgramExist(string words, bool ordered)
        {
            // query
            var aa = new SearchDescriptor<Ngram>()
                .Index($"mn{words.Split(' ').Length}grams")
                .Size(0)
                .Query(q => q
                    .Match(c => c
                    .Field(p => p.w)
                    .Query(words))
                );

            // searching
            var result = _client.Search<Ngram>(aa);
            return result.Total > 0;
        }
        
        #region FUZZY DETECTION AND CORRECTION 
        
        // FUZZY MAIN
        public static Dictionary<string, double> NgramFuzzyMatch(int idx, string[] words, bool ordered, string method)
        {
            if (!ordered)
            {
                var word = words[idx];
                List<string> sWords = new List<string>();
                for (int i = 0; i < words.Length; ++i)
                    if (i != idx)
                        sWords.Add(words[i]);

                return NgramNoOrderedFuzzyMatch(word, sWords.ToArray(), method);
            }
            else
                return NgramOrderedFuzzyMatch(idx, words, method);
        }

        public static Dictionary<string, double> NgramOrderedFuzzyMatch(int idx, string[] words, string method)
        {
            var n = words.Length;
            string searchedWord = words[idx];
            Dictionary<int, string> surWords = new Dictionary<int, string>();

            for (int i = 0; i < 5; ++i)
            {
                if (i < n)
                {
                    if (i != idx)
                        surWords.Add(i + 1, words[i]);
                }
                else
                    surWords.Add(i + 1, null);
            }

            #region QUERY
            var query = new Nest.SearchDescriptor<Ngram>()
                .Index($"sn{n}grams")
                .Size(100)
                .Sort(a => a
                    .Descending(p => p.N))
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(x => x
                                .Field($"w{surWords.ElementAt(0).Key}")
                                .Query(surWords.ElementAt(0).Value)
                                .Operator(Operator.And)
                            ),
                            m => m
                              .Match(x => x
                                .Field($"w{surWords.ElementAt(1).Key}")
                                .Query(surWords.ElementAt(1).Value)
                                .Operator(Operator.And)
                            ),
                              m => m
                              .Match(x => x
                                .Field($"w{surWords.ElementAt(2).Key}")
                                .Query(surWords.ElementAt(2).Value)
                                .Operator(Operator.And)
                            ),
                                m => m
                              .Match(x => x
                                .Field($"w{surWords.ElementAt(3).Key}")
                                .Query(surWords.ElementAt(3).Value)
                                .Operator(Operator.And)
                            ),
                            m => m
                            .Match(x => x
                                .Field($"{method}{idx + 1}")
                                .Query(searchedWord)
                                .Fuzziness(Fuzziness.Auto)
                                .PrefixLength(1)
                                .FuzzyTranspositions(true)
                                .MaxExpansions(1000)
                            )
                        )
                )
                );

            #endregion

            var result = _client.Search<Ngram>(query);
            return CountPercentage(idx, result);
        }

        public static Dictionary<string, double> NgramNoOrderedFuzzyMatch(string word, string[] words, string method)
        {
            var n = words.Length + 1;
            var stringWords = ArrayToString(words);
            #region QUERY
            var query = new Nest.SearchDescriptor<Ngram>()
                .Index($"mn{n}grams")
                .Size(100)
                .Sort(a => a
                    .Descending(p => p.N))
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(x => x
                                .Field(f => f.w)
                                .Query(stringWords)
                                .Operator(Operator.And)
                            ),
                            m => m
                            .Match(x => x
                                .Field(method)
                                .Query(word)
                                .Fuzziness(Fuzziness.Auto)
                                .PrefixLength(1)
                                .FuzzyTranspositions(true)
                                .MaxExpansions(1000)
                            )
                        )
                )
                );
            #endregion   
            var result = _client.Search<Ngram>(query);
            return CountPercentage(result, words);
        }

        private static Dictionary<string, double> CountPercentage(int idx, ISearchResponse<Ngram> searchResponse)
        {
            var results = new Dictionary<string, double>();
            var n = searchResponse.Hits.Sum(hit => hit.Source.N);

            foreach (var hit in searchResponse.Hits)
            {
                string word = hit.Source.GetType().GetProperty($"w{idx + 1}").GetValue(hit.Source, null) as string;
                word = word.Trim('.');
                double probability = (double)hit.Source.N / n;

                if (word != string.Empty)
                {
                    if (!results.ContainsKey(word))
                        results.Add(word, probability);
                    else
                        results[word] += probability;
                }
            }

            return results;
        }

        private static Dictionary<string, double> CountPercentage(ISearchResponse<Ngram> searchResponse, string[] words)
        {
            var results = new Dictionary<string, double>();
            var n = searchResponse.Hits.Sum(hit => hit.Source.N);

            foreach (var hit in searchResponse.Hits)
            {
                var w = hit.Source.w;
                for (int i = 0; i < w.Count; ++i)
                    w[i] = w[i].Trim('.');
                var word = string.Empty;

                foreach (var s in w)
                {
                    if (words.Contains(s)) continue;
                    word = s;
                    break;
                }

                double probability = (double)hit.Source.N / n;

                if (word != string.Empty)
                {
                    if (!results.ContainsKey(word))
                        results.Add(word, probability);
                    else
                        results[word] += probability;
                }
            }

            return results;
        }

        #endregion

        private static string ArrayToString(string[] words)
        {
            string result = string.Empty;
            foreach (var word in words)
                result += word + " ";
            return result;
        }

        // DETECTION MAIN
        public static double NgramNvalue(string text, bool ordered = true)
        {
            if (ordered)
                return NgramValue(text);
            else
                return NgramNoOrderValue(text);
        }

        public static double NgramValue(string words)
        {
            double n = words.Split(' ').Length;
            // query
            var aa = new SearchDescriptor<Ngram>()
                .Index($"sn{n}grams")
                .Size(1)
                .Sort(a => a
                    .Descending(p => p.N))
                .Query(q => q
                    .MatchPhrase(c => c
                        .Field(p => p.s)
                        .Query(words))
                );
            // searching
            var result = _client.Search<Ngram>(aa);
            return CountScore(n, result);
        }

        public static double NgramNoOrderValue(string words)
        {
            double n = words.Trim().Split(' ').Length;
            // query
            var aa = new SearchDescriptor<Ngram>()
                .Index($"mn{n}grams")
                .Size(1)
                .Sort(a => a
                    .Descending(p => p.N))
                .Query(q => q
                    .Match(c => c
                        .Field(p => p.w)
                        .Operator(Operator.And)
                        .Query(words))
                );

            // searching
            var result = _client.Search<Ngram>(aa);
            return CountScore(n, result);
        }

        private static double CountScore(double n, ISearchResponse<Ngram> result)
        {
            if (result.Total == 0)
                return 0;
            //double score = Math.Pow(10, n) * searchResponse.Hits.First().Source.N / 1000;
            //return score;
            return result.Hits.First().Source.N;
        }

        public static bool NgramNoOrderExist(string words)
        {
            // query
            var aa = new SearchDescriptor<Ngram>()
                .Index($"n{words.Split(' ').Length}grams")
                .Size(0)
                .Query(q => q
                    .Match(c => c
                        .Field(p => p.s)
                        .Operator(Operator.And)
                        .Query(words))
                );

            // searching
            var result = _client.Search<Ngram>(aa);
            return result.Total > 0;
        }

    }
}