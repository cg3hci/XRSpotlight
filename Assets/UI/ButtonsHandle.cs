using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using Antlr4.Runtime.Misc;
using EcaRules;
using ECAScripts;
using ECAScripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Examples.UIRule.Prefabs;
using Debug = UnityEngine.Debug;

public class ButtonsHandle : MonoBehaviour
{
    public GameObject actionPrefab;
    public GameObject simpleConditionPrefab;
    public GameObject compositeConditionPrefab;
    public Transform contentActionTransform;
    public Transform contentConditionTransform;

    public struct StringAction
    {
        private string subj;

        public string Subj
        {
            get => subj;
            set => subj = value;
        }

        public string Verb
        {
            get => verb;
            set => verb = value;
        }

        public string Obj
        {
            get => obj;
            set => obj = value;
        }

        public string Value
        {
            get => value;
            set => this.value = value;
        }

        private string verb;
        private string obj;
        private string value;
        private string prep;
        public string Prep
        {
            get => prep;
            set => this.prep = value;
        }
        private string objThe;
        public string ObjThe
        {
            get => objThe;
            set => this.objThe = value;
        }
    }

    public struct StringCondition
    {
        private string toCheck;
        private string andOr;

        public string AndOr
        {
            get => andOr;
            set => andOr = value;
        }

        public string ToCheck
        {
            get => toCheck;
            set => toCheck = value;
        }

        public string Property
        {
            get => property;
            set => property = value;
        }

        public string CheckSymbol
        {
            get => checkSymbol;
            set => checkSymbol = value;
        }

        public string CompareWith
        {
            get => compareWith;
            set => compareWith = value;
        }

        private string property;
        private string checkSymbol;
        private string compareWith;
    }

    public struct RuleString
    {
        private StringAction eventString;
        private List<StringCondition> conditions;
        private List<StringAction> actionsString;

        public RuleString(StringAction eventString, List<StringCondition> conditions, List<StringAction> actionsString)
        {
            this.eventString = eventString;
            this.conditions = conditions;
            this.actionsString = actionsString;
        }

        public StringAction EventString
        {
            get => eventString;
            set => eventString = value;
        }

        public List<StringCondition> Conditions
        {
            get => conditions;
            set => conditions = value;
        }

        public List<StringAction> ActionsString
        {
            get => actionsString;
            set => actionsString = value;
        }
    }

    // Rule prefab
    public GameObject rulePrefab;
    // public GameObject ruleListObj;
    // public GameObject viewportObj;

    // ArrayList containing the rules to be shown in the RuleList
    public ArrayList<string> ruleStrings = new ArrayList<string>();
    public static string modifyingRuleId = "";

    public void AddAction()
    {
        Instantiate(actionPrefab, contentActionTransform);
    }

