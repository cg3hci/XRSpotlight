using System;
using System.Linq.Dynamic.Core;

namespace EcaRules.Json
{
    [Serializable]
    public class JsonEcaRule
    {

        public JsonEcaAction e;
        public string c;
        public JsonEcaAction[] a; 
        
        private void ParseCondition(string s)
        {
            var exp = DynamicExpressionParser.ParseLambda(typeof(bool), s, null);
        }
    }

    [Serializable]
    public class JsonEcaAction
    {
        public string s;
        public string v;
        public string o;
        public string m;
        public string mv;
    }

}