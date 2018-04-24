using System.Collections.Generic;
using System.Linq;

namespace PolishNgramSpellChecker.Model
{
    internal class Ngram
    {
        public string Id { get; set; }
        public int N { get; set; } // how many times does that ngram occurs

        public List<string> w { get; set; } // words i ngram
        public List<string> f { get; set; } // phonetic
        public List<string> p { get; set; } // polish stempel

        public string s { get; set; } // words in ngram separated by space
        public string w1 { get; set; }
        public string w2 { get; set; }
        public string w3 { get; set; }
        public string w4 { get; set; }
        public string w5 { get; set; }

        public string f1 { get; set; }
        public string f2 { get; set; }
        public string f3 { get; set; }
        public string f4 { get; set; }
        public string f5 { get; set; }

        public string p1 { get; set; }
        public string p2 { get; set; }
        public string p3 { get; set; }
        public string p4 { get; set; }
        public string p5 { get; set; }

        //public Ngram(int n, string s)
        //{
        //    N = n;
        //    this.s = s;
        //    var words = this.s.Split(' ');

        //    for (var i = 0; i < words.Length; ++i)
        //    {
        //        switch (i)
        //        {
        //            case 0: w1 = f1 = p1 = words[0]; break;
        //            case 1: w2 = f2 = p2 = words[1]; break;
        //            case 2: w3 = f3 = p3 = words[2]; break;
        //            case 3: w4 = f4 = p4 = words[3]; break;
        //            case 4: w5 = f5 = p5 = words[4]; break;
        //        }
        //    }
        //}

        //public Ngram(int n, List<string> v)
        //{
        //    N = n;
        //    w = v;
        //    f = v.ToList();
        //    p = v.ToList();
        //}

        public override string ToString()
        {
            string result = N.ToString();
            foreach (var word in w)
                result += " " + word;
            return result;
        }
    }
}
