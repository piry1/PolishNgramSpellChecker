using PolishNgramSpellChecker.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNgramSpellChecker.Tests.Modules
{
    public static class FinalTests
    {
        internal static void CompareMethodsCorrection(string fileName, MisspellsGenerationModule.MistakeType mistake, int MinN, int MaxN)
        {
            string paramName = nameof(SpellCheckerParams.MinScoreSpace);
            SpellCheckerParams param = new SpellCheckerParams()
            {
                ScoreMulti = false,
                DetectionMethod = "w",
                MaxN = MaxN,
                MinN = MinN,
                OrderedMatch = true,
                MinScoreSpace = 0.0,
                CorrectionMethod = "dd",
                MinPoints = 0.1,
                UseDetection = false,
                ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN),
                Fuzziness = Nest.Fuzziness.EditDistance(0)
            };
            var param2 = param.GetCopy();
            var param3 = param.GetCopy();
            var param4 = param.GetCopy();

            param2.CorrectionMethod = "f";
            param3.CorrectionMethod = "d"; param3.Fuzziness = Nest.Fuzziness.EditDistance(0); // <= ISTOTNE
            param4.CorrectionMethod = "dd"; param4.Fuzziness = Nest.Fuzziness.EditDistance(0); // <= ISTOTNE

            var param5 = param.GetCopy();
            var param6 = param2.GetCopy();
            var param7 = param3.GetCopy();
            var param8 = param4.GetCopy();

            param5.OrderedMatch = false;
            param6.OrderedMatch = false;
            param7.OrderedMatch = false;
            param8.OrderedMatch = false;

            int count = 21;
            object[] paramVector = new object[count];      
            for (int i = 0; i < count; ++i)
                paramVector[i] = i * 0.05;
            var textList = MisspellsGenerationModule.GetMisspeledSet(@"Data/niespokojni.txt", mistake);
           // TestModule.CompareMethods(textList, paramVector, paramName, param, param2, param3, param4, param5, param6, param7, param8);
            TestModule.CompareMethods(textList, paramVector, paramName, param);
        }
    }
}
