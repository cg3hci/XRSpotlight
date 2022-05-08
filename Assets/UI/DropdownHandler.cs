using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Antlr4.Runtime.Misc;
using EcaRules;
using ECAScripts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Examples.UIRule.Prefabs;
using Object = System.Object;

public class VerbComposition
{
    private string verb;
    private ActionAttribute _actionAttribute;

    public VerbComposition(string verb, ActionAttribute actionAttribute)
    {
        Verb = verb;
        ActionAttribute = actionAttribute;
    }

    public string Verb
    {
        get => verb;
        set => verb = value;
    }

    public ActionAttribute ActionAttribute
    {
        get => _actionAttribute;
        set => _actionAttribute = value;
    }

    public static bool operator ==(VerbComposition obj1, VerbComposition obj2)
    {
        return obj1.ActionEquals(obj2);
    }

    public static bool operator !=(VerbComposition obj1, VerbComposition obj2)
    {
        return !obj1.ActionEquals(obj2);
    }

    public bool ActionEquals(VerbComposition action)
    {
        if (!Verb.Equals(action.Verb))
            return false;
        if (!(ActionAttribute.ObjectType == action.ActionAttribute.ObjectType))
            return false;
        if (!(ActionAttribute.SubjectType == action.ActionAttribute.SubjectType))
            return false;
        if (!(ActionAttribute.Verb.Equals(action.ActionAttribute.Verb)))
            return false;
        return true;
    }
}

public class DropdownHandler : MonoBehaviour
{
    //dropdown references
    public Dropdown subj, verb, objDrop, objValueDrop, value, prepDrop;
    public GameObject theGameObject;

    //Inputfield references
    public InputField objField;

    //Dictionary foreach subject with the reference of the gameobject
    private Dictionary<int, Dictionary<GameObject, string>> subjects =
        new Dictionary<int, Dictionary<GameObject, string>>();

    //Dictionary foreach verb with the index and the verb and the type of object (Position, Rotation, ...)
    private Dictionary<int, VerbComposition> verbsItem = new Dictionary<int, VerbComposition>();

    public Dictionary<int, Dictionary<GameObject, string>> Subjects
    {
        get => subjects;
        set => subjects = value;
    }

    public Dictionary<int, VerbComposition> VerbsItem
    {
        get => verbsItem;
        set => verbsItem = value;
    }

    public Dictionary<string, List<ActionAttribute>> VerbsString
    {
        get => verbsString;
        set => verbsString = value;
    }

    public Dictionary<string, ( ECARules4AllType, Type)> StateVariables
    {
        get => stateVariables;
        set => stateVariables = value;
    }

    public static Dictionary<string, List<ActionAttribute>> verbsString =
        new Dictionary<string, List<ActionAttribute>>();

    //Dictionary for the subject selected with all its state variables and the type
    private Dictionary<string, ( ECARules4AllType, Type)> stateVariables = new Dictionary<string, ( ECARules4AllType, Type)>();

    public static Dictionary<string, Color> colorDict = new Dictionary<string, Color>()
    {
        {"blue", Color.blue}, // 0xff1f77b4,
        {"green", Color.green}, // 0xffd62728
        {"red", Color.red}, // 0xff9467bd
        {"purple", Color.magenta}, // 0xff9467bd
        {"gray", Color.gray}, // 0xff7f7f7f
        {"grey", Color.grey}, // 0xff7f7f7f
        {"yellow", Color.yellow}, // 0xffbcbd22
        {"cyan", Color.cyan}, // 0xff17becf
        {"white", Color.white}, // 0xffffffff
    };

    public static Dictionary<string, string> colorDictHex = new Dictionary<string, string>()
    {
        {"blue", "#1f77b4ff"},
        {"orange", "#ff7f0eff"},
        {"green", "#d62728ff"},
        {"red", "#9467bdff"},
        {"purple", "#9467bdff"},
        {"brown", "#8c564bff"},
        {"pink", "#e377c2ff"},
        {"gray", "#7f7f7fff"},
        {"grey", "#7f7f7fff"},
        {"yellow", "#bcbd22ff"},
        {"cyan", "#bcbd22ff"},
        {"white", "#ffffffff"},
    };

