using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Antlr4.Runtime.Misc;
using EcaRules;
using ECAScripts;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Examples.UIRule.Prefabs;
using Behaviour = EcaRules.Behaviour;
using Object = UnityEngine.Object;
using System.IO;


public class RuleUtils : MonoBehaviour
{
    public struct RulesStruct
    {
        public GameObject prefab;
        public EcaRule EcaRule;
        public ButtonsHandle.RuleString ruleString;

        public RulesStruct(GameObject prefab, EcaRule ecaRule, ButtonsHandle.RuleString ruleString)
        {
            this.prefab = prefab;
            this.EcaRule = ecaRule;
            this.ruleString = ruleString;
        }
    }

    // Dictionary mapping GameObjects and colors for the interface
    public static Dictionary<GameObject, Color> interfaceObjectColors = new Dictionary<GameObject, Color>();

    //Dictionary of Rules
    public static Dictionary<string, RulesStruct> rulesDictionary =
        new Dictionary<string, RulesStruct>();
    
    public static ButtonsHandle.RuleString ConvertRuleObjectToRuleString(EcaRule ecaRule, string ruleText)
    {
        ButtonsHandle.RuleString ruleString = new ButtonsHandle.RuleString() { };
        List<ButtonsHandle.StringAction> actionString = new List<ButtonsHandle.StringAction>();
        List<ButtonsHandle.StringCondition> conditionString = new List<ButtonsHandle.StringCondition>();
        ButtonsHandle.StringAction eventString = new ButtonsHandle.StringAction();
    
        //When action
        EcaAction eventRule = ecaRule.GetEvent();
        //find when row in the text
        string whenString = FindElementInText(ruleText, "when");
        //convert to StringAction
        eventString = ConvertActionToString(eventRule, whenString);
    
        //First Then action
        List<EcaAction> listOfActions = ecaRule.GetActions();
        //using a regex I find all the actions in the file searching for anything that starts with "the" and ends with ";"
        Regex rgx = new Regex("(?<=the\\s)(.*?)(?=;)");
        int i = 0;
        foreach (Match match in rgx.Matches(ruleText))
        {
            actionString.Add(ConvertActionToString(listOfActions[i], match.Value));
            i++;
        }
        
        //Conditions
        EcaCondition ecaCondition = ecaRule.GetCondition();
        if (ecaCondition!=null)
        {
            if (ecaCondition.GetType() == typeof(SimpleEcaCondition))//one condition
            {
                //using a regex I find the condition in the rule searching for anything that starts with "if" and ends with "\n"
                string ifcondition = Regex.Match(ruleText, "(?<=if\\s)(.*?)(?=\\n)").Groups[0].Value;
                conditionString.Add(ConvertConditionToString((SimpleEcaCondition)ecaCondition, ifcondition, null));
            } 
                
            else //multiple conditions
            {
                CompositeEcaCondition ccondition = ecaCondition as CompositeEcaCondition;
                conditionString = ConvertCompositeCondition(ccondition, ruleText);
            }
        }

        ruleString.EventString = eventString;
        ruleString.ActionsString = actionString;
        ruleString.Conditions = conditionString;
        return ruleString;
    }
    
    
    private static ButtonsHandle.StringCondition ConvertConditionToString(SimpleEcaCondition ecaCondition, string stringText, string andOr)
    {
        ButtonsHandle.StringCondition stringCondition = new ButtonsHandle.StringCondition();
        //toCheck
        string toCheck = ecaCondition.GetSubject().name;
        string toCheckType = Regex.Match(stringText, "\\w+(?=\\s+"+toCheck+")").Groups[0].Value;
        stringCondition.ToCheck = FirstCharToUpper(toCheckType) + " " + toCheck;
    
        //property
        stringCondition.Property = ecaCondition.GetProperty();
    
        //checksymbol
        stringCondition.CheckSymbol = ecaCondition.GetSymbol();
    
        //comparewith
        string compareWith = ecaCondition.GetValueToCompare().ToString();
        //the word before comparewith, it can be a type (e.g. color blue -> color) or 
        //it can only the comparewith (e.g. is on)
        // The regex extracts the word BEFORE compareWith
        string objType = Regex.Match(stringText, "\\w+(?=\\s+"+compareWith+")").Groups[0].Value;
        if (objType != stringCondition.CheckSymbol) 
        {
            stringCondition.CompareWith = objType + " " + compareWith;
        }
        else stringCondition.CompareWith = compareWith;
        if (andOr != null) stringCondition.AndOr = andOr;
    
        return stringCondition;
    }

