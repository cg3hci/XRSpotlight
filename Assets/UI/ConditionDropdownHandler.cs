using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime.Misc;
using ECARules4All.RuleEngine;
using ECAScripts;
using UnityEngine;
using UnityEngine.UI;

public class ConditionDropdownHandler : MonoBehaviour
{
    
    //dropdown references
    public Dropdown toCheck, property, checkSymbol, compareWithDrop, andOr;
    
    //Input Field reference
    public InputField compareWithInputField;
    
    //Dictionary foreach subject with the reference of the gameobject
    private Dictionary<int, Dictionary<GameObject, string>> toCheckDictionary = new Dictionary<int, Dictionary<GameObject, string>>();
    
    //Dictionary for the subject selected with all its state variables and the type
    private Dictionary<string, ( ECARules4AllType, Type)> stateVariables = new Dictionary<string, ( ECARules4AllType, Type)>();
    
    //Selected toCheck
    private GameObject toCheckSelected, previousToCheckSelected; //gameobject with the subject
    public GameObject ToCheckSelected => toCheckSelected;
    private string toCheckSelectedType;
    
    //Selected property
    private string propertySelected;
    public string PropertySelected => propertySelected;
    
    //selected symbol
    private string selectedSymbol;
    public string SelectedSymbol => selectedSymbol;

    public static List<string> booleanSymbols = new List<string>() {"is", "is not"};
    public static List<string> matemathicalSymbols = new List<string>() {"=", "!=", ">", "<", "<=", ">="};

    public ECARules4AllType compareWithType;
    
    public XRRaycastPointer raycastPointer;
    private GameObject xrRaycastPointerContainer;

    private void Awake()
    {
        //at start we must populate the first dropdown
        PopulateToCheckDropdown();
        
        //initially property, checkSimbol and compareWidth are disabled
        property.gameObject.SetActive(false); 
        checkSymbol.gameObject.SetActive(false); 
        compareWithDrop.gameObject.SetActive(false); 
        compareWithInputField.gameObject.SetActive(false); 
        

        //Listener
        toCheck.onValueChanged.AddListener(delegate { DropdownValueChangedToCheck(toCheck); });
        property.onValueChanged.AddListener(delegate { DropdownValueChangedProperty(property); }); 
        checkSymbol.onValueChanged.AddListener(delegate { DropdownValueChangedCheckSymbol(checkSymbol); });
        
        //Raycast info for position and object selected
        xrRaycastPointerContainer = GameObject.Find("Player");
        raycastPointer = xrRaycastPointerContainer.GetComponent<XRRaycastPointer>();
        
    }


    void DropdownValueChangedToCheck(Dropdown change)
    {
        //if previous activated, hide next elements
        DisableNextComponent("toCheck");
        
        //activate next element
        property.gameObject.SetActive(true);
        property.ClearOptions();
        
        //retrieve selected string and gameobject
        string selectedSubjectString = change.options[change.value].text;

        if (selectedSubjectString.Equals("Last selected object"))
        {
            FindSelectedObjectByRaycast(change); return;
        }
        //I need to cut the string because in the dropdown we use "Type Name", the dictionary only contains the type
        string selectedCutString = Regex.Match(selectedSubjectString, "[^ ]* (.*)").Groups[1].Value;
        toCheckSelectedType = Regex.Match(selectedSubjectString, "^[^ ]+").Value;
        previousToCheckSelected = toCheckSelected;
        toCheckSelected = GameObject.Find(selectedCutString).gameObject;
        
        //change color
        RuleUtils.ChangeColorGameObjectDropdown(toCheckSelected, toCheck.gameObject.transform, previousToCheckSelected);

        stateVariables = RuleUtils.FindStateVariables(toCheckSelected);
        
        // Used to sort each dropdown's options
        List<string> entries = new List<string>();
        property.options.Add(new Dropdown.OptionData(""));
        foreach (var var in stateVariables)
        {
            if (var.Key == "rotation")
            {
                entries.Add("rotation x");
                entries.Add("rotation y");
                entries.Add("rotation z");
            }
            else entries.Add(var.Key);
        }
        RuleUtils.AddToDropdownInAlphabeticalOrder(property, entries);
    }
    
    void DropdownValueChangedProperty(Dropdown change)
    {
        //if previous activated, hide next elements
        DisableNextComponent("property");
        
        //activate next element
        checkSymbol.gameObject.SetActive(true);
        checkSymbol.ClearOptions();
        
        //retrieve selected string and type
        propertySelected = change.options[change.value].text;
        if (propertySelected.StartsWith("rotation ")) propertySelected = "rotation";
        
        //thanks to the dictionary, we can retrieve the type:
        ECARules4AllType type = stateVariables[propertySelected].Item1;

        
        // Used to sort each dropdown's options
        List<string> entries = new List<string>();
        checkSymbol.options.Add(new Dropdown.OptionData(""));
        switch (type)
        {
            case ECARules4AllType.Float:
            case ECARules4AllType.Integer:
                foreach (var symbol in matemathicalSymbols)
                {
                    entries.Add(symbol);
                }
                break;
            
            case ECARules4AllType.Boolean:
            case ECARules4AllType.Position:
            case ECARules4AllType.Rotation:
            case ECARules4AllType.Path:
            case ECARules4AllType.Color:
            case ECARules4AllType.Text:
            case ECARules4AllType.Identifier:
            case ECARules4AllType.Time:
                foreach (var symbol in booleanSymbols)
                {
                    entries.Add(symbol);
                }
                break;
        }
        RuleUtils.AddToDropdownInAlphabeticalOrder(checkSymbol, entries);
    }
    
