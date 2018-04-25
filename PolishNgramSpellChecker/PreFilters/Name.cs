using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNgramSpellChecker.PreFilters
{
    internal class Name
    {
        public string Value { get; }
        public int[] MaleCases { get; } = new int[6];
        public int[] FemaleCases { get; } = new int[6];

        public Name(string name)
        {
            Value = name;
            for (int i = 0; i < MaleCases.Length; ++i)
            {
                MaleCases[i] = 0;
                FemaleCases[i] = 0;
            }
        }

        public Name(string name, string maleCases, string femaleCases)
        {
            Value = name;
            MaleCases = StringToArray(maleCases);
            FemaleCases = StringToArray(femaleCases);
        }

        public void AddCase(int caseNumber, Gender gender)
        {
            if (gender == Gender.male)
                AddCase(caseNumber, MaleCases);
            else
                AddCase(caseNumber, FemaleCases);
        }

        private void AddCase(int caseNumber, int[] list)
        {
            if (caseNumber >= 0 && caseNumber < list.Length)
                list[caseNumber] = 1;
        }

        public override bool Equals(object obj)
        {
            var o = obj as Name;
            return o.Value.Equals(Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value}, {ArrayToString(MaleCases)}, {ArrayToString(FemaleCases)}";
        }

        private string ArrayToString(int[] list)
        {
            string result = string.Empty;
            foreach (var l in list)
                result += l;
            return result;
        }

        private int[] StringToArray(string data)
        {
            int[] result = new int[6];
            for (int i = 0; i < data.Length && i < 6; ++i)
                result[i] = Int32.Parse(data[i].ToString());
            return result;
        }

        public enum Gender
        {
            male,
            female
        }
    }
}