    private static List<ButtonsHandle.StringCondition> ConvertCompositeCondition(CompositeEcaCondition ecaCondition,
        string stringText)
    {

        List<ButtonsHandle.StringCondition> stringConditions = new List<ButtonsHandle.StringCondition>();
        ;
        string completecondition = (Regex.Match(stringText, "(?<=\\r\\nif\\s)(.*?)(?=\\r\\n)").Groups[0].Value);
       
        string[] split = completecondition.Split(new[] {" and ", " or "}, StringSplitOptions.RemoveEmptyEntries);
        GroupCollection andOrs = Regex.Match(stringText, "(\\sand\\s|\\sor\\s)").Groups;
        
        int i = 0;
        foreach (var c in ecaCondition.Children())
        {
            if (!string.IsNullOrEmpty(split[i])) ;
            {
                string andOrNull = null;
                if (i != 0) andOrNull = andOrs[i - 1].Value;
                stringConditions.Add(ConvertConditionToString((SimpleEcaCondition) c, split[i], andOrNull));
            }
            i++;
        }

        return stringConditions;
    }
    
    static string FindElementInText(string rule, string elem)
    {
        return Regex.Match(rule, elem + " .*?\n").Groups[0].Value;
    }
    
    public static string FirstCharToUpper(string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input.First().ToString().ToUpper() + input.Substring(1)
        };
    
    
    public static ButtonsHandle.StringAction ConvertActionToString(EcaAction ecaAction, string stringText)
    {
        ButtonsHandle.StringAction stringAction = new ButtonsHandle.StringAction();
    
        //subject
        string subj = ecaAction.GetSubject().name;
        string subjType = Regex.Match(stringText, "\\w+(?=\\s+"+subj+")").Groups[0].Value;
        stringAction.Subj = FirstCharToUpper(subjType) + " " + subj;
    
        //verb
        stringAction.Verb = ecaAction.GetActionMethod();
    
        //object
        if (ecaAction.GetObject() != null && ecaAction.GetModifierValue()==null)
        {
            //object name
            string obj = ecaAction.GetObject().ToString();
            
            // Remove (UnityEngine.GameObject) from "obj" (e.g.: obj = "bigButton1 (UnityEngine.GameObject)")
            obj = obj.Split('(')[0];
            obj = obj.Remove(obj.Length-1, 1);
            //the word before object, it can be a type (e.g. interacts with the character x -> character) or 
            //it can be the verb (e.g. turns on)
            string objType = Regex.Match(stringText, "\\w+(?=\\s+"+obj+")").Groups[0].Value;

            if (objType != stringAction.Verb) //maybe not needed
            {
                stringAction.Obj = FirstCharToUpper(objType) + " " + obj;
            }
            else stringAction.Obj = obj;
            
            //check if there is a "the" before the object
            int theOccurrences = Regex.Matches(stringText, " the ").Count;
            if (theOccurrences>0) stringAction.ObjThe = "The";
            
        }

        //object + value
        if (ecaAction.GetModifierValue() != null)
        {
            //object name
            string obj = ecaAction.GetObject().ToString();
            stringAction.Obj = obj;
            stringAction.Prep = ecaAction.GetModifier();
            // stringAction.Value = Regex.Match(stringText, "\\w+(?=\\s+"+stringAction.Value+")").Groups[0].Value;
            // (?<=to\s).*  /// Picks everything after "to"

            stringAction.Value = Regex.Match(stringText, "(?<=" + stringAction.Prep + "\\s).*").Groups[0].Value.Split(' ')[0];

        }
        return stringAction;
    }

