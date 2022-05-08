using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace EcaRules
{
     ///<summary>
    ///Describes an interaction inside the system
    ///</summary>
    public class EcaAction
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

        public EcaAction(){}

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
        public EcaAction(GameObject sub, string verb, object obj, string mod, object value) : this(sub, verb, obj)
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
        public EcaAction(GameObject sub, string verb, object obj) : this(sub, verb)
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
        public EcaAction(GameObject sub, string verb)
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
            return ActionEquals(obj as EcaAction);
        }

        public static bool operator ==(EcaAction one, EcaAction two)
        {
            return one.ActionEquals(two);
        }

        public static bool operator !=(EcaAction one, EcaAction two)
        {
            return !one.ActionEquals(two);
        }

        //TODO null action
        private bool ActionEquals(EcaAction ecaAction)
        {
            if (!a_subject.Equals(ecaAction.GetSubject()))
                return false;
            if (!a_verb.Equals(ecaAction.GetActionMethod()))
                return false;
            if (a_object != null && !a_object.Equals(ecaAction.GetObject()))
                return false;
            if (a_value != null && !a_value.Equals(ecaAction.GetModifierValue()))
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
    ///Interface used for interacting with the <seealso cref="EcaEventBus"/>
    ///</summary>
    public interface EcaActionListener
    {
        //ActionPerformed (Action a)
        //public void ActionPerformed(System.Object s, string verb, System.Object o)
        ///<summary>
        ///<c>ActionPerformed</c> is called when an <seealso cref="EcaAction"/> is executed and notified with a <seealso cref="EcaEventBus.Publish(EcaAction)"/>
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="a"/>: The action that the EventBus notified to be executed</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        void ActionPerformed(EcaAction a);
    }
}