    public void AddCondition()
    {
        //If a simple condition exists, we should instantiate a composite condition
        if (GameObject.Find("SimpleConditionPrefab(Clone)") && GameObject.Find("SimpleConditionPrefab(Clone)").activeInHierarchy)
        {
            Instantiate(compositeConditionPrefab, contentConditionTransform);
        }
        else
        {
            //else we must activate the simple condition and hierarchy
            GameObject parentConditionList = GameObject.Find("PartsBuilder");
            parentConditionList.transform.Find("ConditionList").gameObject.SetActive(true);

            GameObject parentHeaderCondition = GameObject.Find("Labels");
            parentHeaderCondition.transform.Find("_headerCondition").gameObject.SetActive(true);
            Instantiate(simpleConditionPrefab, contentConditionTransform);
        }
    }

    
    public EcaRule CreateRule()
    {
        EcaRule ecaRule = null; //result

        RuleString ruleString = new RuleString() { };
        List<StringAction> actionString = new List<StringAction>();
        List<StringCondition> conditionString = new List<StringCondition>();
        StringAction eventString = new StringAction();

        //When action
        EcaAction whenEcaAction = FindAction(GameObject.Find("Event"), ref eventString);
        // if (whenAction == null) return null;
        bool whenValid = whenEcaAction.IsValid();
        if (!whenValid){
            Debug.Log("Invalid when"); 
            return null; //the findAction returns null, so something is missing
        }

        //Actions
        // TODO: filter out objects with name "ActionPrefab (Clone)"
        var allActions = GameObject.FindGameObjectsWithTag("Action").ToList();
        var actions = from act in allActions where act.name != "ActionPrefab" select act;
        if (actions.Count() == 0) return null;

        bool thenValid = true;
        ArrayList<EcaAction> listOfActions = new ArrayList<EcaAction>();
        foreach (var action in actions)
        {
            StringAction singleAction = new StringAction();
            EcaAction thenEcaAction = FindAction(action, ref singleAction);
            if (thenEcaAction.IsValid())
            {
                actionString.Add(singleAction);
                listOfActions.Add(thenEcaAction);
            }
            else thenValid = false;
        }

        if (!thenValid){
            Debug.Log("Invalid then"); 
            return null; //if one of the action is not valid, the rule is null
        }
        

        //conditions
        bool condition = false;
        bool compositeConditions = false;

        //check if condition exists
        GameObject simpleC = GameObject.Find("SimpleConditionPrefab(Clone)");
        //I need to initialize the variables with standard values
        SimpleEcaCondition simpleEcaCondition = new SimpleEcaCondition(simpleC, "", "", "");
        CompositeEcaCondition finalEcaCondition = new CompositeEcaCondition();
        if (simpleC != null)
        {
            condition = true;
            simpleEcaCondition = FindSimpleCondition(simpleC);
            if (simpleEcaCondition.IsValid())
            {
                StringCondition simpleConditionString = CreateStringCondition(simpleC, simpleEcaCondition, false);
                conditionString.Add(simpleConditionString);
                //composite conditions
                compositeConditions = compositeConditionExists();
                if (compositeConditions)
                {
                    finalEcaCondition = CreateCompositeConditions(simpleEcaCondition);
                    var allCompositeConditionObjects = GameObject.FindGameObjectsWithTag("CompositeCondition").ToList();
                    var compositeConditionObjects = from act in allCompositeConditionObjects 
                        where act.name != "CompositeConditionPrefab" select act;
                    foreach (var obj in compositeConditionObjects)
                    {
                        StringCondition compConditionString =
                            CreateStringCondition(obj, FindSimpleCondition(obj), true);
                        conditionString.Add(compConditionString);
                    }
                }
            }
            else return null;
        }

        if (condition && simpleEcaCondition.IsValid())
        {
            if (compositeConditions)
            {
                ecaRule = new EcaRule(whenEcaAction, finalEcaCondition, listOfActions);
            }
            else
            {
                ecaRule = new EcaRule(whenEcaAction, simpleEcaCondition, listOfActions);
            }
        }
        else
        {
            ecaRule = new EcaRule(whenEcaAction, listOfActions);
        }

        Debug.Log("Valid rule");


        ruleString = new RuleString(eventString, conditionString, actionString);
        

        // Save the string for the pop-ups
        UIManager.popUpEcaRule = ecaRule;
        UIManager.PopUpRuleString = ruleString;

        return ecaRule;
    }

    StringCondition CreateStringCondition(GameObject condition,
        SimpleEcaCondition simpleEcaCondition, bool composite)
    {
        ConditionDropdownHandler conditionDropdownHandler = condition.GetComponent<ConditionDropdownHandler>();
        StringCondition result = new StringCondition();
        if (!composite)
        {
            result.AndOr = "";
        }
        else
        {
            Dropdown andOr = conditionDropdownHandler.andOr;
            result.AndOr = andOr.options[andOr.value].text;
        }

        Dropdown dropdownToCheck = conditionDropdownHandler.toCheck;
        Dropdown dropdownCompareWidthDrop = conditionDropdownHandler.compareWithDrop;
        if (dropdownCompareWidthDrop && dropdownCompareWidthDrop.IsActive())
        {
            result.CompareWith = dropdownCompareWidthDrop.options[dropdownCompareWidthDrop.value].text;
        }
        else
        {
            InputField compareCompareWidthInput = conditionDropdownHandler.compareWithInputField;
            result.CompareWith = compareCompareWidthInput.text;
        }

        result.Property = simpleEcaCondition.GetProperty();
        result.CheckSymbol = simpleEcaCondition.GetSymbol();
        result.ToCheck = dropdownToCheck.options[dropdownToCheck.value].text;
        return result;
    }

