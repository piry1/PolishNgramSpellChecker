using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PolishNgramSpellChecker.Modules.Orthography
{
    internal class OrthographyModule
    {
        private HashSet<string> _polishDictionary = new HashSet<string>();
        private string _dictionaryPath = @"data/unigrams.txt";

        public OrthographyModule() => LoadDictionary(_dictionaryPath);

        public bool[] IsCorrect(string[] words)
        {
            bool[] result = new bool[words.Length];

            for (int i = 0; i < words.Length; ++i)
                result[i] = (_polishDictionary.Contains(words[i]) || words[i].First() == '_');

            return result;
        }

        private void LoadDictionary(string path)
        {
            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                try
                {
                    var word = line.Trim().Split(' ')[1];
                    _polishDictionary.Add(word);
                }
                catch
                {
                    Console.WriteLine($"Wrong record in: {_dictionaryPath} -- {line}");
                }
            }
            Console.WriteLine("Done loading");
        }
    }
}
