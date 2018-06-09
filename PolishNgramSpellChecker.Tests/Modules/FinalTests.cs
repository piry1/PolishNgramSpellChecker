using PolishNgramSpellChecker.Params;
using mistype = PolishNgramSpellChecker.Tests.Modules.MisspellsGenerationModule.MistakeType;
namespace PolishNgramSpellChecker.Tests.Modules
{
    public static class FinalTests
    {
        internal static void CompareMethodsCorrection(string fileName, mistype mistake, int MinN, int MaxN)
        {
            string paramName = nameof(SpellCheckerParams.MinScoreSpace);
            SpellCheckerParams param = new SpellCheckerParams()
            {
                ScoreMulti = true,
                DetectionMethod = "w",
                MaxN = MaxN,
                MinN = MinN,
                OrderedMatch = true,
                MinScoreSpace = 0.0,
                CorrectionMethod = "w",
                MinPoints = 0.1,
                UseDetection = false,
                ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
            };
            var param2 = param.GetCopy();
            var param3 = param.GetCopy();
            var param4 = param.GetCopy();

            param2.CorrectionMethod = "f";
            param3.CorrectionMethod = "d"; param3.Fuzziness = mistake == mistype.NoDia || mistake == mistype.HeavyNoDia ? Nest.Fuzziness.EditDistance(0) : Nest.Fuzziness.Auto; // <= ISTOTNE
            param4.CorrectionMethod = "dd"; param4.Fuzziness = mistake == mistype.NoDia || mistake == mistype.HeavyNoDia ? Nest.Fuzziness.EditDistance(0) : Nest.Fuzziness.Auto; // <= ISTOTNE

            var param5 = param.GetCopy();
            var param6 = param2.GetCopy();
            var param7 = param3.GetCopy(); param7.Fuzziness = mistake == mistype.NoDia || mistake == mistype.HeavyNoDia ? Nest.Fuzziness.EditDistance(0) : Nest.Fuzziness.Auto; // <= ISTOTNE
            var param8 = param4.GetCopy(); param8.Fuzziness = mistake == mistype.NoDia || mistake == mistype.HeavyNoDia ? Nest.Fuzziness.EditDistance(0) : Nest.Fuzziness.Auto; // <= ISTOTNE

            param5.OrderedMatch = false;
            param6.OrderedMatch = false;
            param7.OrderedMatch = false;
            param8.OrderedMatch = false;

            int count = 21;
            object[] paramVector = new object[count];      
            for (int i = 0; i < count; ++i)
                paramVector[i] = i * 0.05;
            var textList = MisspellsGenerationModule.GetMisspeledSet(@"Data/niespokojni.txt", mistake);
            TestModule.CompareMethods(textList, paramVector, paramName, param, param2, param3, param4, param5, param6, param7, param8);
           // TestModule.CompareMethods(textList, paramVector, paramName, param);
        }

        internal static void CompareMethods(string fileName, mistype mistake, int MinN, int MaxN)
        {
            string paramName = nameof(SpellCheckerParams.MinScoreSpace);
            SpellCheckerParams param1 = new SpellCheckerParams()
            {
                ScoreMulti = false,
                DetectionMethod = "w",
                MaxN = MaxN,
                MinN = MinN,
                OrderedMatch = true,
                MinScoreSpace = 0.0,
                CorrectionMethod = "w",
                MinPoints = 0.1,
                UseDetection = false,
                ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
            };

            var param2 = param1.GetCopy();
            var param3 = param1.GetCopy();
            var param4 = param1.GetCopy();

            param2.CorrectionMethod = "f";
            param3.CorrectionMethod = "d"; param3.Fuzziness = mistake == mistype.NoDia || mistake == mistype.HeavyNoDia ? Nest.Fuzziness.EditDistance(0) : Nest.Fuzziness.Auto; // <= ISTOTNE
            param4.CorrectionMethod = "dd"; param4.Fuzziness = mistake.Equals(mistype.NoDia) || mistake == mistype.HeavyNoDia ? Nest.Fuzziness.EditDistance(0) : Nest.Fuzziness.Auto; // <= ISTOTNE

            var param5 = param1.GetCopy();
            var param6 = param2.GetCopy();
            var param7 = param3.GetCopy(); param7.Fuzziness = mistake.Equals(mistype.NoDia) || mistake == mistype.HeavyNoDia ? Nest.Fuzziness.EditDistance(0) : Nest.Fuzziness.Auto; // <= ISTOTNE
            var param8 = param4.GetCopy(); param8.Fuzziness = mistake.Equals(mistype.NoDia) || mistake == mistype.HeavyNoDia ? Nest.Fuzziness.EditDistance(0) : Nest.Fuzziness.Auto; // <= ISTOTNE

            param5.OrderedMatch = false;
            param6.OrderedMatch = false;
            param7.OrderedMatch = false;
            param8.OrderedMatch = false;

            object[] paramVector = new object[] { 0 };
            
            var textList = MisspellsGenerationModule.GetMisspeledSet(@"Data/niespokojni.txt", mistake);
            TestModule.CompareMethods(textList, paramVector, paramName, param1, param2, param3, param4, param5, param6, param7, param8);
        }