    public static Dictionary<Color, string> reversedColorDict = new Dictionary<Color, string>()
    {
       // { UIColors.blue, "blue" }, // 0xff1f77b4,
        {UIColors.orange, "orange"}, // 0xffff7f0e
        {UIColors.green, "green"}, // 0xffd62728
        {UIColors.red, "red"}, // 0xff9467bd
        {UIColors.purple, "purple"}, // 0xff9467bd
        {UIColors.brown, "brown"}, // 0xff8c564b
        {UIColors.pink, "pink"}, // 0xffe377c2
        {UIColors.gray, "gray"}, // 0xff7f7f7f
        {UIColors.grey, "grey"}, // 0xff7f7f7f
        {UIColors.yellow, "yellow"}, // 0xffbcbd22
        {UIColors.cyan, "cyan"}, // 0xff17becf
        {UIColors.white, "white"}, // 0xffffffff
    };

    private EcaAction ecaAction; //serve?

    //Selected
    private GameObject subjectSelected; //gameobject with the subject

    private GameObject previousSelectedSubject, previousSelectedObject=null;
    // public GameObject SubjectSelected => subjectSelected;
    private string subjectSelectedType; //e.g. ECALight, Character....

    private string verbSelectedString; //string with the verb
    public string VerbSelectedType { get; set; }
    public string ObjSelectedType { get; set; }

    public string VerbSelectedString
    {
        get => verbSelectedString;
        set => verbSelectedString = value;
    }

    private GameObject objectSelected;

    public GameObject SubjectSelected
    {
        get => subjectSelected;
        set => subjectSelected = value;
    }

    public GameObject ObjectSelected
    {
        get => objectSelected;
        set => objectSelected = value;
    }

    // public GameObject ObjectSelected => objectSelected;

    public XRRaycastPointer raycastPointer;
    private GameObject xrRaycastPointerContainer;
    
    //variables for the rows in the prefab
    public GameObject SubjectRow, VerbRow, ObjectRow;

    // Start is called before the first frame update
    void Awake()
    {

        //Listener
        verb.onValueChanged.AddListener(delegate { DropdownValueChangedVerb(verb); });
        subj.onValueChanged.AddListener(delegate { DropdownValueChangedSubject(subj); });
        //object with the value e.g. changes "active" ...
        objValueDrop.onValueChanged.AddListener(delegate { DropdownValueChangedObjectValue(objValueDrop); });
        //object without the value e.g. looks at gameobject
        objDrop.onValueChanged.AddListener(delegate { DropdownValueChangedObject(objDrop); });


        PopulateSubjDropdown();

        //initially verb, obj and value are disabled
        verb.gameObject.SetActive(false);
        objDrop.gameObject.SetActive(false);
        objValueDrop.gameObject.SetActive(false);
        value.gameObject.SetActive(false);
        objField.gameObject.SetActive(false);
        theGameObject.SetActive(false);
        prepDrop.gameObject.SetActive(false);

        //Raycast info for position and object selected
        xrRaycastPointerContainer = GameObject.Find("Player");
        raycastPointer = xrRaycastPointerContainer.GetComponent<XRRaycastPointer>();
    }