    CompositeEcaCondition CreateCompositeConditions(SimpleEcaCondition firstEcaCondition)
    {
        var allCompositeConditions = GameObject.FindGameObjectsWithTag("CompositeCondition").ToList();
        var compositeConditions = from act in allCompositeConditions where act.name != "CompositeConditionPrefab" select act;

        ArrayList<SimpleEcaCondition> simpleConditions = new ArrayList<SimpleEcaCondition>();
        simpleConditions.Add(firstEcaCondition);
        ArrayList<CompositeEcaCondition.ConditionType> conditionTypes = new ArrayList<CompositeEcaCondition.ConditionType>();
        foreach (var obj in compositeConditions)
        {
            SimpleEcaCondition simpleEcaCondition = FindSimpleCondition(obj);
            if (simpleEcaCondition != null)
            {
                simpleConditions.Add(simpleEcaCondition);
                conditionTypes.Add(findOperator(obj));
            }
        }


        // To properly create a composite rule, we start from the last condition
        // We need to reverse the lists of simple conditions and boolean operators that link them
        // I.E.: A and B or C  => C or B and A
        CompositeEcaCondition result = new CompositeEcaCondition();
        simpleConditions.Reverse();
        conditionTypes.Reverse();

        // If there are 3 or more conditions
        if (simpleConditions.Count > 2)
        {
            // We start by creating a new composite condition based on the last two in the original configuration
            // I.E.: B or C
            result = new CompositeEcaCondition(conditionTypes[0], new List<EcaCondition>()
            {
                simpleConditions[1], simpleConditions[0]
            });
            // We then keep iterating on the remaining conditions, creating a new composite one 
            // connecting the next simple one (i.e.: A) with the previously created composite condition
            for (int i = 2; i < simpleConditions.Count; i++)
            {
                result = new CompositeEcaCondition(conditionTypes[i - 1],
                    new List<EcaCondition>()
                    {
                        simpleConditions[i], result
                    });
            }
        }

        // Otherwise if there are just 2 simple conditions we simply create a composite one
        // respecting the proper order
        if (simpleConditions.Count == 2)
        {
            result = new CompositeEcaCondition(conditionTypes[0], new List<EcaCondition>()
            {
                simpleConditions[1], simpleConditions[0]
            });
        }


        // Returns a single condition, containing the tree of simple conditions inside itself
        return result;
    }

    CompositeEcaCondition.ConditionType findOperator(GameObject compositeConditionObject)
    {
        ConditionDropdownHandler conditionDropdownHandler =
            compositeConditionObject.GetComponent<ConditionDropdownHandler>();
        Dropdown andOr = conditionDropdownHandler.andOr;
        string value = andOr.options[andOr.value].text;
        CompositeEcaCondition.ConditionType res;
        if (value.Equals("And"))
        {
            return CompositeEcaCondition.ConditionType.AND;
        }

        return CompositeEcaCondition.ConditionType.OR;
    }

