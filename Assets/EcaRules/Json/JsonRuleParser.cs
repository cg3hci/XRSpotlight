using System.IO;
using UnityEngine;

namespace EcaRules.Json
{
    public class JsonRuleParser: IEcaRuleParser
    {
        public JsonEcaRules Rules { get; internal set; }

        public void ReadRuleFile(string path)
        {
            string json = File.ReadAllText(path);
            this.Rules = JsonUtility.FromJson<JsonEcaRules>(json);
            foreach (var r in this.Rules.Rules)
            {
                var ecaRule = ParseRule(r);
                // TODO: reactivate this when the parsing is ready
                //EcaRuleEngine.GetInstance().Add(ecaRule);
            }
        }

        private EcaRule ParseRule(JsonEcaRule r)
        {
            // TODO: stub implementation
            return new EcaRule(new EcaAction(null, "test"), null);
        }
    }
}