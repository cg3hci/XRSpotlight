using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECAScripts.Utils;
using UnityEngine;

namespace EcaRules
{
    /// <summary>
    /// <c>Condition</c> is the superclass used by the <seealso cref="EcaRule"/> class for defining checks for rule execution
    /// <para/>
    /// NOTE: Use <seealso cref="SimpleEcaCondition"/> for simple statements and <seealso cref="CompositeEcaCondition"/> for lists of Conditions
    /// </summary>
    [Serializable]
    public abstract class EcaCondition
    {
        public EcaCondition parent { get; set; }

        /// <summary>
        /// <c>Evaluate</c> returns the result of the evaluation of the statements contained inside of it.
        /// </summary>
        /// <strong>Returns:</strong> A boolean containing the result of the evaluation
        /// <para/>
        public abstract bool Evaluate();


        /// <summary>
        /// <c>IsLeaf</c> defines if this node is a leaf or not.
        /// <para/>
        /// <strong>Returns:</strong> A boolean that defines if this node is a leaf or not
        /// </summary>
        public abstract bool IsLeaf();
        
        /// <summary>
        /// <c>GetParent</c> returns the parent node of this object 
        /// <para/>
        /// <strong>Returns:</strong> The parent <seealso cref="EcaCondition"/> object for this one.
        /// </summary>
        public EcaCondition GetParent()
        {
            return parent;
        }
    }
    
    [Serializable]
    public class CompositeEcaCondition : EcaCondition
    {
        public enum ConditionType
        {
            NONE,
            OR,
            AND,
            NOT
        }

        private ConditionType op;
        private List<EcaCondition> children;

        public ConditionType Op
        {
            get { return op; }
            set
            {
                op = value;
                CheckChildrenCount();
            }
        }

        public CompositeEcaCondition()
            : this(ConditionType.NONE, new List<EcaCondition>())
        {
        }

        public CompositeEcaCondition(ConditionType type, List<EcaCondition> conditions)
        {
            children = conditions;
            Op = type;

            foreach (EcaCondition child in children)
            {
                child.parent = this;
            }
        }

        public EcaCondition GetChild(int i)
        {
            if (i >= 0 && i < children.Count())
            {
                return children[i];
            }

            return null;
        }

        public void AddChild(EcaCondition c)
        {
            c.parent = this;
            children.Add(c);
            CheckChildrenCount();
        }

        public void InsertChild(int index, EcaCondition c)
        {
            c.parent = this;
            children.Insert(index, c);
            CheckChildrenCount();
        }

        public void RemoveChild(EcaCondition c)
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
        public IEnumerable<EcaCondition> Children()
        {
            foreach (EcaCondition c in children)
            {
                yield return c;
            }
        }

        public override bool Evaluate()
        {
            bool res;
            bool temp = Op == ConditionType.AND ? true : false;

            foreach (EcaCondition c in children)
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
    ///<c>SimpleCondition</c> defines a simple condition to be checked, this is a subclass of <seealso cref="EcaCondition"/> node. 
    ///</summary>
    [Serializable]
    public class SimpleEcaCondition : EcaCondition
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
        public SimpleEcaCondition(GameObject component, string property, string symbol, object compareWith)
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
}