    public SimpleEcaCondition FindSimpleCondition(GameObject row)
    {
        ConditionDropdownHandler conditionDropdownHandler = row.GetComponent<ConditionDropdownHandler>();
        GameObject toCheckSelected = conditionDropdownHandler.ToCheckSelected; //tocheck
        string propertySelected = conditionDropdownHandler.PropertySelected; //property
        string symbolSelected = conditionDropdownHandler.SelectedSymbol;

        //Check if data is complete
        if (toCheckSelected == null || string.IsNullOrEmpty(propertySelected) || string.IsNullOrEmpty(symbolSelected))
            return new SimpleEcaCondition(null, "", "", null);

        //compareWith as dropdown 
        GameObject gameObjectDropdown = conditionDropdownHandler.compareWithDrop.gameObject;
        if (gameObjectDropdown.activeInHierarchy)
        {
            Dropdown objDropdown = gameObjectDropdown.GetComponent<Dropdown>();
            string objValue = objDropdown.options[objDropdown.value].text;

            switch (propertySelected)
            {
                case "color":
                    Color color = DropdownHandler.colorDict[objValue];
                    return new SimpleEcaCondition(toCheckSelected, propertySelected, symbolSelected, color);
                case "position":
                    Vector3 vector3 = conditionDropdownHandler.raycastPointer.pos;
                    if (vector3 != Vector3.zero)
                        return new SimpleEcaCondition(toCheckSelected, propertySelected, symbolSelected,
                            new Position(vector3.x, vector3.y, vector3.z));
                    break;
                case "pov":
                    var pov = objValue == "First" ? ECACamera.POV.First : ECACamera.POV.Third;
                    return new SimpleEcaCondition(toCheckSelected, propertySelected, symbolSelected, pov);
                default:
                    ECABoolean boolean = objValue == "true" ? ECABoolean.TRUE : ECABoolean.FALSE;
                    return new SimpleEcaCondition(toCheckSelected, propertySelected, symbolSelected, boolean);
            }
        }
        //compareWith as inputField
        GameObject gameObjectInputField = conditionDropdownHandler.compareWithInputField.gameObject;
        if (gameObjectInputField.activeInHierarchy)
        {
            InputField inputField = gameObjectInputField.GetComponent<InputField>();
            if (inputField.text.Length > 0)
            {
                switch (conditionDropdownHandler.compareWithType)
                {
                    case ECARules4AllType.Text:
                        return new SimpleEcaCondition(toCheckSelected, propertySelected, symbolSelected, inputField.text);
                    case ECARules4AllType.Float:
                    case ECARules4AllType.Time:
                        return new SimpleEcaCondition(toCheckSelected, propertySelected, symbolSelected,
                            float.Parse(inputField.text));
                    case ECARules4AllType.Integer:
                        return new SimpleEcaCondition(toCheckSelected, propertySelected, symbolSelected,
                            Int32.Parse(inputField.text));
                    case ECARules4AllType.Rotation :
                        Rotation r = new Rotation();
                        string rotatesAround= conditionDropdownHandler.property.options[conditionDropdownHandler.property.value].text;
                        switch (rotatesAround)
                        {
                            case "rotation x":
                                r.x = float.Parse(inputField.text);
                                break;
                            case "rotation y":
                                r.y = float.Parse(inputField.text);
                                break;
                            case "rotation z":
                                r.z = float.Parse(inputField.text);
                                break;
                        }
                        return new SimpleEcaCondition(toCheckSelected, propertySelected, symbolSelected, r);
                }
                
            }
        }

        return new SimpleEcaCondition(null, "", "", null);
    }

