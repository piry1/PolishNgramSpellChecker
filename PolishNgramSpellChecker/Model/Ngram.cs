using System.Collections.Generic;

namespace PolishNgramSpellChecker.Model
{
    internal class Ngram
    {
        public string Id { get; set; }
        public int N { get; set; } // how many times does that ngram occurs
        public List<string> V { get; set; } // words in ngram

        public string S { get; set; } // words in ngram separated by space
        public string S1 { get; set; }
        public string S2 { get; set; }
        public string S3 { get; set; }
        public string S4 { get; set; }
        public string S5 { get; set; }
    }
}