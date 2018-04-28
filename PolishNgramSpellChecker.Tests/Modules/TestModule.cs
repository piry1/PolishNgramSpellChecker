using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNgramSpellChecker.Params;
using PolishNgramSpellChecker.Tests.Model;
using PolishNgramSpellChecker.Tests.Modules;

namespace PolishNgramSpellChecker.Tests.Modules
{
    internal static class TestModule
    {
        public static void RunTests(List<Sentence[]> testTextsList, SpellCheckerParams spellParams)
        {
            SpellChecker spellChecker = new SpellChecker();
            int n = testTextsList.Count();
            int sentenceCount = testTextsList.Count() * testTextsList[0].Length;
            List<bool> target = new List<bool>();
            List<bool> output = new List<bool>();

            int idx = 1;
            foreach (var text in testTextsList)
            {
                Console.WriteLine($"{idx} texts of {n}");
                idx++;

                foreach (var sentence in text)
                {
                    var res = spellChecker.CheckSentence(string.Join(" ", sentence.Words), spellParams);

                    if (res.Words.Count() != sentence.Words.Count())
                    {
                        Console.WriteLine(string.Join(" ", sentence.OriginalWords));
                        Console.WriteLine(string.Join(" ", res.Words));
                    }

                    target.AddRange(sentence.IsWordCorrect);
                    output.AddRange(res.IsWordCorrect);
                }
            }

            //target.ForEach(x => x = !x);
            //output.ForEach(x => x = !x);

            var confusionMatrix = new ConfusionMatrix(target.ToArray(), output.ToArray());
            File.WriteAllText("Data/res.csv", confusionMatrix.ToString());
        }

    }
}
