using System;
using System.Collections.Generic;
using System.Linq;
using LazyCache;
using Nest;

namespace PolishNgramSpellChecker.Database
{
    public static class Elastic
    {
        private static ElasticClient _client;
        private static IAppCache _cache = new CachingService();

        private static string _orderedSearchIndexPrefix = "ss";
        private static string _noOrderedSearchIndexPrefix = "mm";

        public static string Url { get; private set; } = "http://localhost:9200";

        public static void SetConnection(string url = null)
        {
            if (url != null) Url = url;
            var node = new Uri(Url);
            var settings = new ConnectionSettings(node);
            //settings.DefaultIndex(indexName);
            _client = new ElasticClient(settings);
        }

        #region UNIGRAMS

        public static int CheckWord(string word)
        {
            var query = new Nest.SearchDescriptor<Ngram>()
               .Index("unigrams")
               .Size(1)            
               .Query(q => q
                   .Match(x => x
                       .Field("w1")
                       .Query(word)
                   )
               );
            var result = _client.Search<Ngram>(query);
            return result.Hits.Count() == 0 ? 0 : result.Hits.First().Source.N;
        }

        public static Dictionary<string, int> GetSimilarWords(string word, string method, Fuzziness fuzziness, int prefixLength = 1)
        {
            string key = $"GetSimilarWords+{word}+{method}+{fuzziness}+{prefixLength}";
            return _cache.GetOrAdd(key, () => SimilarWords(word, method, fuzziness, prefixLength));
        }

        private static Dictionary<string, int> SimilarWords(string word, string method, Fuzziness fuzziness, int prefixLength)
        {
            #region QUERY
            var query = new Nest.SearchDescriptor<Ngram>()
                .Index("unigrams")
                .Size(100)
                .Sort(a => a
                    .Ascending(p => p.N))
                .Query(q => q
                    .Match(x => x
                        .Field($"{method}1")
                        .Query(word)
                        .Fuzziness(fuzziness)
                        .PrefixLength(prefixLength)
                        .FuzzyTranspositions(true)
                        .MaxExpansions(1000)
                    )
                );
            #endregion

            var result = _client.Search<Ngram>(query);

            var results = new Dictionary<string, int>();

            foreach (var hit in result.Hits)
            {
                string similarWord = hit.Source.w1;
                similarWord = similarWord.Trim('.');

                if (word == string.Empty || word == similarWord) continue;

                if (!results.ContainsKey(similarWord))
                    results.Add(similarWord, hit.Source.N);
                else
                    results[similarWord] += hit.Source.N;
            }

            return results;
        }

        #endregion

        #region FUZZY DETECTION AND CORRECTION 

        // FUZZY MAIN
        public static Dictionary<string, double> GetSimilarWords(int idx, string[] words, bool ordered, string method)
        {
            string key = $"NgramFuzzyMatch+{idx}+{string.Join(" ", words)}+{ordered}+{method}";

            var result = _cache.GetOrAdd(key, () => ordered ?
                NgramFuzzyMatch(idx, words, method)
                : NgramFuzzyMatch_no(idx, words, method));

            return result;
        }