    static EcaAction FindAction(GameObject row, ref StringAction stringAction)
    {
        DropdownHandler dropDownHaldler = row.GetComponent<DropdownHandler>();
        GameObject subjectSelected = dropDownHaldler.SubjectSelected; //subject
        string verbSelectedString = dropDownHaldler.VerbSelectedString; //verb

        //Check if data is complete
        if (subjectSelected == null || verbSelectedString == "") return new EcaAction();

        //string operation
        stringAction.Verb = verbSelectedString;
        Dropdown dropdownsubj = dropDownHaldler.subj.GetComponent<Dropdown>();
        stringAction.Subj = dropdownsubj.options[dropdownsubj.value].text;


        //Object:
        GameObject gameObjectDropdown = dropDownHaldler.objDrop.gameObject;
        GameObject gameObjectValueDropdown = dropDownHaldler.objValueDrop.gameObject;
        if (gameObjectDropdown.activeInHierarchy || gameObjectValueDropdown.activeInHierarchy)
        {
            Dropdown objDropdown;
            if (gameObjectDropdown.activeInHierarchy)
            {
                objDropdown = gameObjectDropdown.GetComponent<Dropdown>();
                if (dropDownHaldler.theGameObject.activeInHierarchy) stringAction.ObjThe = "The";
            }
            else objDropdown = gameObjectValueDropdown.GetComponent<Dropdown>();

            if (objDropdown.options.Count == 0)
                return new EcaAction();
            
            string objValue = objDropdown.options[objDropdown.value].text;

            if (objValue == "") return new EcaAction();
            
            stringAction.Obj = objValue;

            GameObject valueDropdown = dropDownHaldler.value.gameObject;

            if (dropDownHaldler.VerbSelectedType != null)
            {
                switch (dropDownHaldler.VerbSelectedType)
                {
                    case "YesNo":
                        ECABoolean booleanYesNo = objValue == "yes" ? ECABoolean.YES : ECABoolean.NO;
                        return new EcaAction(subjectSelected, verbSelectedString, booleanYesNo);
                    case "TrueFalse":
                        ECABoolean booleanTrueFalse = objValue == "true" ? ECABoolean.TRUE : ECABoolean.FALSE;
                        return new EcaAction(subjectSelected, verbSelectedString, booleanTrueFalse);
                    case "OnOff":
                        ECABoolean booleanOnOff = objValue == "on" ? ECABoolean.ON : ECABoolean.OFF;
                        return new EcaAction(subjectSelected, verbSelectedString, booleanOnOff);
                    case "Object":
                    case "ECAObject":
                    case "GameObject":
                        //The Object is a GameObject
                        if (dropDownHaldler.ObjectSelected != null)
                        {
                            return new EcaAction(subjectSelected, verbSelectedString,
                                dropDownHaldler.ObjectSelected);
                        }

                        break;
                    case "Position":
                        Vector3 vector3 = dropDownHaldler.raycastPointer.pos;
                        return new EcaAction(subjectSelected, verbSelectedString,
                            new Position(vector3.x, vector3.y, vector3.z));
                    default:
                        //e.g. character eats food
                        if (dropDownHaldler.ObjectSelected != null)
                        {
                            return new EcaAction(subjectSelected, verbSelectedString,
                                dropDownHaldler.ObjectSelected);
                        }

                        break;
                }
            }


            //if there is the object dropdown, there can be the value
            if (valueDropdown.activeInHierarchy)
            {
                Dropdown vDropdown = valueDropdown.GetComponent<Dropdown>();
                string vDropValue = vDropdown.options[vDropdown.value].text;

                stringAction.Value = vDropValue;

                string prop = dropDownHaldler.prepDrop.options[dropDownHaldler.prepDrop.value].text;
                stringAction.Prep = prop;
                
                switch (dropDownHaldler.ObjSelectedType)
                {
                    case "YesNo":
                        ECABoolean booleanYesNo = vDropValue == "yes" ? ECABoolean.YES : ECABoolean.NO;
                        return new EcaAction(subjectSelected, verbSelectedString, objValue, prop, booleanYesNo);
                    case "TrueFalse":
                        ECABoolean booleanTrueFalse = vDropValue == "true" ? ECABoolean.TRUE : ECABoolean.FALSE;
                        return new EcaAction(subjectSelected, verbSelectedString,objValue, prop, booleanTrueFalse);
                    case "OnOff":
                        ECABoolean booleanOnOff = vDropValue == "on" ? ECABoolean.ON : ECABoolean.OFF;
                        return new EcaAction(subjectSelected, verbSelectedString, objValue, prop,booleanOnOff);

                    case "ECAColor":
                        // TODO: Set color properly
                        // If my dictionary contains the value in vDropValue then it is a color
                        if (DropdownHandler.colorDictHex.ContainsKey(vDropValue))
                        {
                            // Get the proper RGBA value for the hex color
                            //Color colorValue = DropdownHandler.colorDict[vDropValue];
                            ECAColor ecaColor = new ECAColor(vDropValue);
                            // Create a new Action with the proper color
                            return new EcaAction(subjectSelected, verbSelectedString, objValue, prop, ecaColor);
                        }
                        return new EcaAction(subjectSelected, verbSelectedString, objValue, prop, vDropValue);
                    case "String":
                        //TODO ottimizzare
                        return new EcaAction(subjectSelected, verbSelectedString, objValue, prop, vDropValue);

                    case "POV":
                        var pov = vDropValue == "First" ? ECACamera.POV.First : ECACamera.POV.Third;
                        return new EcaAction(subjectSelected, verbSelectedString, objValue, prop, pov);
                }
                
            }

            //if the input field is active the object can be a single or a string
            //if both object value and input field are active e.g. increases intensity number
            
            //Obj dropdown + InputField
            GameObject inputField = dropDownHaldler.objField.gameObject;
            if (inputField.activeInHierarchy)
            {
                string inputText = inputField.GetComponent<InputField>().text;
                stringAction.Value = inputText;
                string prop="";
                if(dropDownHaldler.prepDrop.options.Count>0)
                    prop = dropDownHaldler.prepDrop.options[dropDownHaldler.prepDrop.value].text;;
                
                if (inputText.Length > 0)
                {
                    switch (@dropDownHaldler.ObjSelectedType)
                    {
                        case "String":
                            return new EcaAction(subjectSelected, verbSelectedString, objValue,prop, inputText);
                     
                        case "Int32":
                            return new EcaAction(subjectSelected, verbSelectedString, objValue, prop, Int32.Parse(inputText));
                     
                        case "Double":
                            return new EcaAction(subjectSelected, verbSelectedString, objValue, prop, Double.Parse(inputText));

                        case "Rotation":
                            float degreesValue = float.Parse(inputText);
                            Rotation rot = new Rotation();
                            string axis = objValue;
                            if (axis == "x") rot.x = degreesValue;
                            if (axis == "y") rot.y = degreesValue;
                            else if (axis == "z") rot.z = degreesValue;
                            return new EcaAction(subjectSelected, verbSelectedString, rot);
                        default:                    
                            return new EcaAction(subjectSelected, verbSelectedString, objValue, prop, float.Parse(inputText));

                    }
                }
            }
        }
        GameObject gameObjectInputField = dropDownHaldler.objField.gameObject;
        if (gameObjectInputField.activeInHierarchy)
        {
            InputField inputField = dropDownHaldler.objField;
            stringAction.Obj = inputField.text;
            if (inputField.text.Length > 0 && !string.IsNullOrEmpty(dropDownHaldler.VerbSelectedType))
            {
                switch (@dropDownHaldler.VerbSelectedType)
                {
                    case "String":
                        return new EcaAction(subjectSelected, verbSelectedString,  inputField.text);
                     
                    case "Int32":
                        return new EcaAction(subjectSelected, verbSelectedString, Int32.Parse(inputField.text));

                    case "Double":
                        return new EcaAction(subjectSelected, verbSelectedString, Double.Parse(inputField.text));

                    default:                    
                        return new EcaAction(subjectSelected, verbSelectedString,  float.Parse( inputField.text));

                }
            }
        }

        //in the elese case, the sentence is composed only of two words (e.g. vehicle starts)
        if (!gameObjectDropdown.activeInHierarchy && !gameObjectInputField.activeInHierarchy)
        {
            return new EcaAction(subjectSelected, verbSelectedString);
        }

        return new EcaAction();
    }


