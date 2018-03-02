using System;
using Nest;

namespace PolishNgramSpellChecker.Controller
{
    internal static class Elasticsearch
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
    }
}