    //Some verbs have more action attributes but inherit from two different ecascript, but they are the same, so we check if the verb
    //is equal
    public static bool SameAttributesList(List<ActionAttribute> list)
    {
        string previousVerb = list[0].Verb;
        bool flag = true;
        foreach (var act in list)
        {
            flag = flag && (act.Verb == previousVerb);
        }

        if (flag && !string.IsNullOrEmpty(list[0].variableName))
        {
            string previousVariableName = list[0].variableName;
            foreach (var vAttribute in list)
            {
                flag = flag && (vAttribute.variableName == previousVariableName);
            }
        }
        return flag;
    }

    public static Dictionary<int, Dictionary<GameObject, string>> FindSubjects()
    {
        //ref to gameObject and inner type of ecaComponent
        var oldResult = new Dictionary<GameObject, string>();
        var result = new Dictionary<int, Dictionary<GameObject, string>>();

        //the subjects are eca components inside the scene
        var foundSubjects = FindObjectsOfType<ECAObject>();
        
        int i = 0;
        //foreach gameobject found with the ecaobject script
        foreach (var ecaObject in foundSubjects)
        {
            string type = FindInnerTypeNotBehaviour(ecaObject.gameObject);
            Dictionary<GameObject, string> dictionary = new Dictionary<GameObject, string>()
                {{ecaObject.gameObject, type}};
            result.Add(i, dictionary);
            i++;
        }

        return result;
    }

    /// <summary>
    /// Returns behaviour children
    /// </summary>
    /// <param name="listOfEcaAttributes"></param>
    /// <returns></returns>
    static ArrayList<Type> FindBehaviourChildrenAmongEcaAttributes(List<Type> listOfEcaAttributes)
    {
        ArrayList<Type> behaviours = new ArrayList<Type>();
        foreach (var type in listOfEcaAttributes)
        {
            RequireComponent[] requiredComponentsAtts = Attribute.GetCustomAttributes(type,
                typeof(RequireComponent), true) as RequireComponent[];

            if (requiredComponentsAtts.Length > 0) //e.g. interactable has two requires
            {
                if (requiredComponentsAtts[0] != null &&
                    requiredComponentsAtts[0].m_Type0 == typeof(Behaviour)) //behaviour children
                {
                    behaviours.Add(type);
                }
            }
        }

        return behaviours;
    }

    public static string FindInnerTypeNotBehaviour(GameObject gameObject)
    {
        //retrieve list of EcaAttributes of the gameobject
        List<Type> listOfEcaAttributes = RetrieveECAAttributes(gameObject);

        //from here we search among the attributes and we create another list without 
        //the behaviour and the children of the behaviour

        //first, we search for the behaviour
        Type behaviour = FindBehaviourAmongEcaAttributes(listOfEcaAttributes);
        if (behaviour != null)
        {
            listOfEcaAttributes.Remove(behaviour);
            //if there is a behaviour, probably there will be the children
            ArrayList<Type> behaviourChildren = FindBehaviourChildrenAmongEcaAttributes(listOfEcaAttributes);
            if (behaviourChildren != null)
            {
                foreach (var beh in behaviourChildren)
                {
                    listOfEcaAttributes.Remove(beh);
                }

                
            }
        }

        return FindTheInnerOne(listOfEcaAttributes);
        ;
    }