    void DropdownValueChangedCheckSymbol(Dropdown change)
    {
        //retrieve selected string 
        selectedSymbol = change.options[change.value].text;
        
        ECARules4AllType type = stateVariables[propertySelected].Item1;
        compareWithType = type;
        
        // Used to sort each dropdown's options
        List<string> entries = new List<string>();

        switch (type)
        {
            case ECARules4AllType.Color:
                compareWithDrop.ClearOptions();
                compareWithDrop.gameObject.SetActive(true);
                compareWithDrop.options.Add(new Dropdown.OptionData(""));
                // Add colors to dropdown
                foreach (KeyValuePair<string, Color> kvp in DropdownHandler.colorDict)
                    entries.Add(kvp.Key);
                compareWithDrop.RefreshShownValue();
                //if previous activated, hide the input field
                compareWithInputField.gameObject.SetActive(false);
                break;
            case ECARules4AllType.Position:
                compareWithDrop.ClearOptions();
                compareWithDrop.gameObject.SetActive(true);
                compareWithDrop.options.Add(new Dropdown.OptionData(""));
                Vector3 selectedPos = raycastPointer.pos;
                if (selectedPos != Vector3.zero) entries.Add("Last selected position");
                compareWithDrop.RefreshShownValue();
                //if previous activated, hide the input field
                compareWithInputField.gameObject.SetActive(false);
                break;
            case ECARules4AllType.Boolean:
                compareWithDrop.ClearOptions();
                compareWithDrop.gameObject.SetActive(true);
                compareWithDrop.options.Add(new Dropdown.OptionData(""));
                //TODO togliere questo schifo
                if (toCheckSelectedType == "ECALight" || toCheckSelectedType == "Light")
                {
                    entries.Add("on");
                    entries.Add("off");
                }
                else
                {
                    entries.Add("true");
                    entries.Add("false");
                }
                
                compareWithDrop.RefreshShownValue();
                //if previous activated, hide the input field
                compareWithInputField.gameObject.SetActive(false);
                break;
            case ECARules4AllType.Float:
            case ECARules4AllType.Time:
            case ECARules4AllType.Rotation:
                ActivateInputField(InputField.CharacterValidation.Decimal);
                break;
            case ECARules4AllType.Integer:
                ActivateInputField(InputField.CharacterValidation.Integer);
                break;
            case ECARules4AllType.Text:
                ActivateInputField(InputField.CharacterValidation.Alphanumeric);
                break;

            case ECARules4AllType.Identifier:
                //TODO alias
                if (propertySelected == "pov")
                {
                    compareWithDrop.ClearOptions();
                    compareWithDrop.gameObject.SetActive(true);
                    compareWithDrop.options.Add(new Dropdown.OptionData(""));
                    compareWithDrop.options.Add(new Dropdown.OptionData("First"));
                    compareWithDrop.options.Add(new Dropdown.OptionData("Third"));
                    compareWithDrop.RefreshShownValue();
                    //if previous activated, hide the input field
                    compareWithInputField.gameObject.SetActive(false);
                }
                
                break;
        }
        RuleUtils.AddToDropdownInAlphabeticalOrder(compareWithDrop, entries);
    }

    public void PopulateToCheckDropdown()
    {
        
        toCheck.ClearOptions();
        toCheckDictionary = RuleUtils.FindSubjects();
        toCheck.options.Add(new Dropdown.OptionData(""));
        toCheck.options.Add(new Dropdown.OptionData("Last selected object"));
        
        // Used to sort each dropdown's options
        List<string> entries = new List<string>();

        for (int i = 0; i < toCheckDictionary.Count; i++)
        {
            foreach(KeyValuePair<GameObject,string> entry in toCheckDictionary[i])
            {
                //TODO handle alias
                string type = RuleUtils.FindInnerTypeNotBehaviour(entry.Key);
                type = RuleUtils.RemoveECAFromString(type);
                entries.Add(type + " " + entry.Key.name);
                toCheck.RefreshShownValue();
            }
        }
        RuleUtils.AddToDropdownInAlphabeticalOrder(toCheck, entries);
        toCheck.RefreshShownValue();
    }
    
    void DisableNextComponent(string changedField)
    {
        switch (changedField)
        {
            // ToCheck
            case "toCheck":
                property.gameObject.SetActive(false); 
                checkSymbol.gameObject.SetActive(false); 
                compareWithDrop.gameObject.SetActive(false); 
                compareWithInputField.gameObject.SetActive(false);
                
                break;
            // Property
            case "property": 
                checkSymbol.gameObject.SetActive(false); 
                compareWithDrop.gameObject.SetActive(false); 
                compareWithInputField.gameObject.SetActive(false);
                break;
            
                
        }
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
                    change.value = i; return;
                }
            }
            //return change.options[change.value].text;
        }

        Debug.Log("Object not found");
    }

    void ActivateInputField(InputField.CharacterValidation characterValidation)
    {
        //activate
        compareWithInputField.gameObject.SetActive(true);
        //refresh new value
        compareWithInputField.Select();
        compareWithInputField.text = "";
        compareWithInputField.characterValidation = characterValidation;
        //if previous activated, hide the dropdown
        compareWithDrop.gameObject.SetActive(false);
    }
}
