using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.Database;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Tests.Model;
using PolishNgramSpellChecker.Tests.Modules;

namespace PolishNgramSpellChecker.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            //FinalTests.CompareMethodsDetection(@"Data/niespokojni.txt", MisspellsGenerationModule.MistakeType.HeavyNoDia, 2, 2);
           // FinalTests.DetectionCorrectionCrossTest(@"Data/niespokojni.txt", MisspellsGenerationModule.MistakeType.Shuffle, 2, 3, "dd");
            FinalTests.CompareMethodsCorrection(@"Data/niespokojni.txt", MisspellsGenerationModule.MistakeType.Shuffle, 2, 3);

            // FinalTests.CompareMethods(@"Data/niespokojni.txt", MisspellsGenerationModule.MistakeType.BazForm, 3, 3);

            //SpellChecker spellChecker = new SpellChecker();
            //SpellCheckerParams param = new SpellCheckerParams()
            //{
            //    ScoreMulti = true,
            //    DetectionMethod = "w",
            //    MaxN = 2,
            //    MinN = 2,
            //    OrderedMatch = true,
            //    MinScoreSpace = 0.0,
            //    CorrectionMethod = "w",
            //    MinPoints = 0.05,
            //    UseDetection = false,
            //    ScoreCountFunc = ScoreCountFunction.GetFunc(ScoreCountFunctions.Pow10ByN)
            //};

            //var param2 = param.GetCopy();
            //var param3 = param.GetCopy();
            //param2.CorrectionMethod = "f";
            //param3.CorrectionMethod = "d";

            //string param1Name = nameof(SpellCheckerParams.MinScoreSpace);
            //string param2Name = nameof(SpellCheckerParams.MinPoints);
            //Console.WriteLine(param1Name);
            //int count1 = 21;
            //int count2 = 10;
            //object[] param1Vector = new object[count1]; //{ 0.0, 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4 };
            //object[] param2Vector = new object[count2];

            //for (int i = 0; i < count1; ++i)
            //    param1Vector[i] = i*0.05;

            //for (int i = 0; i < count2; ++i)
            //    param2Vector[i] = 0.1 * (i + 1);


            //var textList = MisspellsGenerationModule.GetMisspeledSet(@"Data/niespokojni.txt", MisspellsGenerationModule.MistakeType.Misspells);

            //******************testing coverage * *****************
            //var sentences = PreparationModule.LoadTestFile(@"Data/niespokojni.txt", true);
            //for (int i = 5; i < 6; ++i)
            //{
            //    Console.WriteLine($"N: {i} ************************");
            //    var res = CoverModule.Coverage(sentences, i, true, 1);
            //    Console.WriteLine($"Coverage or1: {res.CoveragePercent * 100}, Ngram Percent: {res.Ncoverage * 100}");
            //    res = CoverModule.Coverage(sentences, i, true, 2);
            //    Console.WriteLine($"Coverage or2: {res.CoveragePercent * 100}, Ngram Percent: {res.Ncoverage * 100}");

            //    //var res2 = CoverModule.Coverage(sentences, i, false, 1);
            //    //Console.WriteLine($"Coverage no_or1: {res2.CoveragePercent * 100}, Ngram Percent: {res2.Ncoverage * 100}");
            //    //res2 = CoverModule.Coverage(sentences, i, false, 2);
            //    //Console.WriteLine($"Coverage no_or2: {res2.CoveragePercent * 100}, Ngram Percent: {res2.Ncoverage * 100}");
            //}

            //var watch = System.Diagnostics.Stopwatch.StartNew();
            ////TestModule.RunTests(textList, param, param1Vector, param1Name);
            ////TestModule.RunCrossTests(textList, param, param1Vector, param1Name, param2Vector, param2Name);
            //TestModule.CompareMethods(textList, param1Vector, param1Name, param, param2, param3);
            //watch.Stop();
            //Console.WriteLine($"{watch.ElapsedMilliseconds / 1000}s - time");

            Console.WriteLine("END");
            Console.ReadKey();
        }


    }
}