    bool compositeConditionExists()
    {
        var allCompositeConditionObjects = GameObject.FindGameObjectsWithTag("CompositeCondition").ToList();
        var compositeConditionObjects = from act in allCompositeConditionObjects where act.name != "CompositeConditionPrefab" select act;
        return compositeConditionObjects.Count() > 0;
    }
    

    public static void DiscardChanges()
    {
        //When:
        GameObject eventParentObj = GameObject.Find("Event");
        ClearEventAction(eventParentObj);

        //Action
        GameObject[] allActionParentObj = GameObject.FindGameObjectsWithTag("Action");
        var actionParentObj = from act in allActionParentObj where act.name != "ActionPrefab" select act;
        var actParObj = actionParentObj.ToArray();
        ClearEventAction(actParObj[0]); //leave only the first action
        for (int i=1;i< actParObj.Length;i++)
        {
            Destroy(actParObj[i]);
        }

        //Simple condition
        GameObject simpleCondParentObj = GameObject.Find("SimpleConditionPrefab(Clone)");
        if (simpleCondParentObj)
        {
            //Remove the color from the gameobject selected
            ConditionDropdownHandler conditionDropdownHandler =
                simpleCondParentObj.GetComponent<ConditionDropdownHandler>();
            if (conditionDropdownHandler.ToCheckSelected &&
                conditionDropdownHandler.ToCheckSelected.transform.GetComponent<ECAOutline>())
                Destroy(conditionDropdownHandler.ToCheckSelected.transform.GetComponent<ECAOutline>());
            Destroy(simpleCondParentObj); //destroying clones
        }


        //Composite condition
        GameObject[] allConditionsParentObj = GameObject.FindGameObjectsWithTag("CompositeCondition");
        var conditionsParentObj = from act in allConditionsParentObj where act.name != "CompositeConditionPrefab" select act;
        if (conditionsParentObj.Count() > 0)
        {
            foreach (var condition in conditionsParentObj)
            {
                //remove color
                ConditionDropdownHandler compositeDropdownHandler = condition.GetComponent<ConditionDropdownHandler>();
                if (compositeDropdownHandler.ToCheckSelected &&
                    compositeDropdownHandler.ToCheckSelected.transform.GetComponent<ECAOutline>())
                    Destroy(condition); //destroying clones
            }
        }

        if (GameObject.Find("ConditionList"))
            GameObject.Find("ConditionList").SetActive(false);
        if (GameObject.Find("_headerCondition"))
            GameObject.Find("_headerCondition").SetActive(false);
    }

