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
            public int N { get; set; } = 2;
            public int MinN { get; set; } = 2;
            public int MaxN { get; set; } = 5;
            public bool OrderedMatch { get; set; } = true;
            public double MinScoreSpace { get; set; } = 0.35;
            public ScoreCountFunctions ScoreCountFunction { get; set; } = ScoreCountFunctions.Standard;
            public DetectionAlgorithm DetectionAlgorithm { get; set; } = DetectionAlgorithm.Simple;
            public string Text { get; set; }
            public SpellCheckerParams GetSpellCheckerParams()
            {
                return new SpellCheckerParams
                {
                    N = this.N,
                    MinN = this.MinN,
                    MaxN = this.MaxN,
                    OrderedMatch = this.OrderedMatch,
                    ScoreCountFunc = Params.ScoreCountFunction.GetFunc(this.ScoreCountFunction),
                    DetectionAlgorithm = this.DetectionAlgorithm,
                    MinScoreSpace = this.MinScoreSpace
                };

            }

        }




    }
}