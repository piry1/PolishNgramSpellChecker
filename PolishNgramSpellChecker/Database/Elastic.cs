using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using PolishNgramSpellChecker.Model;

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

        public static bool NgramExist(string words)
        {
            // query
            var aa = new SearchDescriptor<Ngram>()
                .Index($"n{words.Split(' ').Length}grams")
                .Size(0)
                .Query(q => q
                    .MatchPhrase(c => c
                    .Field(p => p.S)
                    .Query(words))
                );

            // searching
            var result = _client.Search<Ngram>(aa);
            return result.Total > 0;
        }

        public static double NgramFuzzyMatch(string word, string[] words)
        {
            int n = words.Length + 1;
            var stringWords = ArrayToString(words);

            // query
            var aa = new Nest.SearchDescriptor<Ngram>()
                .Index($"n{n}grams")
                .Size(100)
                .Sort(a => a
                    .Descending(p => p.N))
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(x => x
                                .Field(f => f.S)
                                .Query(stringWords)
                                .Operator(Operator.And)
                            ),
                            m => m
                            .Match(x => x
                                .Field(f => f.S)
                                .Query(word)
                                .Fuzziness(Fuzziness.EditDistance(2))
                                .PrefixLength(2)
                                .FuzzyTranspositions(true)
                                .MaxExpansions(2000)
                            )
                        )
                )
                );
            // searching
            var result = _client.Search<Ngram>(aa);



            CountPercentage(result);
            return CountScore(n, result);
        }

        private static void CountPercentage(ISearchResponse<Ngram> result)
        {
            int n = 0;

            foreach (var hit in result.Hits)
                n += hit.Source.N;

            foreach (var hit in result.Hits)
            {
                Console.WriteLine((double)hit.Source.N / n + ": " + hit.Source.S);
            }


        }


        private static string ArrayToString(string[] words)
        {
            string result = string.Empty;
            foreach (var word in words)
                result += word + " ";
            return result;
        }


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
                .Index($"n{n}grams")
                .Size(1)
                .Sort(a => a
                    .Descending(p => p.N))
                .Query(q => q
                    .MatchPhrase(c => c
                        .Field(p => p.S)
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
                .Index($"n{n}grams")
                .Size(1)
                .Sort(a => a
                    .Descending(p => p.N))
                .Query(q => q
                    .Match(c => c
                        .Field(p => p.S)
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
            //double score = Math.Pow(10, n) * result.Hits.First().Source.N / 1000;
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
                        .Field(p => p.S)
                        .Operator(Operator.And)
                        .Query(words))
                );

            // searching
            var result = _client.Search<Ngram>(aa);
            return result.Total > 0;
        }

    }
}