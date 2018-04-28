using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishNgramSpellChecker.Tests.Modules
{
    internal class ConfusionMatrix
    {
        public int TotalPopulation { get; private set; }

        public double ConditionPositive { get; set; }
        public double ConditionNegative { get; set; } // generated misspells
        public double PredictedPositive { get; set; }
        public double PredictedNegative { get; set; }

        public double TP { get; private set; } = 0;
        public double TN { get; private set; } = 0;
        public double FP { get; private set; } = 0;
        public double FN { get; private set; } = 0;

        public double Prevalence { get; private set; } // all target positive cases
        public double ACC { get; private set; }        // accuracy

        public double TPR { get; private set; }
        public double FPR { get; private set; }
        public double FNR { get; private set; }
        public double TNR { get; private set; }
        public double PPV { get; private set; }
        public double FDR { get; private set; }
        public double FOR { get; private set; }
        public double NPV { get; private set; }
        public double LRplus { get; private set; }
        public double LRminus { get; private set; }
        public double DOR { get; private set; }
        public double F1score { get; private set; }

        public ConfusionMatrix(bool[] target, bool[] output)
        {
            for (int i = 0; i < target.Length; ++i)
            {
                if (target[i] && output[i])
                    TP++;
                else if (!target[i] && !output[i])
                    TN++;
                else if (target[i] && !output[i])
                    FN++;
                else if (!target[i] && output[i])
                    FP++;
            }

            ConditionPositive = TP + FN;
            ConditionNegative = FP + TN;
            PredictedPositive = TP + FP;
            PredictedNegative = TN + FN;
            TotalPopulation = target.Length;

            Prevalence = (TP + FN) / TotalPopulation;
            ACC = (TP + TN) / TotalPopulation;

            TPR = TP / ConditionPositive;
            FPR = FP / ConditionNegative;
            FNR = FN / ConditionPositive;
            TNR = TN / ConditionNegative;

            PPV = TP / PredictedPositive;
            FDR = FP / PredictedPositive;
            FOR = FN / PredictedNegative;
            NPV = TN / PredictedNegative;

            LRplus = TPR / FPR;
            LRminus = FNR / TNR;

            DOR = LRplus / LRminus;
            F1score = 2 / (1 / TPR + 1 / PPV);
        }

        public override string ToString()
        {
            return $"{TotalPopulation};{ConditionPositive};{ConditionNegative};{Prevalence};{ACC}\n" +
                 $"{PredictedPositive};{TP};{FP};{PPV};{FDR}\n" +
                 $"{PredictedNegative};{FN};{TN};{FOR};{NPV}\n" +
                 $";{TPR};{FPR};{LRplus};{DOR}\n" +
                 $";{FNR};{TNR};{LRminus};{F1score}";
        }
    }
}