    /**
     * Listener for verb dropdown, when called we need to provide the correct set of objects
     */
    void DropdownValueChangedVerb(Dropdown change)
    {
        //retrieve selected string and gameobject
        verbSelectedString = change.options[change.value].text;
        int verbSelectedIndex = change.value;

        DisableNextComponent("verb");

        //now, I need to know if the object would be a GameObject or a value 
        List<ActionAttribute> actionAttributes = verbsString[verbSelectedString];
        
        // Used to sort each dropdown's options
        List<string> entries = new List<string>();
        
        //we need to activate the object dropdown
        if (actionAttributes.Count == 1 || RuleUtils.SameAttributesList(actionAttributes))
        {
            ActionAttribute ac = actionAttributes[0];
            if (ac.ObjectType != null)
            {
                Debug.Log(ac.ObjectType.Name);
                VerbSelectedType = ac.ObjectType.Name;
                
                switch (ac.ObjectType.Name)
                {
                    case "Object":
                    case "ECAObject":
                    case "GameObject":
                        theGameObject.SetActive(true);
                        objDrop.gameObject.SetActive(true);
                        objDrop.ClearOptions();
                        objDrop.options.Add(new Dropdown.OptionData(""));
                        objDrop.options.Add(new Dropdown.OptionData("Last selected object"));
                        
                        for (int i = 0; i < subjects.Count; i++)
                        {
                            foreach (KeyValuePair<GameObject, string> entry in subjects[i])
                            {
                                //TODO handle alias
                                if (entry.Key != subjectSelected)
                                {
                                    //type needs a refactor: it can't be a behaviour and if contains "ECA" should be parsed
                                    string type = RuleUtils.FindInnerTypeNotBehaviour(entry.Key);
                                    type = RuleUtils.RemoveECAFromString(type);
                                    // objDrop.options.Add(new Dropdown.OptionData(type + " " + entry.Key.name));
                                    entries.Add(type + " " + entry.Key.name);
                                }
                            }
                        }

                        break;
                    case "YesNo":
                        objDrop.gameObject.SetActive(true);
                        objDrop.ClearOptions();
                        objDrop.options.Add(new Dropdown.OptionData("")); 
                        entries.Add("yes"); 
                        entries.Add("no");
                        break;
                    case "TrueFalse":
                        objDrop.gameObject.SetActive(true);
                        objDrop.ClearOptions();
                        objDrop.options.Add(new Dropdown.OptionData(""));
                        entries.Add("true");
                        entries.Add("false");
                        break;
                    case "OnOff":
                        objDrop.gameObject.SetActive(true);
                        objDrop.ClearOptions();
                        objDrop.options.Add(new Dropdown.OptionData(""));
                        entries.Add("on");
                        entries.Add("off");
                        break;
                    case "Position":
                        objDrop.gameObject.SetActive(true);
                        objDrop.ClearOptions();
                        objDrop.options.Add(new Dropdown.OptionData(""));
                        Vector3 selectedPos = raycastPointer.pos;
                        if (selectedPos != Vector3.zero)
                        {
                            //objDrop.options.Add(new Dropdown.OptionData(raycastPointer.TextPosition.text));
                            objDrop.options.Add(new Dropdown.OptionData("Last selected position"));
                        }

                        break;
                    case "Single": //Float
                        ActivateInputField("decimal");
                        break;
                    case "String":
                        ActivateInputField("string");
                        break;
                    case "Rotation":
                        objValueDrop.ClearOptions();
                        objValueDrop.gameObject.SetActive(true);
                        objValueDrop.options.Add(new Dropdown.OptionData(""));
                        objValueDrop.options.Add(new Dropdown.OptionData("x"));
                        objValueDrop.options.Add(new Dropdown.OptionData("y"));
                        objValueDrop.options.Add(new Dropdown.OptionData("z"));
                        objValueDrop.RefreshShownValue();
                        break;
                    //TODO path
                    case "Path":
                        break;
                    case "Int32":
                        ActivateInputField("integer");
                        break;
                    default:
                        //it can be a typeof(EcaComponent), but first we need to retrieve the component
                        theGameObject.SetActive(true);
                        string comp = ac.ObjectType.Name;
                        Component c = subjectSelected.GetComponent(comp);
                        objDrop.gameObject.SetActive(true);
                        objDrop.ClearOptions();
                        objDrop.options.Add(new Dropdown.OptionData(""));
                        objDrop.options.Add(new Dropdown.OptionData("Last selected object"));
                        //it's possible that the verb is passive (e.g. character eats food),
                        //in this case we don't find it in the selected subject, but in one of the subjects
                        if (c == null)
                        {
                            if (subjects.Count == 0) subjects= RuleUtils.FindSubjects();
                            RuleUtils.AddObjectPassiveVerbs(subjects, comp, objDrop);

                        }
                        else //the verb is not passive, the object component can be found in all ecaobject in the scene
                        {
                            RuleUtils.AddObjectActiveVerbs(subjects, comp, objDrop, subjectSelected);
                        }
                        
                        break;
                }
                
                RuleUtils.AddToDropdownInAlphabeticalOrder(objDrop, entries);
            }
            //value e.g. increases intensity
            else if (ac.ValueType != null) 
            {
                objValueDrop.ClearOptions();
                objValueDrop.gameObject.SetActive(true);
                objValueDrop.options.Add(new Dropdown.OptionData(""));
                objValueDrop.options.Add(new Dropdown.OptionData(ac.variableName));
                objValueDrop.RefreshShownValue();
            }
        }
        //in the else case, the sentence is composed only of two words (e.g. vehicle starts)
        //we don't need to activate anything

        //if actionAttributes.Count is >1 means that there are verbs like changes, that has
        //more attributes (active, visibility...)
        //we activate the object value drop
        else
        {
            VerbSelectedType = null;
            objValueDrop.ClearOptions();
            objValueDrop.options.Add(new Dropdown.OptionData(""));
            foreach (var ac in actionAttributes)
            {
                if (ac.ValueType != null)
                {
                    objValueDrop.gameObject.SetActive(true);
                    // objValueDrop.options.Add(new Dropdown.OptionData(ac.variableName));
                    entries.Add(ac.variableName);
                    objValueDrop.RefreshShownValue();
                }
            }
            RuleUtils.AddToDropdownInAlphabeticalOrder(objValueDrop, entries);
        }
    }

