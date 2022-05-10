using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine.Serialization;

namespace EcaRules.Json
{
    [Serializable]
    public class JsonEcaRules
    {
        public JsonEcaRule[] Rules = Array.Empty<JsonEcaRule>();

        public override bool Equals(object o)
        {
            if (o is JsonEcaRules rs)
            {
                return this.Equals(rs);
            }
            else
            {
                return false;
            }
        }

        protected bool Equals(JsonEcaRules other)
        {
            return this.Rules.SequenceEqual(other.Rules);
        }

        public override int GetHashCode()
        {
            return (Rules != null ? Rules.GetHashCode() : 0);
        }
    }
    
    [Serializable]
    public class JsonEcaRule
    {

        public JsonEcaAction Event = new JsonEcaAction();
        public JsonEcaCondition Condition = new JsonEcaCondition();
        public JsonEcaAction[] Actions = Array.Empty<JsonEcaAction>();

        public override bool Equals(object o)
        {
            if (o is JsonEcaRule rule)
            {
                return this.Equals(rule);
            }
            else
            {
                return false;
            }
        }

        protected bool Equals(JsonEcaRule other)
        {
            return this.Event.Equals(other.Event) &&
                   this.Condition.Equals(other.Condition) &&
                   this.Actions.SequenceEqual(other.Actions);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Event != null ? Event.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Condition != null ? Condition.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Actions != null ? Actions.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    [Serializable]
    public class JsonEcaAction
    {
        // init to empty string for compatibility with JsonUtility.ToJson()
        public string Subj = "";
        public string Verb = "";
        public string DirObj = "";
        public string Spec = "";
        public string SpecVal = "";

        public override bool Equals(object o)
        {
            if (o is JsonEcaAction act)
            {
                return this.Equals(act);
            }
            else
            {
                return false;
            }
        }

        protected bool Equals(JsonEcaAction other)
        {
            return this.Subj.Equals(other.Subj) &&
                   this.Verb.Equals(other.Verb) &&
                   this.DirObj.Equals(other.DirObj) &&
                   this.Spec.Equals(other.Spec) &&
                   this.SpecVal.Equals(other.Spec);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Subj != null ? Subj.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Verb != null ? Verb.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DirObj != null ? DirObj.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Spec != null ? Spec.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SpecVal != null ? SpecVal.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    [Serializable]
    public class JsonEcaCondition
    {
        public string[] Ids = Array.Empty<string>();
        public string LambdaExpr = "";

        public override bool Equals(object o)
        {
            if (o is JsonEcaCondition cnd)
            {
                return this.Equals(cnd);
            }
            else
            {
                return false;
            }
        }

        protected bool Equals(JsonEcaCondition other)
        {
            return this.Ids.SequenceEqual(other.Ids)
                   && this.LambdaExpr.Equals(other.LambdaExpr);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Ids != null ? Ids.GetHashCode() : 0) * 397) ^ (LambdaExpr != null ? LambdaExpr.GetHashCode() : 0);
            }
        }
    }

}