using System;
using Newtonsoft.Json;
using PolishNgramSpellChecker.Params;

namespace PolishNgramSpellChecker.HttpApi.Server.Controllers
{
    internal class SpellCheckerController
    {
        public SpellChecker _spellChecker = new SpellChecker();

        public HttpResponse Check(string body)
        {
            var request = JsonConvert.DeserializeObject<SpellCheckerRequest>(body);
            var spellCheckerParams = request.GetSpellCheckerParams();
           // Console.WriteLine(spellCheckerParams);
            var result = _spellChecker.CheckSentence(request.Text, spellCheckerParams);
            return HttpResponse.ReturnJson(result);
        }

        public class SpellCheckerRequest
        {        
            public int MinN { get; set; } = 2;
            public int MaxN { get; set; } = 3;
            public bool OrderedMatch { get; set; } = true;
            public double MinScoreSpace { get; set; } = 0;
            public ScoreCountFunctions ScoreCountFunction { get; set; } = ScoreCountFunctions.Pow10ByN;            
            public string Text { get; set; }
            public double MinPoints { get; set; } = 20;
            public string Detection { get; set; } = "w";
            public string Correction { get; set; } = "w";
            public bool UseDetection { get; set; } = true;
            public bool Multi { get; set; }

            public SpellCheckerParams GetSpellCheckerParams()
            {
                return new SpellCheckerParams
                {
                    MinN = this.MinN,
                    MaxN = this.MaxN,
                    OrderedMatch = this.OrderedMatch,
                    ScoreCountFunc = Params.ScoreCountFunction.GetFunc(this.ScoreCountFunction),
                    MinScoreSpace = this.MinScoreSpace,
                    MinPoints = this.MinPoints,
                    CorrectionMethod = Correction,
                    DetectionMethod = Detection,     
                    UseDetection = UseDetection,
                    ScoreMulti = Multi
                };

            }

        }




    }
}