    static List<Type> RetrieveECAAttributes(GameObject gameObject)
    {
        List<Type> listOfEcaAttributes = new List<Type>();
        Component[] listOfComponents = gameObject.GetComponents<Component>();
        foreach (Component c in listOfComponents)
        {
            Type cType = c.GetType();

            //searching for the components of type ecarules
            if (Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
            {
                //take all the feasible components
                listOfEcaAttributes.Add(cType);
            }
        }

        return listOfEcaAttributes;
    }

    static Type FindBehaviourAmongEcaAttributes(List<Type> listOfEcaAttributes)
    {
        foreach (var type in listOfEcaAttributes)
        {
            if (type.Name.Equals("Behaviour"))
            {
                return type;
            }
        }

        return null;
    }

    //checks if the gameobject has an entry for behaviour or not
    static bool checkIfBehaviour(GameObject g, Dictionary<int, Dictionary<GameObject, string>> subjects)
    {
        //if the behaviour is present it means that there are two entries of the dictionary with the same gameobject
        int i = 0;
        foreach (var s in subjects)
        {
            foreach (var ss in subjects[i])
            {
                if (ss.Key == g)
                {
                    i++;
                }
            }
        }

        return i > 1;
    }

    //we pass bool passive when we have to retrieve passive verbs
    public static Dictionary<int, VerbComposition> FindActiveVerbs(GameObject subjSelected,
        Dictionary<int, Dictionary<GameObject, string>> subjects, [CanBeNull] string selectedType,
        bool passive)
    {
        Dictionary<int, VerbComposition> result = new Dictionary<int, VerbComposition>();
        int i = 0;
        bool behaviourExist = checkIfBehaviour(subjSelected, subjects);

        foreach (Component c in subjSelected.GetComponents<Component>())
        {
            Type cType = c.GetType();

            //searching for the components of type ecarules
            if (Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
            {
                if (behaviourExist)
                {
                    //foreach component we find the verbs
                    var componentVerbs = ListActionsItem(cType);
                    foreach (var el in componentVerbs)
                    {
                        result.Add(i, el);
                        i++;
                    }
                }
                else
                {
                    //foreach component we find the verbs
                    var componentVerbs = ListActionsItem(cType);
                    foreach (var el in componentVerbs)
                    {
                        //for example, food has the verb eats, that has as subject Character, we don't
                        //want to add it to the verbs of food
                        if (passive)
                        {
                            if (el.ActionAttribute.SubjectType.Name == cType.Name)
                            {
                                result.Add(i, el);
                                i++;
                            }
                        }
                        else
                        {
                            result.Add(i, el);
                            i++;
                        }
                    }
                }

                //result = result.Concat(componentVerbs).ToDictionary(s => s.Key, s => s.Value);
            }
        }

        /*Debug.Log("Verbs: ");
        foreach (KeyValuePair<int, VerbComposition> kvp in result)
        {
            Debug.Log( string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value.Verb + kvp.Value.ActionAttribute) );
        }*/
        return result;
    }

    public static void FindPassiveVerbs(GameObject subjSelected,
        Dictionary<int, Dictionary<GameObject, string>> subjects, string selectedType,
        ref Dictionary<int, VerbComposition> activeVerbs)
    {
        List<string> ecaScriptOfTheGameobject = FindECAScripts(subjSelected);
        foreach (var subj in subjects)
        {
            foreach (var var in subj.Value)
            {
                if (var.Key != subjSelected && var.Value != selectedType)
                {
                    //we pass "passive" as false in order to include in the verbs of each subject even those who don't
                    //have itself as subject. (e.g. among Food verbs there will be Eats)
                    Dictionary<int, VerbComposition> verbs = FindActiveVerbs(var.Key, subjects, null, false);
                    //foreach verb of each subject
                    foreach (var v in verbs)
                    {
                        //selected type is the inner ecascript selected, but an animal is also a character,
                        //so we need to find also the verbs of the superior hierarchy, so we check with
                        //all the ecaScript
                        foreach (var script in ecaScriptOfTheGameobject)
                        {
                            if (v.Value.ActionAttribute.SubjectType.Name == script)
                            {
                                //It doesn't already exist in my list
                                if(!DictionaryContainsValue(activeVerbs, v.Value)) 
                                {
                                    int lastIndex = activeVerbs.Count - 1;
                                    activeVerbs.Add(lastIndex + 1, v.Value);
                                }
                            }
                               
                        }
                        
                    }
                }
            }
        }
    }

    private static List<string> FindECAScripts(GameObject gameObject)
    {
        List<string> result = new List<string>();
        foreach (Component c in gameObject.GetComponents<Component>())
        {
            Type cType = c.GetType();

            //searching for the components of type ecarules
            if (Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
            {
                result.Add(cType.Name);
            }
        }

        return result;
    }

    private static bool DictionaryContainsValue(Dictionary<int, VerbComposition> verbs, VerbComposition value)
    {
        foreach (var var in verbs)
        {
            if (var.Value.ActionEquals(value))
            {
                return true;
            }
        }

        return false;
    }

    public static string FindTheInnerOne(List<Type> listEcaComponents)
    {
        Dictionary<int, string> depts = new Dictionary<int, string>();
        foreach (var comp in listEcaComponents)
        {
            int dept = GetDepth(comp, 0);
            depts.Add(dept, comp.Name);
        }

        var maxKey = depts.Keys.Max();
        return depts[maxKey];
    }

    public static Dictionary<string,List<ActionAttribute>> PopulateVerbsString( Dictionary<int,VerbComposition> verbsItem)
    {
        Dictionary<string, List<ActionAttribute>> result = new Dictionary<string, List<ActionAttribute>>();
        foreach (KeyValuePair<int, VerbComposition> entry in verbsItem)
        {
            //e.g.verbs like "changes" can have multiple action attributes (visible, active...)
            if (result.ContainsKey(entry.Value.ActionAttribute.Verb))
            {
                result[entry.Value.ActionAttribute.Verb].Add(entry.Value.ActionAttribute);
            }

            else
            {
                List<ActionAttribute> hashSet = new List<ActionAttribute>();
                hashSet.Add(entry.Value.ActionAttribute);
                result.Add(entry.Value.ActionAttribute.Verb, hashSet);
            }
        }

        return result;
    }

    /// <summary>
    /// Returns the dept of a ecarules component
    /// </summary>
    /// <param name="c"></param> type of a component
    /// <param name="depth"></param> the starting depth (0 if we want to search from ecaobject)
    /// <returns>the dept of a ecacomponent</returns>
    private static int GetDepth(MemberInfo c, int depth = 0)
    {
        MemberInfo info = c;
        object[] attributes = info.GetCustomAttributes(true);
        for (int i = 0; i < attributes.Length; i++)
        {
            if (attributes[i] is RequireComponent)
            {
                MemberInfo new_info = ((RequireComponent) attributes[i]).m_Type0;
                return GetDepth(new_info, depth + 1);
            }
        }

        return depth;
    }

    ///<summary>
    ///<c>ListActions</c> returns all the ActionAttribute tagged variables. 
    ///<para/>
    ///<strong>Parameters:</strong> 
    ///<list type="bullet">
    ///<item><description><paramref name="c"/>: The Type to check</description></item>
    ///</list>
    ///<para/>
    ///<strong>Returns:</strong> A dictionary of the string of the action and the object type (Position, Rotation...)
    ///</summary>
    public static List<VerbComposition> ListActionsItem(Type c)
    {
        List<VerbComposition> actions = new List<VerbComposition>();
        foreach (MethodInfo m in c.GetMethods())
        {
            var a = m.GetCustomAttributes(typeof(ActionAttribute), true);
            if (a.Length > 0)
            {
                foreach (var item in a)
                {
                    ActionAttribute ac = (ActionAttribute) item;
                    if (ac.ObjectType != null)
                    {
                        VerbComposition verbComposition = new VerbComposition(ac.ObjectType.ToString(), ac);
                        actions.Add(verbComposition);
                    }
                    else //siamo nel caso in cui abbiamo verbi tipo cambia visibilità a 
                    {
                        VerbComposition verbComposition = new VerbComposition(ac.variableName, ac);
                        actions.Add(verbComposition);
                    }
                }
            }
        }

        return actions;
    }

    /**
     * Returns for each state variable the name and the ECARules4AllType
     */
    public static Dictionary<string, ( ECARules4AllType, Type)> FindStateVariables(GameObject gameObject)
    {
        Dictionary<string, ( ECARules4AllType, Type)> variables = new Dictionary<string, ( ECARules4AllType, Type)>();

        foreach (Component c in gameObject.GetComponents<Component>())
        {
            Type cType = c.GetType();

            //searching for the components of type ecarules
            if (Attribute.IsDefined(cType, typeof(ECARules4AllAttribute)))
            {
                //foreach component we find the verbs
                var componentVariables = ListStateVariables(cType);
                // foreach (var var in componentVariables)
                // {
                //     if(!variables.ContainsKey(var.Key)) variables.Add(var.Key, (var.Value, cType));
                // }
                variables = variables.Concat(componentVariables).ToDictionary(s => s.Key, s => s.Value);
             
            }
        }

        return variables;
    }


    public static Dictionary<string, (ECARules4AllType, Type)> ListStateVariables(Type cType)
    {
        Dictionary<string, (ECARules4AllType, Type)> variables = new Dictionary<string, (ECARules4AllType, Type)>();
        foreach (FieldInfo m in cType.GetFields())
        {
            object[] a = m.GetCustomAttributes(typeof(StateVariableAttribute), true);
            if (a.Length > 0)
            {
                foreach (var item in a)
                {
                    StateVariableAttribute var = (StateVariableAttribute) item;
                    variables.Add(var.Name,( var.type, cType));
                }
            }
        }

        return variables;
    }


    public static void ChangeColorGameObjectDropdown(GameObject gameObject, Transform rowTransform, GameObject previousSelected)
    {
        // between a given GameObject and a color to be used in the interface
        // Then, for each item assign the correct color to the dropdown via the function
        if (!interfaceObjectColors.Keys.Contains(gameObject))
        {
            // If the gameObject isn't in the dictionary we need to add it and assign it a color
            int numOfColors = DropdownHandler.reversedColorDict.Keys.Count;
            // The colors will repeat after a given number of item is used
            int idx = interfaceObjectColors.Keys.Count % numOfColors;
            // Get the color and add the mapping to the dictionary
            Color color = DropdownHandler.reversedColorDict.Keys.ElementAt(idx);
            interfaceObjectColors.Add(gameObject, color);
        }

        // Assign the color to the UI
        Color oColor = interfaceObjectColors[gameObject];
        if(gameObject!=previousSelected)
            outlineColor(gameObject, oColor);
        if(previousSelected!=null) 
            Destroy(previousSelected.GetComponent<ECAOutline>()); //if a subject has already been selected, remove the color
        rowTransform.GetComponent<Image>().color = oColor;
    }

    public static void outlineColor(GameObject gameObject, Color color)
    {
        ECAOutline ecaOutline = gameObject.GetComponent<ECAOutline>();
        if (ecaOutline == null)
        {
            ecaOutline = gameObject.AddComponent<ECAOutline>();
            ecaOutline.OutlineColor = color;
            ecaOutline.OutlineWidth = 5f;
        }
        else
        {
            ecaOutline.OutlineColor = color;
            ecaOutline.OutlineWidth = 5f;
        }
    }
    
    public static void printList(List<string> list)
    {
        foreach (var e in list)
        {
            Debug.Log(e);
        }
    }

    public static void printList(ECAObject[] list)
    {
        foreach (var e in list)
        {
            Debug.Log(e.ToString());
        }
    }

    public static void printList(Component[] list)
    {
        foreach (var e in list)
        {
            Debug.Log(e.ToString());
        }
    }

    public static void clearInputField(InputField inputfield)
    {
        inputfield.Select();
        inputfield.text = "";
    }

    public static string RemoveECAFromString(string ecaType)
    {
        return ecaType.Replace("ECA", "");
    }

    //When the object of the rule is not a component in the subject, we need to retrieve it from other gameobjects
    //e.g. character eats typeof(Food), the character is not also a food, so we need to find between all the
    //gameobjects the ones with Food component
    public static void AddObjectPassiveVerbs( Dictionary<int,Dictionary<GameObject,string>> subjects, string comp,
        Dropdown objDrop)
    {
        bool found = false;
        ArrayList<string> resArrayList = new ArrayList<string>();
                            
        foreach (var subj in subjects)
        {
            foreach (var var in subj.Value)
            {
                if (var.Key.GetComponent(comp) != null)
                {
                    found = true;
                    string type = FindInnerTypeNotBehaviour(var.Key);
                    type = RemoveECAFromString(type);
                    resArrayList.Add(type + " " + var.Key.name);
                }
            }
        }

        if (found)
        {
            // Used to sort each dropdown's options
            List<string> entries = new List<string>();
            
            // objDrop.options.Add(new Dropdown.OptionData(""));
            foreach (var option in resArrayList)
            {
                // objDrop.options.Add(new Dropdown.OptionData(option));
                entries.Add(option);
            }
            AddToDropdownInAlphabeticalOrder(objDrop, entries);
        }
    }

    public static void AddObjectActiveVerbs(Dictionary<int, Dictionary<GameObject, string>> subjects, string comp,
        Dropdown objDrop, GameObject subjectSelected)
    {
        objDrop.options.Add(new Dropdown.OptionData(""));
        // Used to sort each dropdown's options
        List<string> entries = new List<string>();
        
        for (int i = 0; i < subjects.Count; i++)
        {
            foreach (KeyValuePair<GameObject, string> entry in subjects[i])
            {
                //TODO handle alias
                if (entry.Key != subjectSelected && entry.Key.GetComponent(comp))
                {
                    string type = FindInnerTypeNotBehaviour(entry.Key);
                    type = RemoveECAFromString(type);
                    // objDrop.options.Add(new Dropdown.OptionData(type + " " + entry.Key.name));
                    entries.Add(type + " " + entry.Key.name);
                }
            }
        }
        AddToDropdownInAlphabeticalOrder(objDrop, entries);
    }

    public static void LoadRulesAndAddToUI(string path)
    {
        // Read the txt file containing the rules
        TextRuleParser ruleParser = new TextRuleParser();
        ruleParser.ReadRuleFile(path);
        
        // For each rule in the RuleEngine, add it to the RuleList (UI)
        /*foreach (Rule rule in RuleEngine.GetInstance().Rules())
        {
            GameObject prefab = ButtonsHandle.CreateRuleRow(null, rule);
            string newRuleUuid = Guid.NewGuid().ToString();
            prefab.name = newRuleUuid;
            
            if (!rulesDictionary.ContainsKey(newRuleUuid))
            {
                // Add to rulesDictionary
                // Need the UUID, the RuleStruct (prefab, rule, ruleString)
                GameObject ruleString2 = prefab.transform.GetChild(0).gameObject;
                string textRule = ruleString2.GetComponent<Text>().text;
               // ButtonsHandle.RuleString ruleString = ConvertRuleObjectToRuleString(rule, textRule);
                //rulesDictionary.Add(newRuleUuid, new RulesStruct(prefab, rule, ruleString));
            }
            
        }*/
        
        // Add to rulesDictionary
        // Need the UUID, the RuleStruct (prefab, rule, ruleString) [The ruleString will be temporarily null]
        
    }
    
    
    
    public static void AddToDropdownInAlphabeticalOrder(Dropdown dropdown, List<string> entries)
    {
        entries.Sort();
        foreach (var s in entries)
        {
            dropdown.options.Add(new Dropdown.OptionData( s));
        }
    }


    public static void SaveRulesToFile()
    {
        // Save the rules on the file
        TextRuleSerializer serializer = new TextRuleSerializer();
        // string path = Path.Combine("Assets", Path.Combine("Resources", "storedRules.txt"));
        //string path = Path.Combine(Directory.GetCurrentDirectory(), "storedRules.txt");
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "storedRules.txt");
        serializer.SaveRules(path);
    }
    
}