    void DropdownValueChangedObject(Dropdown change)
    {
        DisableNextComponent("object");
        //retrieve selected string and gameobject
        var objSelectedString = change.options[change.value].text;

        //select object by raycast
        if (objSelectedString.Equals("Last selected object"))
        {
            FindSelectedObjectByRaycast(change); return;
        }

        string selectedCutString = Regex.Match(objSelectedString, "[^ ]* (.*)").Groups[1].Value;
        //The object selected is a GameObject
        if (GameObject.Find(selectedCutString) != null)
        {
            previousSelectedObject = objectSelected;
            objectSelected = GameObject.Find(selectedCutString);
            RuleUtils.ChangeColorGameObjectDropdown(objectSelected, objDrop.gameObject.transform, previousSelectedObject);
        }
        else objectSelected = null;

    }
    
    void DropdownValueChangedObjectValue(Dropdown change)
    {
        DisableNextComponent("object");
        //retrieve selected string and gameobject
        var objSelectedString = change.options[change.value].text;
        objectSelected = null;
        
        prepDrop.gameObject.SetActive(true);

        //retrieve action attributes
        verbsString = RuleUtils.PopulateVerbsString(verbsItem);
        List<ActionAttribute> actionAttributes = verbsString[verbSelectedString];
        value.ClearOptions();
        stateVariables = RuleUtils.FindStateVariables(subjectSelected);
        foreach (var ac in actionAttributes)
        {
            // Used to sort each dropdown's options
            List<string> entries = new List<string>();
            
            if (ac.ObjectType==typeof(Rotation))
            {
                ActivateInputField("decimal");
                prepDrop.gameObject.SetActive(false);
                ObjSelectedType = "Rotation";
                return;
            }
            
            if (ac.variableName == objSelectedString)
            {
                prepDrop.options.Add(new Dropdown.OptionData(ac.ModifierString)); 
                prepDrop.RefreshShownValue();
                ObjSelectedType = ac.ValueType.Name;
                
                switch (ac.ValueType.Name)
                {
                    case "YesNo":
                        value.gameObject.SetActive(true);
                        value.options.Add(new Dropdown.OptionData(""));
                        entries.Add("yes"); 
                        entries.Add("no");
                        break;
                    case "TrueFalse":
                        value.gameObject.SetActive(true);
                        value.options.Add(new Dropdown.OptionData(""));
                        entries.Add("true");
                        entries.Add("false");
                        break;
                    case "OnOff":
                        value.gameObject.SetActive(true);
                        value.options.Add(new Dropdown.OptionData(""));
                        entries.Add("on");
                        entries.Add("off");
                        break;
                    case "String":
                        if (objSelectedString == "mesh")
                        {
                            value.gameObject.SetActive(true);
                            value.options.Add(new Dropdown.OptionData(""));
                            foreach (var mesh in UIManager.items) 
                                entries.Add(mesh);
                        }
                        else ActivateInputField("alphanumeric");
                        break;

                    case "ECAColor":
                        value.gameObject.SetActive(true);
                        value.options.Add(new Dropdown.OptionData(""));
                        // Add colors to dropdown
                        foreach (KeyValuePair<string, Color> kvp in colorDict)
                            entries.Add(kvp.Key);
                        break;

                    case "Single":
                    case "Double":
                        ActivateInputField("decimal");
                        break;
                    
                    case "Int32":
                        ActivateInputField("Integer");
                        break;
                    //TODO optimize
                    case "POV":
                        value.gameObject.SetActive(true);
                        value.options.Add(new Dropdown.OptionData(""));
                        entries.Add("First");
                        entries.Add("Third");
                        break;
                }
                RuleUtils.AddToDropdownInAlphabeticalOrder(value, entries);
                return;
            }
        }

    }

