using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace PolishNgramSpellChecker.Tests.Modules
{
    internal static class PreparationModule
    {
        public static List<string[]> LoadTestFile(string path)
        {
            Console.WriteLine("Start loading: " + path);

            List<string[]> result = new List<string[]>();
            var text = File.ReadAllText(path);
            var sentences = text.Split(new char[] { '.', ':', ';', '"', '”', '“', '(', ')', '-' });

            for (int i = 0; i < sentences.Length; ++i)
            {
                sentences[i] = sentences[i].Replace(',', ' ').Trim();
                if (sentences[i].Length == 0) continue;
                var words = sentences[i].Split().ToList();
                words.RemoveAll(x => x.Length == 0);
                result.Add(words.ToArray());
            }

            Console.WriteLine("Done loading");

            return result;
        }
    }
}