        internal static void CompareMethodsDetection(string fileName, mistype mistake, int MinN, int MaxN)
        {
            string paramName = nameof(SpellCheckerParams.MinPoints);
            SpellCheckerParams param = new SpellCheckerParams()
            {
                ScoreMulti = true,
                DetectionMethod = "w",
                MaxN = MaxN,
                MinN = MinN,
                OrderedMatch = true,
                MinScoreSpace = 0.0,
                CorrectionMethod = "w",
                MinPoints = 0.1,
                UseDetection = true,
                ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
            };

            var param2 = param.GetCopy();
            var param3 = param.GetCopy();
            var param4 = param.GetCopy();

            param2.CorrectionMethod = "dd"; param2.Fuzziness = mistake == mistype.NoDia || mistake == mistype.HeavyNoDia ? Nest.Fuzziness.EditDistance(0) : Nest.Fuzziness.Auto; // <= ISTOTNE
            param3.CorrectionMethod = "w"; param3.OrderedMatch = false;
            param4.CorrectionMethod = "dd"; param4.OrderedMatch = false; param4.Fuzziness = mistake == mistype.NoDia || mistake == mistype.HeavyNoDia ? Nest.Fuzziness.EditDistance(0) : Nest.Fuzziness.Auto; // <= ISTOTNE

            var param5 = param.GetCopy();
            param5.DetectionMethod = "p";
            var param6 = param2.GetCopy(); param6.DetectionMethod = "p";
            var param7 = param3.GetCopy(); param7.DetectionMethod = "p";
            var param8 = param4.GetCopy(); param8.DetectionMethod = "p";

            int count = 40;
            object[] paramVector = new object[count];
            for (int i = 1; i <= count; ++i)
                paramVector[i - 1] = 0.1*i;
            var textList = MisspellsGenerationModule.GetMisspeledSet(@"Data/niespokojni.txt", mistake);
            TestModule.CompareMethods(textList, paramVector, paramName, param, param2, param3, param4, param5, param6, param7, param8);
           // TestModule.CompareMethods(textList, paramVector, paramName, param, param5);
        }

        internal static void DetectionCorrectionCrossTest(string fileName, mistype mistake, int MinN, int MaxN, string method)
        {
            
            SpellCheckerParams param = new SpellCheckerParams()
            {
                ScoreMulti = true,
                DetectionMethod = "w",
                MaxN = MaxN,
                MinN = MinN,
                OrderedMatch = true,
                MinScoreSpace = 0.0,
                CorrectionMethod = method,
                MinPoints = 0.1,
                UseDetection = true,
                ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
            };

            string paramName_a = nameof(SpellCheckerParams.MinPoints);
            int count_a = 30;
            object[] paramVector_a = new object[count_a];
            for (int i = 1; i <= count_a; ++i)
                paramVector_a[i - 1] = 0.1 * i;

            string paramName_b = nameof(SpellCheckerParams.MinScoreSpace);
            int count_b = 20;
            object[] paramVector_b = new object[count_b];
            for (int i = 0; i < count_b; ++i)
                paramVector_b[i] = i * 0.025;


            var textList = MisspellsGenerationModule.GetMisspeledSet(fileName, mistake);
            TestModule.RunCrossTests(textList, param, paramVector_a, paramName_a, paramVector_b, paramName_b);
            // TestModule.CompareMethods(textList, paramVector, paramName, param, param5);
        }
    }
}