    static void ClearEventAction(GameObject parentObj)
    {
        //dropdown
        DropdownHandler dropdownHandlerScript = parentObj.GetComponent<DropdownHandler>();
        Dropdown subj = dropdownHandlerScript.subj;
        Dropdown verb = dropdownHandlerScript.verb;
        Dropdown objDrop = dropdownHandlerScript.objDrop;
        Dropdown objValueDrop = dropdownHandlerScript.objValueDrop;
        Dropdown value = dropdownHandlerScript.value;
        GameObject theObj = dropdownHandlerScript.theGameObject;
        Dropdown prep = dropdownHandlerScript.prepDrop;

        InputField objField = dropdownHandlerScript.objField;


        subj.Select();
        subj.value = 0;
        subj.gameObject.GetComponent<Image>().color = UIColors.dropDownColor;

        verb.ClearOptions();
        verb.gameObject.SetActive(false);

        objDrop.ClearOptions();
        objDrop.gameObject.GetComponent<Image>().color = UIColors.dropDownColor;
        objDrop.Select();
        objDrop.value = 0;
        objDrop.gameObject.SetActive(false);
        
        objValueDrop.ClearOptions();
        objValueDrop.Select();
        objValueDrop.value = 0;
        objValueDrop.gameObject.SetActive(false);
        
        prep.ClearOptions();
        prep.Select();
        prep.value = 0;
        prep.gameObject.SetActive(false);

        value.ClearOptions();
        value.gameObject.SetActive(false);

        objField.Select();
        objField.text = "";
        objField.gameObject.SetActive(false);
        
        theObj.SetActive(false);

        //references
        dropdownHandlerScript.VerbsItem.Clear();
        dropdownHandlerScript.VerbsString.Clear();
        dropdownHandlerScript.StateVariables.Clear();
        dropdownHandlerScript.Subjects.Clear();

        //Subject selected color
        GameObject subjectSelected = dropdownHandlerScript.SubjectSelected;
        if (subjectSelected && subjectSelected.transform.GetComponent<ECAOutline>())
            Destroy(subjectSelected.transform.GetComponent<ECAOutline>());

        //Object selected color
        GameObject objectSelected = dropdownHandlerScript.ObjectSelected;
        if (objectSelected && objectSelected.transform.GetComponent<ECAOutline>())
            Destroy(objectSelected.transform.GetComponent<ECAOutline>());

        dropdownHandlerScript.SubjectSelected = null;
        dropdownHandlerScript.ObjectSelected = null;
    }

    public void RemoveAction()
    {
        Debug.Log(this);
    }

    public string FormatRule()
    {
        string sRule = "";

        StringAction newStringAction = new StringAction();
        //When action
        EcaAction whenEcaAction = FindAction(GameObject.Find("Event"), ref newStringAction);
        GameObject whenEventObj = GameObject.Find("Event");

        sRule += ParseActionEvent(whenEventObj, whenEcaAction, "When ");

        //First then action
        EcaAction thenEcaAction = FindAction(GameObject.Find("Action"), ref newStringAction);
        GameObject thenEventObj = GameObject.Find("Action");

        //conditions
        bool condition = false;
        bool compositeConditions = false;

        //check if condition exists
        GameObject simpleC = GameObject.Find("SimpleConditionPrefab(Clone)");

        //I need to initialize the variables with standard values
        SimpleEcaCondition simpleEcaCondition = new SimpleEcaCondition(simpleC, "", "", "");
        CompositeEcaCondition finalEcaCondition = new CompositeEcaCondition();

        if (simpleC != null)
        {
            condition = true;
            simpleEcaCondition = FindSimpleCondition(simpleC);

            if (simpleEcaCondition.IsValid())
            {
                sRule += ParseSimpleCondition(simpleC, simpleEcaCondition, "CompareWithDrop", "If ");

                //composite conditions
                compositeConditions = compositeConditionExists();
                if (compositeConditions)
                {
                    GameObject[] allCompositeConditionObjects = GameObject.FindGameObjectsWithTag("CompositeCondition");
                    var compositeConditionObjects = from act in allCompositeConditionObjects where act.name != "CompositeConditionPrefab" select act;
                    sRule += ParseCompositeCondition(compositeConditionObjects.ToArray());
                }
            }
        }


        // Add the then action(s) to the parsed string
        GameObject[] allActlist = GameObject.FindGameObjectsWithTag("Action");
        var alist = from act in allActlist where act.name != "ActionPrefab" select act;
        sRule += "Then" + ParseMultipleActions(alist.ToArray());

        // Remove unity objects' related parts of the string
        sRule = sRule.Replace("(UnityEngine.GameObject)", "");

        Debug.Log("sRule:\n" + sRule);
        return sRule;
    }