        private static Dictionary<string, double> NgramFuzzyMatch(int idx, string[] words, string method)
        {
            var n = words.Length;
            string searchedWord = words[idx];
            Dictionary<int, string> surWords = new Dictionary<int, string>();

            string field = method == "dd" ? "d" : "w";
            method = method == "dd" ? "d" : method;

            for (int i = 0; i < 5; ++i)
            {
                if (i != idx)
                    surWords.Add(i + 1, i < n ? words[i] : null);
                //if (i < n)
                //{
                //    if (i != idx)
                //        surWords.Add(i + 1, words[i]);
                //}
                //else
                //    surWords.Add(i + 1, null);
            }

            #region QUERY
            var query = new Nest.SearchDescriptor<Ngram>()
                .Index($"{_orderedSearchIndexPrefix}{n}grams")
                .Size(100)
                .Sort(a => a
                    .Descending(p => p.N))
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(x => x
                                .Field($"{field}{surWords.ElementAt(0).Key}")
                                .Query(surWords.ElementAt(0).Value)
                                .Operator(Operator.And)
                            ),
                            m => m
                              .Match(x => x
                                .Field($"{field}{surWords.ElementAt(1).Key}")
                                .Query(surWords.ElementAt(1).Value)
                                .Operator(Operator.And)
                            ),
                              m => m
                              .Match(x => x
                                .Field($"{field}{surWords.ElementAt(2).Key}")
                                .Query(surWords.ElementAt(2).Value)
                                .Operator(Operator.And)
                            ),
                                m => m
                              .Match(x => x
                                .Field($"{field}{surWords.ElementAt(3).Key}")
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

        private static Dictionary<string, double> NgramFuzzyMatch_no(int idx, string[] words, string method)
        {
            string field = method == "dd" ? "d" : "w";
            method = method == "dd" ? "d" : method;
            var word = words[idx];
            List<string> sWords = words.ToList();
            sWords.RemoveAt(idx);
            var n = words.Length;
            var stringWords = string.Join(" ", sWords);
            #region QUERY
            var query = new Nest.SearchDescriptor<Ngram>()
                .Index($"{_noOrderedSearchIndexPrefix}{n}grams")
                .Size(100)
                .Sort(a => a
                    .Descending(p => p.N))
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(x => x
                                .Field(field)
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
            return CountPercentage_no(result, words);
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

        private static Dictionary<string, double> CountPercentage_no(ISearchResponse<Ngram> searchResponse, string[] words)
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

        #region DETECTION MAIN

        public static double GetNgramNValue(string text, bool ordered, string method)
        {
            string key = $"NgramValue:{text}+{ordered}";
            return _cache.GetOrAdd(key, () => NgramNValue(text, ordered, method));
        }

        private static double NgramNValue(string words, bool ordered, string method)
        {
            int n = words.Split(' ').Length;

            var query = ordered ?
                NgramValueQuery(n, words, method)
                : NgramValue_no_Query(n, words, method);

            var result = _client.Search<Ngram>(query);
            return result.Total == 0 ? 0 : result.Hits.First().Source.N;
        }

        #region Queries

        private static SearchDescriptor<Ngram> NgramValueQuery(int n, string text, string method)
        {
            var words = text.Split().ToList();
            while (words.Count < 5)
                words.Add(null);

            var query = new SearchDescriptor<Ngram>()
              .Index($"{_orderedSearchIndexPrefix}{n}grams")
              .Size(1)
              .Sort(a => a
                  .Descending(p => p.N))
              .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(x => x
                                .Field($"{method}1")
                                .Query(words[0])
                                .Operator(Operator.And)
                            ),
                            m => m
                              .Match(x => x
                                .Field($"{method}2")
                                .Query(words[1])
                                .Operator(Operator.And)
                            ),
                              m => m
                              .Match(x => x
                                .Field($"{method}3")
                                .Query(words[2])
                                .Operator(Operator.And)
                            ),
                                m => m
                              .Match(x => x
                                .Field($"{method}4")
                                .Query(words[3])
                                .Operator(Operator.And)
                            ),
                            m => m
                            .Match(x => x
                                .Field($"{method}5")
                                .Query(words[4])
                                .Operator(Operator.And)
                            )
                        )
                )
              );
            return query;
        }

        private static SearchDescriptor<Ngram> NgramValue_no_Query(int n, string words, string method)
        {
            var query = new SearchDescriptor<Ngram>()
               .Index($"{_noOrderedSearchIndexPrefix}{n}grams")
               .Size(1)
               .Sort(a => a
                   .Descending(p => p.N))
               .Query(q => q
                   .Match(c => c
                       .Field(method)
                       .Operator(Operator.And)
                       .Query(words))
               );
            return query;
        }

        #endregion

        #endregion
    }
}