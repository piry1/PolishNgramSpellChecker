using PolishNgramSpellChecker.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Modules.Preprocessing;
namespace PolishNgramSpellChecker.Tests.Modules
{
    public static class CoverModule
    {
        static CoverModule() => Elastic.SetConnection();

        public static double Coverage(List<string[]> text, int n, bool ordered = true)
        {
            var t = text.Select(x => PreprocessingModule.Process(string.Join(" ", x)));
            var data = t.Where(x => x.Length >= n).ToList();
            int count = data.Select(x => x.Length).Sum();
            var results = data.Select(x => ProcessLine(x, n, ordered));
            return (double)results.Sum() / count;
        }

        private static int ProcessLine(string[] line, int n, bool ordered)
        {
            bool[] results = new bool[line.Length];
            bool[] tmp = new bool[n];
            for (int i = 0; i < n; ++i) tmp[i] = true;

            for (int i = 0; i <= results.Length - n; ++i)
            {
                var ngram = string.Join(" ", line.Skip(i).Take(n));
                if (Elastic.GetNgramNValue(ngram, ordered, "w") != 0)
                    Array.Copy(tmp, 0, results, i, n);
            }
            
            return results.Count(x => x == true);
        }
    }
}
