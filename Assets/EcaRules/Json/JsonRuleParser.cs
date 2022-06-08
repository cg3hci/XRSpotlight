using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EcaRules.Types;
using ECAScripts.Utils;
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
            var _event = ParseAction(r.Event);
            var _condition = ParseCondition(r.Condition);
            var _actions = new List<EcaAction>();
            foreach (var a in r.Actions)
            {
                _actions.Add(ParseAction(a));
            }
            return new EcaRule(_event, _condition, _actions);
        }

        private EcaAction ParseAction(JsonEcaAction a)
        {
            var ecaAction = new EcaAction(null, "");

            Type subjectType;
            GameObject subject;
            EcaAction.GetReference(null, a.Subj, out subjectType, out subject);
            if (subject == null) 
                return null;
            
            var verbs = ByVerb(subject, a.Verb);
            foreach (var tuple in verbs)
            {
                if (!string.IsNullOrEmpty(a.SpecVal))
                {
                    if (tuple.Item1.ValueType == null) 
                        continue;
                    var val = GetValue(tuple.Item1.ValueType, a.SpecVal);
                    ecaAction.SetModifierValue(val);
                }

                if (!string.IsNullOrEmpty(a.Spec))
                {
                    if (tuple.Item1.ModifierString.Equals(a.SpecVal))
                        ecaAction.SetModifier(a.Spec);
                    else
                        continue;
                }

                if (!string.IsNullOrEmpty(a.DirObj))
                {
                    if (tuple.Item1.ObjectType == null)
                        continue;
                    var val = GetValue(tuple.Item1.ObjectType, a.DirObj);
                    ecaAction.SetObject(val);
                }
                
                ecaAction.SetSubject(subject);
                ecaAction.SetActionMethod(a.Verb);
                ecaAction.GetMethod().Add(tuple.Item2);

            }
            return ecaAction.GetSubject() != null ? ecaAction: null;
        }

       


        private object GetValue(Type type, string encoded)
        {
            if (type.IsSubclassOf(typeof(GameObject)))
            {
                GameObject val = null;
                Type valType = null;
                EcaAction.GetReference(null, encoded, out valType, out val);
                return val;
            }

            if (type == typeof(ECABoolean))
            {
                if(encoded.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    return ECABoolean.TRUE;
                if(encoded.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                    return ECABoolean.FALSE;
                if (encoded.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                    return ECABoolean.YES;
                if (encoded.Equals("no", StringComparison.InvariantCultureIgnoreCase))
                    return ECABoolean.NO;
                if (encoded.Equals("on", StringComparison.InvariantCultureIgnoreCase))
                    return ECABoolean.ON;
                if (encoded.Equals("off", StringComparison.InvariantCultureIgnoreCase))
                    return ECABoolean.OFF;
            }

            if (type == typeof(EcaColor))
            {
                return new EcaColor(encoded, encoded.StartsWith("#"));
            }

            if (type == typeof(EcaPov))
            {
                if (encoded.Equals("first", StringComparison.InvariantCultureIgnoreCase))
                    return EcaPov.First;
                if (encoded.Equals("third", StringComparison.InvariantCultureIgnoreCase))
                    return EcaPov.Third;
            }

            if (type == typeof(int))
            {
                return int.Parse(encoded);
            }

            if (type == typeof(float))
            {
                return float.Parse(encoded);
            }
            
            return null;
        }

        private EcaCondition ParseCondition(JsonEcaCondition c)
        {
            if (String.IsNullOrEmpty(c.LambdaExpr))
            {
                return null;
            }
            var lambda = new LambdaCondition(c.Ids, c.LambdaExpr);
            return lambda;
        }


        private List<Tuple<EcaActionAttribute, MethodInfo>> ByVerb(GameObject o, string v)
        {
            var tuples = new List<Tuple<EcaActionAttribute, MethodInfo>>();

            foreach(Component c in o.GetComponents(typeof(Component)))
            {
                if (Attribute.IsDefined(c.GetType(), typeof(EcaRules4AllAttribute)))
                {
                    foreach (MethodInfo m in c.GetType().GetMethods())
                    {
                        var attr = m.GetCustomAttribute<EcaActionAttribute>();
                        if(attr != null && attr.Verb.Equals(v))
                        {
                            tuples.Add(new Tuple<EcaActionAttribute, MethodInfo>(attr, m));
                        }
                    }
                }
            }
           

            return tuples;
        }
        
    }
}