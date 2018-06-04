using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Modules.Orthography;

namespace PolishNgramSpellChecker.Modules.AfterCorrection
{
    internal static class AfterCorrectionModule
    {
        private static OrthographyModule ortModule = new OrthographyModule();

        public static IScResponse Check(IScResponse response)
        {
            for (int i = 0; i < response.Words.Length; ++i)
            {
                if (!response.IsWordCorrect[i]) continue;
                if (ortModule.IsCorrect(response.Words[i])) continue;
                var res = CheckForRepleacement(response.Words[i]);
                if (res.Count() == 0) continue;
                response.WordsSugestions[i] = res;
                response.IsWordCorrect[i] = false;
            }

            return response;
        }

        private static Dictionary<string, double> CheckForRepleacement(string word)
        {
            var suggestions = new Dictionary<string, double>();
            var res = Elastic.GetSimilarWords(word, "d", Nest.Fuzziness.EditDistance(0), 0).ToList();
            res.Reverse();
            double max = res.Sum(x => x.Value);
            res.ForEach(x => suggestions.Add(x.Key, (double)x.Value / max));
            return suggestions;
        }

    }

}
