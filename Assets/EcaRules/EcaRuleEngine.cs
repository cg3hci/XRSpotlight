using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECAScripts.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace EcaRules
{
    ///<summary>
    ///<c>EcaRuleEngine</c> manages all the rule execution routines. 
    ///</summary>
    public class EcaRuleEngine : EcaActionListener
    {
        //RuleEngine()
        public static EcaRuleEngine singleton;
        private List<EcaRule> rules = new List<EcaRule>();
        private EcaEventBus ecaEventQueue;


        /// <summary>
        /// <para>Returns an Instance of the RuleEngine.</para>
        /// </summary>
        /// <returns>
        /// <para>The Singleton instance for the Rule Engine.</para>
        /// </returns>
        public static EcaRuleEngine GetInstance()
        {
            if (singleton == null)
            {
                singleton = new EcaRuleEngine();
            }

            return singleton;
        }

        private EcaRuleEngine()
        {
            ecaEventQueue = EcaEventBus.GetInstance();
        }

        ///<summary>
        ///<c>Add</c> adds a new rule inside the RuleEngine. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="r"/>: The rule to be added</description></item>
        ///</list>
        ///</summary>        
        public void Add(EcaRule r)
        {
            rules.Add(r);
            //Si sottoscrive nell'Eventbus un ascoltatore per la regola in questione
            ecaEventQueue.Subscribe(r.GetEvent(), this);
        }

        ///<summary>
        ///<c>Remove</c> removes a given rule from the RuleEngine. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="r"/>: The rule to be removed</description></item>
        ///</list>
        ///</summary>
        public void Remove(EcaRule r)
        {
            rules.Remove(r);
            //Si rimuove dall'Eventbus l'ascoltatore per la regola in questione
            ecaEventQueue.Unsubscribe(r.GetEvent());
        }

        /// <summary>
        /// Lists all the rules in the engine
        /// </summary>
        /// <returns>An iterator over the rules</returns>
        public IEnumerable<EcaRule> Rules()
        {
            foreach (EcaRule r in rules)
            {
                yield return r;
            }
        }

        ///<summary>
        ///<c>IncrementDecrement</c> adds or subtracts a quantity from the first object, equal as the second one. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="one"/>: The value to be incremented/decremented</description></item>
        ///<item><description><paramref name="two"/>: The quantity to add/remove</description></item>
        ///<item><description><paramref name="op"/>: The operation to do</description></item>
        ///</list>
        ///<para/>
        ///<strong>Returns:</strong> A boolean with the answer from the check
        ///</summary>
        private object IncrementDecrement(dynamic one, dynamic two, string op)
        {
            var res = one;
            if (op == "increases") res += two;
            else res -= two;
            return res;
        }

        ///<summary>
        ///<c>ExecuteAction</c> applies a rule into the system. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="act"/>: The action to execute</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public void ExecuteAction(EcaAction act)
        {
            var type = act.GetActionType();

            if (type != EcaAction.ActionType.INVALID)
            {
                List<FieldInfo> fields = new List<FieldInfo>();
                List<MethodInfo> methods = new List<MethodInfo>();
                List<Component> subjects = act.GetSubjectComponent();
                List<Component> objects = new List<Component>();
                int count;

                if (act.AreThereAnyObjects())
                    objects = act.GetObjectComponent();
                if (type == EcaAction.ActionType.CHANGES || type == EcaAction.ActionType.INCREMENTSDECREMENTS)
                {
                    fields = act.GetField();
                    count = fields.Count;
                }
                else
                {
                    methods = act.GetMethod();
                    count = methods.Count;
                }

                for (int i = 0; i < count; i++)
                {
                    switch (type)
                    {
                        case EcaAction.ActionType.CUSTOMCHANGE:
                            methods[i].Invoke(subjects[i], new[] {act.GetModifierValue()});
                            break;
                        case EcaAction.ActionType.CHANGES:
                            fields[i].SetValue(subjects[i], act.GetModifierValue());
                            break;
                        case EcaAction.ActionType.INCREMENTSDECREMENTS:
                            fields[i].SetValue(subjects[i],
                                IncrementDecrement(fields[i].GetValue(subjects[i]), act.GetModifierValue(),
                                    act.GetActionMethod()));
                            break;
                        case EcaAction.ActionType.OBJECT:
                            //if "Action.objct" is != null then we use a GameObject instead of a Component
                            methods[i].Invoke(subjects[i],
                                act.AreThereAnyObjects() ? new object[] {objects[i]} : new[] {act.GetObject()});
                            break;
                        case EcaAction.ActionType.VALUE:
                            methods[i].Invoke(subjects[i], new[] {act.GetObject()});
                            break;
                        case EcaAction.ActionType.VERB:
                            methods[i].Invoke(subjects[i], new object[] { });
                            break;
                        case EcaAction.ActionType.PASSIVE:
                            methods[i].Invoke(subjects[i], new object[] {objects[i]});
                            break;
                    }

                    ecaEventQueue.Publish(act);
                }
            }
        }

        ///<summary>
        ///<c>ListStateVariables</c> returns all the Statevariable tagged variables. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="o"/>: The GameObject to check</description></item>
        ///</list>
        ///<para/>
        ///<strong>Returns:</strong> A list of strings with the variable names
        ///</summary>
        //ListStateVariables (GameObject o)
        public List<String> ListStateVariables(GameObject o)
        {
            return this.ListStateVariables(o.GetComponent<MonoBehaviour>());
        }

        ///<summary>
        ///<c>ListStateVariables</c> returns all the Statevariable tagged variables. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="c"/>: The component to check</description></item>
        ///</list>
        ///<para/>
        ///<strong>Returns:</strong> A list of strings with the variable names
        ///</summary>
        public List<String> ListStateVariables(Component c)
        {
            return this.ListStateVariables(c.GetType());
        }

        ///<summary>
        ///<c>ListStateVariables</c> returns all the Statevariable tagged variables. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="c"/>: The type to check</description></item>
        ///</list>
        ///<para/>
        ///<strong>Returns:</strong> A list of strings with the variable names
        ///</summary>
        public List<String> ListStateVariables(Type c)
        {
            //MonoBehaviour sceneActive = o.GetComponent<MonoBehaviour>();
            //FieldInfo[] fields = sceneActive.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            //for (int i = 0; i < fields.Length; i++)
            //{
            //    StateVariableAttribute state = Attribute.GetCustomAttribute(fields[i], typeof(StateVariableAttribute)) as StateVariableAttribute;
            //    if (state != null)
            //    {
            //        Debug.Log("Fields[i].name: " + fields[i].Name);
            //        Debug.Log("State.name: " + state.Name);
            //        Debug.Log("State.Type: " + state.type);
            //        Debug.Log("--------------------------");
            //    }
            //}

            List<String> variables = new List<String>();
            foreach (FieldInfo m in c.GetFields())
            {
                object[] a = m.GetCustomAttributes(typeof(StateVariableAttribute), true);
                if (a.Length > 0)
                {
                    foreach (var item in a)
                    {
                        variables.Add(item.ToString());
                    }
                }
            }

            return variables;
        }

        ///<summary>
        ///<c>ListActions</c> returns all the ActionAttribute tagged variables. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="o"/>: The GameObject to check</description></item>
        ///</list>
        ///<para/>
        ///<strong>Returns:</strong> A list of strings with the action names
        ///</summary>
        public List<String> ListActions(GameObject o)
        {
            return this.ListActions(o.GetComponent<MonoBehaviour>());
        }

        ///<summary>
        ///<c>ListActions</c> returns all the ActionAttribute tagged variables. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="c"/>: The component to check</description></item>
        ///</list>
        ///<para/>
        ///<strong>Returns:</strong> A list of strings with the action names
        ///</summary>
        public List<String> ListActions(Component c)
        {
            return this.ListActions(c.GetType());
        }

        ///<summary>
        ///<c>ListActions</c> returns all the ActionAttribute tagged variables. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="c"/>: The Type to check</description></item>
        ///</list>
        ///<para/>
        ///<strong>Returns:</strong> A list of strings with the action names
        ///</summary>
        public List<String> ListActions(Type c)
        {
            List<String> actions = new List<String>();
            foreach (MethodInfo m in c.GetMethods())
            {
                object[] a = m.GetCustomAttributes(typeof(ActionAttribute), true);
                if (a.Length > 0)
                {
                    foreach (var item in a)
                    {
                        actions.Add(item.ToString());
                    }
                }
            }

            return actions;
        }


        /*
                public Dictionary<GameObject, ECARuleInfo> CreateDescriptors(GameObject[] objects)
                {
                    Dictionary<GameObject, ECARuleInfo> ecaDescriptors = new Dictionary<GameObject, ECARuleInfo>();

                    foreach(GameObject obj in objects)
                    {
                        ECARuleInfo info = new ECARuleInfo();
                        info.EcaRuleType = typeof(System.Object);
                        ecaDescriptors.Add(obj, info);
                        foreach(Component c in obj.GetComponents())
                        {
                            Type cType = c.GetType();
                            if(Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
                            {
                                // we found a ECARules4All managed type
                                if (cType.IsSubclassOf(typeof(ECARules4All.Object)))
                                {
                                    // find the minimum type associated with the different components
                                    if (cType.IsSubclassOf(info.EcaRuleType))
                                    {
                                        info.EcaRuleType = cType;
                                    }

                                }

                                // add the actions to the list
                                foreach (MethodInfo m in cType.GetMethods())
                                {
                                    ActionAttribute[] actions = (ActionAttribute[])m.GetCustomAttributes(typeof(ActionAttribute), true);
                                    foreach (ActionAttribute a in actions)
                                    {
                                        info.Actions.Add(a);
                                    }
                                }
                            }
                        }
                    }

                    return ecaDescriptors;
                }

        public List<String> BuildEventList()
        {
            List<String> actions = new List<String>();
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in ass.GetExportedTypes())
                {
                    if (t.IsSubclassOf(typeof(UnityEngine.Component)) && Attribute.IsDefined(t, typeof(ECARules4AllAttribute)))
                    {
                        actions.AddRange(this.ListActions(t));
                    }
                }
            }

            return actions;
        }
        */

        ///<summary>
        ///<c>ActionPerformed</c> is an implementation of the <see cref="EcaActionListener"/> interface.
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="a"/>: The action that the EventBus notified to be executed</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public void ActionPerformed(EcaAction a)
        {
            //se sono arrivato qui ho ricevuto un'azione appena fatta, controllo nella lista di regole quale triggera e le eseguo
            List<EcaAction> toExecute = new List<EcaAction>();
            foreach (EcaRule r in rules)
            {
                if (r.GetEvent() == a && (r.GetCondition() == null || r.GetCondition().Evaluate()))
                {
                    foreach (EcaAction act in r.GetActions())
                    {
                        toExecute.Add(act);
                    }
                }
            }

            foreach (EcaAction act in toExecute)
            {
                ExecuteAction(act);
            }
        }
    }
    
}