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

        public class CoverageResult
        {
            public int N { get; set; }
            public double CoveragePercent { get; set; }
            public double Ncoverage { get; set; }
        }

        public static CoverageResult Coverage(List<string[]> text, int n, bool ordered = true, int minCount = 0)
        {
            if (n < 1 || n > 5)
                throw new Exception($"Wrong N-gram count: N = {n}");

            var t = text.Select(x => PreprocessingModule.Process(string.Join(" ", x)));
            var data = t.Where(x => x.Length >= n).ToList();
            int count = data.Select(x => x.Length).Sum();
            var results = data.Select(x => n == 1 ?
                ProcessUnigramsLine(x, minCount) :
                ProcessLine(x, n, ordered, minCount));

            return new CoverageResult
            {
                N = n,
                CoveragePercent = (double)results.Sum() / count,
                Ncoverage = (double)data.Select(x => x.Length).Sum() / (double)text.Select(x => x.Length).Sum()
            };
        }

        private static int ProcessLine(string[] line, int n, bool ordered, int minCount)
        {
            bool[] results = new bool[line.Length];
            bool[] tmp = new bool[n];
            for (int i = 0; i < n; ++i) tmp[i] = true;

            for (int i = 0; i <= results.Length - n; ++i)
            {
                var ngram = string.Join(" ", line.Skip(i).Take(n));
                if (Elastic.GetNgramNValue(ngram, ordered, "w") >= minCount)
                    Array.Copy(tmp, 0, results, i, n);
            }

            return results.Count(x => x == true);
        }

        private static int ProcessUnigramsLine(string[] line, int minScore)
        {
            bool[] results = new bool[line.Length];
            for (int i = 0; i < results.Length; ++i)
            {
                var res = Elastic.CheckWord(line[i]);
                results[i] = res >= minScore;
            }
            return results.Count(x => x == true);
        }
    }
}