    public string ParseActionEvent(GameObject obj, EcaRules.EcaAction ecaActionEvent, string header)
    {
        string parsed = "";

        Dropdown dropdown = obj.transform.Find("ObjectDrop").GetComponent<Dropdown>();

        if (dropdown.IsActive())
        {
            string property = dropdown.options[dropdown.value].text;

            if (property == "color")
            {
                Dropdown whenColor = obj.transform.Find("ValueDrop").GetComponent<Dropdown>();
                string color = whenColor.options[whenColor.value].text;

                string[] separator = {"#"};
                string[] removeRGBA = ecaActionEvent.ToString().Split(separator, StringSplitOptions.None);

                parsed += header + removeRGBA[0] + color + "\n";
            }
            else
            {
                parsed += header + ecaActionEvent + "\n";
            }
        }
        else
        {
            parsed += header + ecaActionEvent + "\n";
        }


        return parsed;
    }


    public string ParseSimpleCondition(GameObject obj, SimpleEcaCondition sc, string transformProperty, string header)
    {
        string parsed = "";

        Dropdown property = obj.transform.Find(transformProperty).GetComponent<Dropdown>();
        if (property.IsActive())
        {
            if (sc.GetProperty() == "color")
            {
                string color = property.options[property.value].text;

                string[] separator = {"#"};
                string[] removeRGBA = sc.ToString().Split(separator, StringSplitOptions.None);

                parsed += header + removeRGBA[0] + color + "\n";
            }
            else
            {
                parsed += header + sc + "\n";
            }
        }
        else
        {
            parsed += header + sc + "\n";
        }

        return parsed;
    }

    public string ParseCompositeCondition(GameObject[] compositeConditions)
    {
        string parsed = "";

        foreach (var cc in compositeConditions)
        {
            SimpleEcaCondition sc = FindSimpleCondition(cc);

            Dropdown andOr = cc.transform.Find("AndOr").GetComponent<Dropdown>();
            string value = andOr.options[andOr.value].text;

            parsed += ParseSimpleCondition(cc, sc, "CompareWithDrop", value + " ");
        }

        return parsed;
    }

    public string ParseMultipleActions(GameObject[] thenActions)
    {
        string parsed = "";
        StringAction newStringAction = new StringAction();

        foreach (var a in thenActions)
        {
            EcaAction ecaAction = FindAction(a, ref newStringAction);
            parsed += ParseActionEvent(a, ecaAction, " ");
        }

        return parsed;
    }

    public static GameObject CreateRuleRow(GameObject existingRule, EcaRule ecaRule)
    {
        // Step 1 controllare che existingRule non sia null
        // Step 2 Se non è null, la stiamo modificando
        // Step 3 modificare il testo della regola attingendo alla struttura [il testo lo pesco dalla struttura usando existingRule]
        // Step 4 Se è null rifai quello che faceva inizialmente
        // Step 5 il testo viene sempre preso dalla struttura
        GameObject newRulePrefab;

        // Complete formatted rule
        TextRuleSerializer textRuleSerializer = new TextRuleSerializer();
        StringWriter stringWriter = new StringWriter();
        textRuleSerializer.PrintRule(ecaRule, stringWriter);
        string label = stringWriter.ToString();

        if (existingRule)
        {
            // We're modifying the rule here
            GameObject ruleString2 = existingRule.transform.GetChild(0).gameObject;
            Text text2 = ruleString2.GetComponent<Text>();
            text2.text = label;
            return null;
        }

        // We're creating a new rule
        // Find the rulelist and the viewport containing the rules
        
        GameObject ruleList = GameObject.Find("RuleList");
        // GetChild 1 -> RuleScroll
        // Child 0 -> Viewport
        // Child 0 -> Content
        GameObject viewport = ruleList.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
        // Instantiate the prefab inside the viewport
        newRulePrefab = Instantiate(Resources.Load("Rule"), viewport.transform) as GameObject;
        // Fix the new rule's transform
        // newRule.transform.rotation.Set(0, 0, 0, 0);
        newRulePrefab.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, 0f);
        newRulePrefab.transform.localScale = Vector3.one;


        // Access the label inside its child
        GameObject ruleString = newRulePrefab.transform.GetChild(0).gameObject;
        Text text = ruleString.GetComponent<Text>();
        text.text = label;


        return newRulePrefab;
    }
}