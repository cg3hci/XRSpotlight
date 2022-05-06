using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECAScripts.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace ECARules4All.RuleEngine
{
    ///<summary>
    ///<c>RuleEngine</c> manages all the rule execution routines. 
    ///</summary>
    public class RuleEngine : ActionListener
    {
        //RuleEngine()
        public static RuleEngine singleton;
        private List<Rule> rules = new List<Rule>();
        private EventBus eventQueue;


        /// <summary>
        /// <para>Returns an Instance of the RuleEngine.</para>
        /// </summary>
        /// <returns>
        /// <para>The Singleton instance for the Rule Engine.</para>
        /// </returns>
        public static RuleEngine GetInstance()
        {
            if (singleton == null)
            {
                singleton = new RuleEngine();
            }

            return singleton;
        }

        private RuleEngine()
        {
            eventQueue = EventBus.GetInstance();
        }

        ///<summary>
        ///<c>Add</c> adds a new rule inside the RuleEngine. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="r"/>: The rule to be added</description></item>
        ///</list>
        ///</summary>        
        public void Add(Rule r)
        {
            rules.Add(r);
            //Si sottoscrive nell'Eventbus un ascoltatore per la regola in questione
            eventQueue.Subscribe(r.GetEvent(), this);
        }

        ///<summary>
        ///<c>Remove</c> removes a given rule from the RuleEngine. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="r"/>: The rule to be removed</description></item>
        ///</list>
        ///</summary>
        public void Remove(Rule r)
        {
            rules.Remove(r);
            //Si rimuove dall'Eventbus l'ascoltatore per la regola in questione
            eventQueue.Unsubscribe(r.GetEvent());
        }

        /// <summary>
        /// Lists all the rules in the engine
        /// </summary>
        /// <returns>An iterator over the rules</returns>
        public IEnumerable<Rule> Rules()
        {
            foreach (Rule r in rules)
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
        public void ExecuteAction(Action act)
        {
            var type = act.GetActionType();

            if (type != Action.ActionType.INVALID)
            {
                List<FieldInfo> fields = new List<FieldInfo>();
                List<MethodInfo> methods = new List<MethodInfo>();
                List<Component> subjects = act.GetSubjectComponent();
                List<Component> objects = new List<Component>();
                int count;

                if (act.AreThereAnyObjects())
                    objects = act.GetObjectComponent();
                if (type == Action.ActionType.CHANGES || type == Action.ActionType.INCREMENTSDECREMENTS)
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
                        case Action.ActionType.CUSTOMCHANGE:
                            methods[i].Invoke(subjects[i], new[] {act.GetModifierValue()});
                            break;
                        case Action.ActionType.CHANGES:
                            fields[i].SetValue(subjects[i], act.GetModifierValue());
                            break;
                        case Action.ActionType.INCREMENTSDECREMENTS:
                            fields[i].SetValue(subjects[i],
                                IncrementDecrement(fields[i].GetValue(subjects[i]), act.GetModifierValue(),
                                    act.GetActionMethod()));
                            break;
                        case Action.ActionType.OBJECT:
                            //if "Action.objct" is != null then we use a GameObject instead of a Component
                            methods[i].Invoke(subjects[i],
                                act.AreThereAnyObjects() ? new object[] {objects[i]} : new[] {act.GetObject()});
                            break;
                        case Action.ActionType.VALUE:
                            methods[i].Invoke(subjects[i], new[] {act.GetObject()});
                            break;
                        case Action.ActionType.VERB:
                            methods[i].Invoke(subjects[i], new object[] { });
                            break;
                        case Action.ActionType.PASSIVE:
                            methods[i].Invoke(subjects[i], new object[] {objects[i]});
                            break;
                    }

                    eventQueue.Publish(act);
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
        ///<c>ActionPerformed</c> is an implementation of the <see cref="ActionListener"/> interface.
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="a"/>: The action that the EventBus notified to be executed</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public void ActionPerformed(Action a)
        {
            //se sono arrivato qui ho ricevuto un'azione appena fatta, controllo nella lista di regole quale triggera e le eseguo
            List<Action> toExecute = new List<Action>();
            foreach (Rule r in rules)
            {
                if (r.GetEvent() == a && (r.GetCondition() == null || r.GetCondition().Evaluate()))
                {
                    foreach (Action act in r.GetActions())
                    {
                        toExecute.Add(act);
                    }
                }
            }

            foreach (Action act in toExecute)
            {
                ExecuteAction(act);
            }
        }
    }

    ///<summary>
    ///<c>Rule</c> describes a single rule, consisting of a "when" clause, a "then" list of actions and the optional "if" condition statements.
    ///<para/>
    ///See also: <seealso cref="Action"/>, <seealso cref="Condition"/>
    ///</summary>
    public class Rule
    {
        private Action Event;
        private Condition conditions;
        private List<Action> actions;

        ///<summary>
        ///Standard Declaration
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="when"/>: The "when" statement (Type: <seealso cref="Action"/>)</description></item>
        ///<item><description><paramref name="ifStatement"/>: The "if" statement(s) (Type: List &lt;<seealso cref="Condition"/>&gt;)</description></item>
        ///<item><description><paramref name="listOfActions"/>: The "then" list of actions(Type: List &lt;<seealso cref="Action"/>&gt;)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public Rule(Action when, Condition ifStatements, List<Action> listOfActions)
        {
            Event = when;
            conditions = ifStatements;
            actions = listOfActions;
        }


        ///<summary>
        ///Declaration without the if Statement 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="when"/>: The "when" statement (Type: <seealso cref="Action"/>)</description></item>
        ///<item><description><paramref name="listOfActions"/>: The "then" list of actions(Type: List &lt;<seealso cref="Action"/>&gt;)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public Rule(Action when, List<Action> listOfActions)
        {
            Event = when;
            conditions = null;
            actions = listOfActions;
        }

        ///<summary>
        ///<c>GetEvent</c> returns the action in the "when" clause. 
        ///<para/>
        ///<strong>Returns:</strong> The <seealso cref="Action"/> associated with the rule
        ///</summary>
        public Action GetEvent()
        {
            return Event;
        }

        ///<summary>
        ///<c>GetConditions</c> returns the condition(s) in the "if" clause. 
        ///<para/>
        ///<strong>Returns:</strong> The <seealso cref="Condition"/>(s) associated with the rule
        ///</summary>
        public Condition GetCondition()
        {
            return conditions;
        }

        ///<summary>
        ///<c>GetActions</c> returns the list of actions in the "then" clause. 
        ///<para/>
        ///<strong>Returns:</strong> The list of <seealso cref="Action"/> associated with the rule
        ///</summary>
        public List<Action> GetActions()
        {
            return actions;
        }
    }

    /// <summary>
    /// <c>Condition</c> is the superclass used by the <seealso cref="Rule"/> class for defining checks for rule execution
    /// <para/>
    /// NOTE: Use <seealso cref="SimpleCondition"/> for simple statements and <seealso cref="CompositeCondition"/> for lists of Conditions
    /// </summary>
    public class Condition
    {
        public Condition parent { get; set; }

        /// <summary>
        /// <c>Evaluate</c> returns the result of the evaluation of the statements contained inside of it.
        /// </summary>
        /// <strong>Returns:</strong> A boolean containing the result of the evaluation
        /// <para/>
        /// <exception cref="NotImplementedException"> if you attempt to instantiate a Condition class it'll always throw this exception, since it's implemented in their subclasses. </exception>
        public virtual bool Evaluate()
        {
            //A Condition which is neither a SimpleCondition or a CompositeCondition doesn't know
            //what to evaluate, it's a job for its derived classes
            throw new NotImplementedException(
                "You need to instantiate a SimpleCondition or a CompositeCondition object");
        }

        /// <summary>
        /// <c>IsLeaf</c> defines if this node is a leaf or not.
        /// <para/>
        /// <strong>Returns:</strong> A boolean that defines if this node is a leaf or not
        /// </summary>
        /// <exception cref="NotImplementedException"> if you attempt to instantiate a Condition class it'll always throw this exception, since it's implemented in their subclasses. </exception>
        public virtual bool IsLeaf()
        {
            //A Condition which is neither a SimpleCondition or a CompositeCondition doesn't know
            //if it's a leaf or not, it's a job for its derived classes
            throw new NotImplementedException(
                "You need to instantiate a SimpleCondition or a CompositeCondition object");
        }

        /// <summary>
        /// <c>GetParent</c> returns the parent node of this object 
        /// <para/>
        /// <strong>Returns:</strong> The parent <seealso cref="Condition"/> object for this one.
        /// </summary>
        public Condition GetParent()
        {
            return parent;
        }
    }

    public class CompositeCondition : Condition
    {
        public enum ConditionType
        {
            NONE,
            OR,
            AND,
            NOT
        }

        private ConditionType op;
        private List<Condition> children;

        public ConditionType Op
        {
            get { return op; }
            set
            {
                op = value;
                CheckChildrenCount();
            }
        }

        public CompositeCondition()
            : this(ConditionType.NONE, new List<Condition>())
        {
        }

        public CompositeCondition(ConditionType type, List<Condition> conditions)
        {
            children = conditions;
            Op = type;

            foreach (Condition child in children)
            {
                child.parent = this;
            }
        }

        public Condition GetChild(int i)
        {
            if (i >= 0 && i < children.Count())
            {
                return children[i];
            }

            return null;
        }

        public void AddChild(Condition c)
        {
            c.parent = this;
            children.Add(c);
            CheckChildrenCount();
        }

        public void InsertChild(int index, Condition c)
        {
            c.parent = this;
            children.Insert(index, c);
            CheckChildrenCount();
        }

        public void RemoveChild(Condition c)
        {
            children.Remove(c);
        }

        public int ChildrenCount()
        {
            return children.Count();
        }


        /// <summary>
        /// Lists all the rules in the engine
        /// </summary>
        /// <returns>An iterator over the rules</returns>
        public IEnumerable<Condition> Children()
        {
            foreach (Condition c in children)
            {
                yield return c;
            }
        }

        public override bool Evaluate()
        {
            bool res;
            bool temp = Op == ConditionType.AND ? true : false;

            foreach (Condition c in children)
            {
                switch (Op)
                {
                    case ConditionType.OR:
                        temp = temp || c.Evaluate();
                        if (temp) return temp;
                        break;

                    case ConditionType.AND:
                        temp = temp && c.Evaluate();
                        if (!temp) return temp;
                        break;

                    case ConditionType.NOT:
                        return !c.Evaluate();
                }
            }

            return temp;
        }

        public override bool IsLeaf()
        {
            return false;
        }

        private void CheckChildrenCount()
        {
            if (op == ConditionType.NOT && children.Count > 1)
            {
                throw new ArgumentException("Cannot set more than one child using the not operator");
            }
        }
    }


    ///<summary>
    ///<c>SimpleCondition</c> defines a simple condition to be checked, this is a subclass of <seealso cref="Condition"/> node. 
    ///</summary>
    public class SimpleCondition : Condition
    {
        private GameObject toCheck;
        private string property;
        private string checkSymbol;
        private object compareWith = null;

        private FieldInfo variable;
        private Component componentToCheck;

        ///<summary>
        ///Declaration for the first Condition
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="component"/>: The GameObject to be checked in the future (Type: <seealso cref="GameObject"/>)</description></item>
        ///<item><description><paramref name="property"/>: The property to be checked (Type: String)</description></item>
        ///<item><description><paramref name="symbol"/>: The symbol to use for the comparison, like "=", "is", "!=", etc. (Type: String)</description></item>
        ///<item><description><paramref name="compareWith"/>: The value to compare(Type: Object)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public SimpleCondition(GameObject component, string property, string symbol, object compareWith)
        {
            toCheck = component;
            this.property = property;
            checkSymbol = symbol;
            this.compareWith = compareWith;
        }

        ///<summary>
        ///<c>CompareValues</c> checks whether two objects satisfy given conditions. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="one"/>: The first object</description></item>
        ///<item><description><paramref name="two"/>: The second object</description></item>
        ///<item><description><paramref name="op"/>: The operation to do</description></item>
        ///</list>
        ///<para/>
        ///<strong>Returns:</strong> A boolean with the answer from the check
        ///</summary>
        //Funzione di appoggio che controlla se si soddisfano controlli logici data una stringa
        private bool CompareValues(dynamic one, dynamic two, string op)
        {
            switch (op)
            {
                case "is":
                case "=":
                    return one == two;
                case "is not":
                case "!=":
                    return one != two;
                case ">":
                    return one > two;
                case "<":
                    return one < two;
                case ">=":
                    return one >= two;
                case "<=":
                    return one <= two;
                default: return false;
            }
        }

        public bool IsValid()
        {
            if (toCheck != null && (property != null || property != ""))
            {
                string[] mathValues = {"<", ">", "<=", ">="};
                //Si identifica il componente da controllare, tra quelli presenti nel GameObject
                foreach (Component c in toCheck.GetComponents<Component>())
                {
                    Type cType = c.GetType();
                    //Se uno di loro contiene l'attributo Custom "ECARules4All" allora si ispeziona
                    if (Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
                    {
                        // we found a ECARules4All managed type
                        //Si va alla ricerca del campo che ci serve
                        foreach (FieldInfo m in cType.GetFields())
                        {
                            //Per ogni variabile etichettata con l'attributo "StateVariable" si controlla se i tipi combaciano, nel caso si fa un controllo sull'uguaglianza sui valori
                            StateVariableAttribute[] variables =
                                (StateVariableAttribute[]) m.GetCustomAttributes(typeof(StateVariableAttribute), true);
                            foreach (StateVariableAttribute a in variables)
                            {
                                //Check if the type supports mathematical operations (such as <, >, >= and <=)
                                ECARules4AllOperations.supportsMathematicalConditionChecks.TryGetValue(a.type,
                                    out var res);
                                //If the operation is supported check whether is true or not
                                if (!res && !mathValues.Contains(checkSymbol) || res)
                                {
                                    if (a.Name == GetProperty() && m.FieldType == GetValueType() ||
                                        a.Name == GetProperty() && m.FieldType == typeof(ECABoolean) &&
                                        compareWith is bool)
                                    {
                                        variable = m;
                                        componentToCheck = c;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override bool Evaluate()
        {
            if (IsValid())
            {
                return CompareValues(variable.GetValue(componentToCheck), GetValueToCompare(), GetSymbol());
            }

            return false;
        }

        public override bool IsLeaf()
        {
            return true;
        }

        ///<summary>
        ///<c>GetSubject</c> returns the Gameobject to be checked
        ///<para/>
        ///<strong>Returns:</strong> The Gameobject inside the class instance
        ///</summary>
        public GameObject GetSubject()
        {
            return toCheck;
        }

        ///<summary>
        ///<c>GetProperty</c> returns the name of the property to be checked
        ///<para/>
        ///<strong>Returns:</strong> The string containing the name
        ///</summary>
        public string GetProperty()
        {
            return property;
        }

        ///<summary>
        ///<c>GetSymbol</c> returns the symbol to be used in the comparison
        ///<para/>
        ///<strong>Returns:</strong> The string containing the symbol
        ///</summary>
        public string GetSymbol()
        {
            return checkSymbol;
        }

        ///<summary>
        ///<c>GetProperty</c> returns the value to be checked against the value inside the GameObject in the same condition statement.
        ///<para/>
        ///<strong>Returns:</strong> The string containing the value to compare
        ///</summary>
        public object GetValueToCompare()
        {
            return compareWith;
        }

        ///<summary>
        ///<c>GetValueType</c> returns the type of the value property
        ///<para/>
        ///<strong>Returns:</strong> The Type of the value
        ///</summary>
        public Type GetValueType()
        {
            return compareWith.GetType();
        }

        public override string ToString()
        {
            return toCheck + " " + property + " " + checkSymbol + " " + compareWith;
        }
    }

    ///<summary>
    ///Describes an interaction inside the system
    ///</summary>
    public class Action
    {
        public enum ActionType
        {
            INVALID,
            CUSTOMCHANGE,
            CHANGES,
            INCREMENTSDECREMENTS,
            VERB,
            OBJECT,
            VALUE,
            PASSIVE,
        }

        private GameObject a_subject = null;

        private string a_verb;

        //private string a_prep;
        private object a_object = null;
        private string a_modifier; //to/by
        private object a_value = null;
        private string a_unit = null;

        private List<MethodInfo> method = new List<MethodInfo>();
        private List<FieldInfo> field = new List<FieldInfo>();
        private List<Component> subject = new List<Component>(), objct = new List<Component>();
        private bool objects = false;
        private ActionType type;

        public Action(){}

        ///<summary>
        ///Declaration with all the elements (plus an explicit modifier)
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="sub"/>: The GameObject that executes the action (Type: <seealso cref="GameObject"/>)</description></item>
        ///<item><description><paramref name="verb"/>: The action that it does (Type: String)</description></item>
        ///<item><description><paramref name="obj"/>: The object that receives the action - in this case is a variable that changes value (Type: Object)</description></item>
        ///<item><description><paramref name="mod"/>: A string representing a decorative string to put between the object and the value (Type: String)</description></item>
        ///<item><description><paramref name="value"/>: The value to set or add/subtract(Type: Object)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public Action(GameObject sub, string verb, object obj, string mod, object value) : this(sub, verb, obj)
        {
            a_modifier = mod;
            this.a_value = value;
        }

        //public Action(GameObject sub, string verb, string preposition, object obj, object value)
        //    : this(sub, verb, obj, value)
        //{
        //    this.a_prep = preposition;
        //}

        ///<summary>
        ///Declaration without a value to set
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="sub"/>: The Gameobject that executes the action (Type: <seealso cref="GameObject"/>)</description></item>
        ///<item><description><paramref name="verb"/>: The action that it does (Type: String)</description></item>
        ///<item><description><paramref name="obj"/>: The object that supports the action(Type: Object)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public Action(GameObject sub, string verb, object obj) : this(sub, verb)
        {
            this.a_object = obj;
        }

        ///<summary>
        ///Declaration with only a verb
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="sub"/>: The Gameobject that executes the action (Type: <seealso cref="GameObject"/>)</description></item>
        ///<item><description><paramref name="verb"/>: The action that it does (Type: String)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public Action(GameObject sub, string verb)
        {
            this.a_subject = sub;
            this.a_verb = verb;
        }

        ///<summary>
        ///<c>GetSubject</c> returns the subject of the action
        ///<para/>
        ///<strong>Returns:</strong> The GameObject of the subject
        ///</summary>
        public GameObject GetSubject()
        {
            return a_subject;
        }

        ///<summary>
        ///<c>SetSubject</c> sets the subject of the action
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="sub"/>: The GameObject to be set as subject (Type: <seealso cref="GameObject"/>)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public void SetSubject(GameObject sub)
        {
            a_subject = sub;
        }

        ///<summary>
        ///<c>GetObject</c> returns the object of the action
        ///<para/>
        ///<strong>Returns:</strong> The object containing the object of the action
        public object GetObject()
        {
            return a_object;
        }

        ///<summary>
        ///<c>SetObject</c> sets the object of the action
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="obj"/>: The GameObject to be set as object (Type: <seealso cref="object"/>)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public void SetObject(object obj)
        {
            a_object = obj;
        }

        ///<summary>
        ///<c>GetActionMethod</c> returns the verb of the action
        ///<para/>
        ///<strong>Returns:</strong> The string containing the verb of the action
        ///</summary>
        public string GetActionMethod()
        {
            return a_verb;
        }

        ///<summary>
        ///<c>SetActionMethod</c> sets the verb of the action
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="verb"/>: The verb of the action (Type: <seealso cref="string"/>)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public void SetActionMethod(string verb)
        {
            a_verb = verb;
        }

        ///<summary>
        ///<c>GetPreposition</c> returns the preposition value
        ///<para/>
        ///<strong>Returns:</strong> The preposition value
        ///</summary>
        //public object GetPreposition()
        //{
        //    return a_prep;
        //}

        ///<summary>
        ///<c>SetPreposition</c> sets the value of the preposition
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="val"/>: The value to be set (Type: <seealso cref="string"/>)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        //public void SetPreposition(string val)
        //{
        //    a_prep = val;
        //}

        ///<summary>
        ///<c>GetModifierValue</c> returns the value to be set or added/subtracted
        ///<para/>
        ///<strong>Returns:</strong> The value needed to modify a variable
        ///</summary>
        public object GetModifierValue()
        {
            return a_value;
        }

        ///<summary>
        ///<c>SetActionMethod</c> sets the value of the action
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="val"/>: The value to be set (Type: <seealso cref="object"/>)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public void SetModifierValue(object val)
        {
            a_value = val;
        }

        ///<summary>
        ///<c>SetModifier</c> sets the modifier string of the action
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="val"/>: The value to be set (Type: <seealso cref="string"/>)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public void SetModifier(string val)
        {
            a_modifier = val;
        }

        /// <summary>
        /// <c>GetModifier</c> get the modifier string of the action
        /// </summary>
        /// <returns>the modifier string of the action</returns>
        public string GetModifier()
        {
            return a_modifier;
        }

        ///<summary>
        ///<c>GetObjectType</c> returns the Type of the object of the action
        ///<para/>
        ///<strong>Returns:</strong> The Type of the object
        ///</summary>
        public Type GetObjectType()
        {
            if (a_object != null)
                return a_object.GetType();
            else return null;
        }

        ///<summary>
        ///<c>GetModifierValueType</c> returns the Type of the value element of the action
        ///<para/>
        ///<strong>Returns:</strong> The Type of the value element
        ///</summary>
        public Type GetModifierValueType()
        {
            if (a_value != null)
                return a_value.GetType();
            else return null;
        }

        public String GetMeasureUnit()
        {
            return a_unit;
        }

        public void SetMeasureUnit(string unit)
        {
            this.a_unit = unit;
        }

        public override string ToString()
        {
            return a_subject + " " + a_verb + " " + a_object + " " + a_modifier + " " + a_value;
        }

        //Override vari per controllare l'uguaglianza tra Azioni
        public override bool Equals(object obj)
        {
            return ActionEquals(obj as Action);
        }

        public static bool operator ==(Action one, Action two)
        {
            return one.ActionEquals(two);
        }

        public static bool operator !=(Action one, Action two)
        {
            return !one.ActionEquals(two);
        }

        //TODO null action
        private bool ActionEquals(Action action)
        {
            if (!a_subject.Equals(action.GetSubject()))
                return false;
            if (!a_verb.Equals(action.GetActionMethod()))
                return false;
            if (a_object != null && !a_object.Equals(action.GetObject()))
                return false;
            if (a_value != null && !a_value.Equals(action.GetModifierValue()))
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public List<MethodInfo> GetMethod()
        {
            return method;
        }

        public List<FieldInfo> GetField()
        {
            return field;
        }

        public List<Component> GetSubjectComponent()
        {
            return subject;
        }

        [CanBeNull]
        public List<Component> GetObjectComponent()
        {
            return objct;
        }

        public bool AreThereAnyObjects()
        {
            return objects;
        }

        public bool IsValid()
        {
            return GetActionType() != ActionType.INVALID;
        }

        public ActionType GetActionType()
        {
            type = ActionType.INVALID;
            bool changed = false;
            bool passive = true;
            //Clear the old lists if this method is called another time on the same Action
            method.Clear();
            field.Clear();
            subject.Clear();
            objct.Clear();
            //Si va alla ricerca di un componente che abbia come Attributo "ECARules4All"
            if (GetSubject() != null)
            {
                ECAObject toTest = GetSubject().GetComponent<ECAObject>();
                foreach (Component c in GetSubject().GetComponents<Component>())
                {
                    Type cType = c.GetType();
                    if (Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
                    {
                        // we found a ECARules4All managed type
                        //Se la regola ha un valore nel campo ModifierValue significa che sta modificando una variabile
                        //Dopo aver controllato la correttezza dei tipi si provvede alla modifica tramite assegnamento o incremento/decremento
                        //sulla base del verbo descritto nell'azione
                        if (GetModifierValueType() != null)
                        {
                            foreach (MethodInfo m in cType.GetMethods())
                            {
                                ActionAttribute[] actions =
                                    (ActionAttribute[]) m.GetCustomAttributes(typeof(ActionAttribute), true);
                                foreach (ActionAttribute a in actions)
                                {
                                    if (a.Verb == GetActionMethod() && a.SubjectType == c.GetType() &&
                                        a.variableName == (string) GetObject() &&
                                        (a.ValueType == GetModifierValueType() || a.ValueType.IsSubclassOf(GetModifierValueType()) || GetModifierValueType().IsSubclassOf(a.ValueType)))
                                    {
                                        method.Add(m);
                                        subject.Add(c);
                                        if (!changed)
                                        {
                                            type = ActionType.CUSTOMCHANGE;
                                            changed = true;
                                        }

                                        break;
                                    }
                                }
                            }


                            foreach (FieldInfo f in cType.GetFields())
                            {
                                StateVariableAttribute[] variables =
                                    (StateVariableAttribute[]) f.GetCustomAttributes(typeof(StateVariableAttribute),
                                        true);
                                foreach (StateVariableAttribute a in variables)
                                {
                                    if (a.Name == (string) GetObject() && f.FieldType == GetModifierValueType())
                                    {
                                        passive = false;
                                        /* It is necessary to explain how this bit of code works:
                                         In order to apply a variable change we need to know which verb the rule refers to,
                                         this verb is stored into the "ECARules4AllOperations" file, inside this file there's
                                         a Dictionary of supported verbs and what they want to instruct to the RuleEngine.
                                         We try to get a translation for the verb, if it exists then we check if the variable's type
                                         inside an ECAObject/Behaviour supports that kind of operation, and if so we proceed to modify
                                         it according to the verb written in the rule
                                         */
                                        string verb;
                                        string[] supportedOps;
                                        ECARules4AllOperations.operationAliases.TryGetValue(GetActionMethod(),
                                            out verb);
                                        ECARules4AllOperations.supportedOperations.TryGetValue(a.type,
                                            out supportedOps);
                                        if (supportedOps.Contains(verb))
                                        {
                                            field.Add(f);
                                            subject.Add(c);
                                            switch (verb)
                                            {
                                                case "changes":
                                                    if (!changed)
                                                    {
                                                        type = ActionType.CHANGES;
                                                        changed = true;
                                                    }

                                                    break;
                                                case "increases":
                                                case "decreases":
                                                    if (!changed)
                                                    {
                                                        type = ActionType.INCREMENTSDECREMENTS;
                                                        changed = true;
                                                    }

                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                            //Se non c'é il suddetto campo popolato significa che é una regola che richiama funzioni, si controlla quindi se la funzione é presente nel
                            //componente, e nel caso si esegue, sempre dopo aver controllato se i tipi sono compatibili
                        {
                            if (toTest.isActive == true || GetActionMethod() == "activates" &&
                                !(toTest.isActive == true) && c is ECAObject)
                            {
                                // se l'oggetto è attivo oppure l'oggetto è non attivo ma il verbo coincide con activates di ECAObject...
                                foreach (MethodInfo m in cType.GetMethods())
                                {
                                    ActionAttribute[] actions =
                                        (ActionAttribute[]) m.GetCustomAttributes(typeof(ActionAttribute), true);
                                    foreach (ActionAttribute a in actions)
                                    {
                                        if (a.Verb == GetActionMethod())
                                        {
                                            passive = false;
                                            if (a.SubjectType == c.GetType())
                                            {
                                                if (GetObjectType() != null)
                                                {
                                                    if (GetObject() is GameObject && ((GameObject) GetObject())
                                                        .GetComponent<ECAObject>().isActive)
                                                    {
                                                        if (a.ObjectType == GetObjectType())
                                                        {
                                                            method.Add(m);
                                                            subject.Add(c);
                                                            if (!changed)
                                                            {
                                                                type = ActionType.OBJECT;
                                                                changed = true;
                                                            }

                                                            break;
                                                        }

                                                        foreach (Component cObj in ((GameObject) GetObject())
                                                            .GetComponents<Component>())
                                                        {
                                                            if (cObj.GetType() == a.ObjectType)
                                                            {
                                                                method.Add(m);
                                                                subject.Add(c);
                                                                objct.Add(cObj);
                                                                objects = true;
                                                                if (!changed)
                                                                {
                                                                    type = ActionType.OBJECT;
                                                                    changed = true;
                                                                }

                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else if (a.ObjectType == GetObjectType())
                                                    {
                                                        method.Add(m);
                                                        subject.Add(c);
                                                        if (!changed)
                                                        {
                                                            type = ActionType.VALUE;
                                                            changed = true;
                                                        }

                                                        break;
                                                    }
                                                }
                                                else if (GetObjectType() == null)
                                                {
                                                    if (m.GetParameters().Length == 0)
                                                    {
                                                        method.Add(m);
                                                        subject.Add(c);
                                                        if (!changed)
                                                        {
                                                            type = ActionType.VERB;
                                                            changed = true;
                                                        }

                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (passive && GetObject() is GameObject)
                                {
                                    ECAObject actionObject = ((GameObject) GetObject()).GetComponent<ECAObject>();
                                    if (actionObject != null && actionObject.isActive == true)
                                    {
                                        foreach (Component cPass in ((GameObject) GetObject())
                                            .GetComponents<Component>())
                                        {
                                            Type cTypePass = cPass.GetType();
                                            if (Attribute.IsDefined(cTypePass, typeof(ECARules4AllAttribute)))
                                            {
                                                foreach (MethodInfo mPass in cTypePass.GetMethods())
                                                {
                                                    ActionAttribute[] actionsPass =
                                                        (ActionAttribute[]) mPass.GetCustomAttributes(
                                                            typeof(ActionAttribute), true);
                                                    foreach (ActionAttribute aPass in actionsPass)
                                                    {
                                                        if (aPass.Verb == GetActionMethod())
                                                        {
                                                            if (aPass.SubjectType == cType)
                                                            {
                                                                method.Add(mPass);
                                                                subject.Add(cPass);
                                                                objct.Add(c);
                                                                objects = true;
                                                                if (!changed)
                                                                {
                                                                    type = ActionType.PASSIVE;
                                                                    changed = true;
                                                                }

                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return type;
        }
    }

    ///<summary>
    ///Interface used for interacting with the <seealso cref="EventBus"/>
    ///</summary>
    public interface ActionListener
    {
        //ActionPerformed (Action a)
        //public void ActionPerformed(System.Object s, string verb, System.Object o)
        ///<summary>
        ///<c>ActionPerformed</c> is called when an <seealso cref="Action"/> is executed and notified with a <seealso cref="EventBus.Publish(Action)"/>
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="a"/>: The action that the EventBus notified to be executed</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        void ActionPerformed(Action a);
    }

    ///<summary>
    ///Class that manages all the messages sent inside the system
    ///</summary>
    public class EventBus
    {
        public static EventBus singleton;
        private Dictionary<Action, ActionListener> eventQueue = new Dictionary<Action, ActionListener>();

        ///<summary>
        ///<c>GetInstance</c> returns an Instance of the EventBus. 
        ///<para/>
        ///<strong>Returns:</strong> The Singleton instance for the Event Bus
        ///</summary>
        public static EventBus GetInstance()
        {
            if (singleton == null)
            {
                singleton = new EventBus();
            }

            return singleton;
        }

        private EventBus()
        {
        }

        ///<summary>
        ///<c>Publish</c> sends an <seealso cref="Action"/> inside the EventBus, ready to be caught by any <seealso cref="ActionListener"/> who registered for catching it. 
        ///</summary>
        public void Publish(Action a)
        {
            foreach (KeyValuePair<Action, ActionListener> saved in eventQueue)
            {
                if (saved.Key == a)
                {
                    saved.Value.ActionPerformed(a);
                }
            }
        }

        ///<summary>
        ///<c>Subscribe</c> registers an <seealso cref="ActionListener"/> with an <seealso cref="Action"/> so it can be notified in case the action gets executed. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="a"/>: The action that the EventBus have to notify in case of execution</description></item>
        ///<item><description><paramref name="l"/>: The listener that will handle the notification</description></item>
        ///</list>
        ///</summary>
        public void Subscribe(Action a, ActionListener l)
        {
            //This function checks whether if a key is already inside the EventQueue, if it's not it's added to the list
            //It's done like this because the equivalent library function doesn't work for some reason :)
            //The check is also in place because even if a Dictionary SHOULDN'T have more than one key with the same value
            //the system still adds it, duplicating it and creating unwanted errors
            var existenceCheck = true;
            foreach (var pair in eventQueue)
            {
                if (pair.Key == a)
                {
                    existenceCheck = false;
                }
            }

            if (existenceCheck)
                eventQueue.Add(a, l);
        }

        ///<summary>
        ///<c>Unsubscribe</c> unregisters an <seealso cref="Action"/> from the system, if needed. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="a"/>: The action to be removed</description></item>
        ///</list>
        ///</summary>
        public void Unsubscribe(Action a)
        {
            eventQueue.Remove(a);
        }
    }
}