    /**
     * Listener for subject dropdown, when called we need to provide the correct set of verbs
     */
    void DropdownValueChangedSubject(Dropdown change)
    {
        DisableNextComponent("subject");
        verb.gameObject.SetActive(true);
        verb.ClearOptions();

        //When we go back from the rule list we assign the value to 0
        if (change.value == 0) return;
        
        //retrieve selected string and gameobject
        string selectedSubjectString = change.options[change.value].text;
        if (selectedSubjectString.Equals("Last selected object"))
        {
            FindSelectedObjectByRaycast(change); return;
        }

        //I need to cut the string because in the dropdown we use "Type Name", the dictionary only contains the type
        string selectedCutString = Regex.Match(selectedSubjectString, "[^ ]* (.*)").Groups[1].Value;
        previousSelectedSubject = subjectSelected;

        subjectSelected = GameObject.Find(selectedCutString).gameObject;

        //we have to find it from the dictionary, because some types are trimmed (see ECALight -> Light)
        foreach (var item in subjects)
        {
            foreach (var keyValuePair in item.Value)
            {
                if (keyValuePair.Key == subjectSelected)
                {
                    subjectSelectedType = keyValuePair.Value;
                }
            }
        }
        

        verbsItem = RuleUtils.FindActiveVerbs(subjectSelected, subjects, subjectSelectedType, true);
        RuleUtils.FindPassiveVerbs(subjectSelected, subjects, subjectSelectedType, ref verbsItem);

        //change color
        RuleUtils.ChangeColorGameObjectDropdown(subjectSelected, subj.gameObject.transform, previousSelectedSubject);


        verbsString.Clear();

        verbsString = RuleUtils.PopulateVerbsString(verbsItem);

        // Add options to verb dropdown
        List<string> entries = new List<string>();
        verb.options.Add(new Dropdown.OptionData(""));
        foreach (var s in verbsString)
        {
            //TODO alias
            // verb.options.Add(new Dropdown.OptionData(s.Key));
            entries.Add(s.Key);
        }
        RuleUtils.AddToDropdownInAlphabeticalOrder(verb, entries);
    }
    
    void PopulateSubjDropdown()
    {
        subj.ClearOptions();
        subjects = RuleUtils.FindSubjects();
        subj.options.Add(new Dropdown.OptionData(""));
        subj.options.Add(new Dropdown.OptionData("Last selected object"));
        

        List<string> entries = new List<string>();

        for (int i = 0; i < subjects.Count; i++)
        {
            foreach (KeyValuePair<GameObject, string> entry in subjects[i])
            {
                //TODO alias
                //type needs a refactor: it can't be a behaviour and if contains "ECA" should be parsed
                string type = RuleUtils.FindInnerTypeNotBehaviour(entry.Key);
                type = RuleUtils.RemoveECAFromString(type);
                entries.Add(type + " " + entry.Key.name);
            }
        }
        RuleUtils.AddToDropdownInAlphabeticalOrder(subj, entries);
        subj.RefreshShownValue();
    }

    void DisableNextComponent(string changedField)
    {
        switch (changedField)
        {
            // Change subject
            case "subject":
                verb.gameObject.SetActive(false);
                theGameObject.SetActive(false);
                objDrop.gameObject.SetActive(false);
                value.gameObject.SetActive(false);
                objField.gameObject.SetActive(false);
                objValueDrop.gameObject.SetActive(false);
                prepDrop.gameObject.SetActive(false);
                break;
            // Change verb
            case "verb":
                theGameObject.SetActive(false);
                objDrop.gameObject.SetActive(false);
                objValueDrop.gameObject.SetActive(false);
                value.gameObject.SetActive(false);
                objField.gameObject.SetActive(false);
                prepDrop.gameObject.SetActive(false);
                break;
            // Change object
            case "object":
                value.gameObject.SetActive(false);
                value.ClearOptions();
                break;
        }
    }

    void ActivateInputField(string validationType)
    {
        //activate
        objField.gameObject.SetActive(true);
        //clear input field
        objField.Select();
        objField.text = "";
        //change validation type
        objField.characterValidation = validationType == "decimal"
            ? InputField.CharacterValidation.Decimal
            : (validationType == "integer" ? InputField.CharacterValidation.Integer : 
                InputField.CharacterValidation.Alphanumeric );
    }

    void FindSelectedObjectByRaycast(Dropdown change)
    {
        GameObject selectedGameObjectInRaycast = raycastPointer.LastSelectedObject;

        if (selectedGameObjectInRaycast != null)
        {
            for (int i = 0; i <change.options.Count; i++)
            {
                if (selectedGameObjectInRaycast.name == change.options[i].text.Split(' ').Last())
                {
                    change.value = i;
                    return;
                }
            }
            //return change.options[change.value].text;
        }

        Debug.Log("Object not found");
    }
    
    
}