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

        public Ngram(int n, string s)
        {
            N = n;
            S = s;
            var words = S.Split(' ');

            for (var i = 0; i < words.Length; ++i)
            {
                switch (i)
                {
                    case 0: S1 = words[0]; break;
                    case 1: S2 = words[1]; break;
                    case 2: S3 = words[2]; break;
                    case 3: S4 = words[3]; break;
                    case 4: S5 = words[4]; break;
                }
            }
        }
    }
}