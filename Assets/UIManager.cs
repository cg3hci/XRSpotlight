using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EcaRules;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    private static EcaRule _popUpEcaRule = null;
    private static ButtonsHandle.RuleString popUpRuleString = new ButtonsHandle.RuleString();

    public static ButtonsHandle.RuleString PopUpRuleString
    {
        get => popUpRuleString;
        set => popUpRuleString = value;
    }

    public static EcaRule popUpEcaRule
    {
        get => _popUpEcaRule;
        set => _popUpEcaRule = value;
    }

    //HandUI buttons
    [Header("HandUI components")] public GameObject btnAddAction;
    public GameObject btnAddRule;
    public GameObject btnTestRule;
    public GameObject btnAddCondition;
    public GameObject btnObjectManager;
    public GameObject btnShowHideRules;
    public GameObject btnStopTesting;
    public GameObject TextTestRule;
    public GameObject textShowHideRules;
    public GameObject handUIButtons;
    public GameObject keyboard;
    public GameObject clipboard;
    public GameObject editModeHandCanvasUI;
    public GameObject editorCanvasInterface;


    [Header("Big Canvas")] public GameObject canvasInterface;

    [Header("Rule Editor")] public GameObject ruleEditor;
    public GameObject actionPrefab;
    public GameObject simpleConditionPrefab;
    public GameObject compositeConditionPrefab;
    public Transform contentActionTransform;

    public Transform contentConditionTransform;

    // PopUps
    public GameObject popUpSaveDiscard;
    public GameObject popUpIncompleteRule;
    public GameObject popUpGoBack;

    [Header("Rule List")] public GameObject ruleList;
    public GameObject ruleListContent;

    // Rule prefab
    public GameObject rulePrefab;

    [Header("Object Manager")] public GameObject objectManager;
    public GameObject placeholderObjectPrefab;
    public GameObject replacementObjectPrefab;
    public GameObject contentPlaceholder;
    public GameObject contentReplacement;
    public GameObject placeholderString;
    public GameObject replacementString;

    [Header("Lock UI")]
    //AccessUI
    public string passcode = "123";

    public Text passcodeText;
    public GameObject accessUI;
    public GameObject handUI;
    private bool editorLocked = true;

    [Header("Others")] public GameObject playerCamera;
    public ButtonsHandle buttonsHandle;

    private bool testingSingleRule = false;
    private bool testingAllRules = false;
    public static List<string> items = new List<string>();
    

    private string currentReplacement, currentPlaceholder;

    //TODO: completare le funzioni dell'object manager
    public void selectPlaceholder(GameObject gameObj)
    {
        string t = gameObj.name;
        Debug.Log("placeholder selected, t:"+ t);
        placeholderString.GetComponent<Text>().text = t;
        currentPlaceholder = t;
        if (checkPlaceholder(t) != null)
        {
            currentReplacement = checkPlaceholder(t);
            replacementString.GetComponent<Text>().text = currentReplacement;

        }
        else
        {
            replacementString.GetComponent<Text>().text = "...";
        }
    }

    private string checkPlaceholder(string placeholderName)
    {
        foreach (var rule in EcaRuleEngine.GetInstance().Rules())
        {
            foreach (var action in rule.GetActions())
            {
                if (action.GetObject() == "mesh" && placeholderName == action.GetSubject().name)
                {
                    //TODO: prevedere la possibilità che ci siano più regole di cambio mesh per lo stesso placeholder
                    return action.GetModifierValue().ToString();
                }
            }
        }
        return null;
    }

    public void selectReplacement(GameObject gameObj)
    {
        if (currentPlaceholder != null)
        {
            string t = gameObj.name;
            replacementString.GetComponent<Text>().text = t;
            currentReplacement = t;
            string sceneName = SceneManager.GetActiveScene().name;
            EcaRule newReplacement = new EcaRule(
                new EcaAction(GameObject.Find("Player"), "teleports to",
                    GameObject.Find(sceneName)),
                new List<EcaAction>
                {
                    new EcaAction(GameObject.Find(currentPlaceholder), "changes", "mesh", "to", currentReplacement),
                }
            );
            EcaRuleEngine.GetInstance().Add(newReplacement);
            TextRuleSerializer ser = new TextRuleSerializer();
            ser.SaveRules(System.IO.Path.Combine(Application.streamingAssetsPath, "storedRules.txt"));
            EcaEventBus.GetInstance().Publish( new EcaAction(GameObject.Find("Player"), "teleports to", GameObject.Find(sceneName)));
            GameObject viewport = ruleList.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
            GameObject newRulePrefab = Instantiate(Resources.Load("Rule"), viewport.transform) as GameObject;
            // Fix the new rule's transform
            // newRule.transform.rotation.Set(0, 0, 0, 0);
            newRulePrefab.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, 0f);
            newRulePrefab.transform.localScale = Vector3.one;    
            GameObject ruleString = newRulePrefab.transform.GetChild(0).gameObject;
            Text text = ruleString.GetComponent<Text>();
            TextRuleSerializer textRuleSerializer = new TextRuleSerializer();
            StringWriter stringWriter = new StringWriter();
            textRuleSerializer.PrintRule(newReplacement, stringWriter);
            string label = stringWriter.ToString();
            text.text = label;
            //TODO cleanup
        }
    }

    private void Awake()
    {
        StateLock();
        accessUI.SetActive(false);
        var info = new DirectoryInfo(Application.dataPath + "/Resources/Inventory/Meshes");
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            //For each file in the directory we'd like to know only if prefabs are available (excluding everything else, including .meta files)
            var test = file.ToString().Split('\\');
            var mName = test[test.Length - 1].Split('.').Last().ToLower();
            if (mName.Contains("fbx") || mName.Contains("obj"))
            {
                items.Add(test[test.Length - 1].Split('.')[0]);
            }
        }
         
        foreach (var element in items)
        {
            var buttonPrefab = Instantiate(replacementObjectPrefab, contentReplacement.transform);
            buttonPrefab.SetActive(true);
            buttonPrefab.name = element;
            buttonPrefab.GetComponentInChildren<Text>().text = element;
        }

        var listOfPlaceholders = FindObjectsOfType<Placeholder>();

        foreach (var placeholder in listOfPlaceholders)
        {
            var buttonPrefab = Instantiate(placeholderObjectPrefab, contentPlaceholder.transform);
            buttonPrefab.SetActive(true);
            buttonPrefab.name = placeholder.name;
            buttonPrefab.GetComponentInChildren<Text>().text = placeholder.name;
        }

        //TODO sistemare dopo la demo, è stato fatto per non far vedere la canvas all'avvio della scena ma per popolare le regole bisogna attivarla
        ruleList.SetActive(true);
        //string path = Path.Combine("Assets", Path.Combine("Resources", "storedRules.txt"));
        //string path = Path.Combine(Directory.GetCurrentDirectory(), "storedRules.txt");
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "storedRules.txt");
        RuleUtils.LoadRulesAndAddToUI(path);
        editorCanvasInterface.SetActive(false);

        // TextRuleSerializer ser = new TextRuleSerializer();
        // ser.SaveRules(path);

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode mode)
    {
        EcaEventBus.GetInstance().Publish(new EcaAction(GameObject.Find("Player"), "teleports to", GameObject.Find(SceneManager.GetActiveScene().name)));
    }

    //State for login mechanism
    private void StateLock()
    {
        if (editorLocked)
        {
            accessUI.SetActive(!accessUI.activeSelf);
            btnAddAction.SetActive(false);
            btnAddCondition.SetActive(false);
            btnAddRule.SetActive(false);
            btnObjectManager.SetActive(false);
            btnTestRule.SetActive(false);
            btnShowHideRules.SetActive(false);
            clipboard.SetActive(false);
            editModeHandCanvasUI.SetActive(false);
            keyboard.SetActive(false);
            textShowHideRules.GetComponent<Text>().text = "Show Rules";
        }
        else
        {
            editorLocked = true;
            accessUI.SetActive(false);
            handUI.SetActive(false);
            canvasInterface.SetActive(false);
            btnAddAction.SetActive(false);
            btnAddCondition.SetActive(false);
            btnAddRule.SetActive(false);
            btnObjectManager.SetActive(false);
            btnTestRule.SetActive(false);
            btnShowHideRules.SetActive(false);
            clipboard.SetActive(false);
            keyboard.SetActive(false);
            editModeHandCanvasUI.SetActive(true);
            textShowHideRules.GetComponent<Text>().text = "Show Rules";
        }
    }

    //Rule list state, after successful login
    private void StateStart()
    {
        editorLocked = false;
        accessUI.SetActive(false);
        editModeHandCanvasUI.SetActive(true);

        handUI.SetActive(true);
        btnAddRule.SetActive(true);
        btnTestRule.SetActive(true);
        btnObjectManager.SetActive(true);
        btnShowHideRules.SetActive(true);
        TextTestRule.GetComponent<Text>().text = "Test All Rules";
        btnAddCondition.SetActive(false);
        btnAddAction.SetActive(false);
        btnStopTesting.SetActive(false);
        
        clipboard.SetActive(true);
        ruleList.SetActive(true);
        ruleEditor.SetActive(false);
        objectManager.SetActive(false);
        handUIButtons.SetActive(true);
        keyboard.SetActive(false);
    }

    //object manager state
    private void StateObjectManager()
    {
        handUI.SetActive(true);
        canvasInterface.SetActive(true);
        accessUI.SetActive(false);
        TextTestRule.GetComponent<Text>().text = "Test All Rules";
        
        objectManager.SetActive(true);

        btnShowHideRules.SetActive(false);
        btnAddRule.SetActive(false);
        btnTestRule.SetActive(false);
        btnAddCondition.SetActive(false);
        btnAddAction.SetActive(false);
        btnObjectManager.SetActive(false);
        btnStopTesting.SetActive(false);
        ruleList.SetActive(false);
        ruleEditor.SetActive(false);
    }

    //state for inputfield
    public void StateKeyboard()
    {
        ruleEditor.SetActive(true);
        keyboard.SetActive(true);
        ruleList.SetActive(false);
        handUIButtons.SetActive(false);
        clipboard.SetActive(false);
    }

    //state for creating a new rule
    private void StateNewRule()
    {
        if (!canvasInterface.activeInHierarchy)
        {
            InstantiateCanvas(true);
        }
        objectManager.SetActive(false);
        ruleEditor.SetActive(true);
        ruleList.SetActive(false);

        TextTestRule.GetComponent<Text>().text = "Test This Rule";
        handUIButtons.SetActive(true);
        btnAddAction.SetActive(true);
        btnAddCondition.SetActive(true);
        btnTestRule.SetActive(true);
        btnObjectManager.SetActive(false);
        btnStopTesting.SetActive(false);
        btnShowHideRules.SetActive(false);
        btnAddRule.SetActive(false);

        
        clipboard.SetActive(true);
        keyboard.SetActive(false);
    }

    //TODO todo
    private void StateTesting()
    {
        canvasInterface.SetActive(false);
        btnAddRule.SetActive(false);
        btnAddAction.SetActive(false);
        btnAddCondition.SetActive(false);
        btnShowHideRules.SetActive(false);
        btnTestRule.SetActive(false);
        btnObjectManager.SetActive(false);
        btnStopTesting.SetActive(true);
    }

    public void LockButton()
    {
        StateLock();
    }

    public void CheckPasscode()
    {
        if (passcode.Equals(passcodeText.text))
        {
            passcodeText.text = "Enter passcode";
            StateStart();
        }
        else
        {
            passcodeText.text = "Wrong passcode";
            editorLocked = true;
        }
    }

    public void AddNumber(string n)
    {
        bool containsLetter = Regex.IsMatch(passcodeText.text, "[a-zA-Z]");

        if (containsLetter)
        {
            passcodeText.text = "";
        }

        passcodeText.text += n;
    }

    public void RemoveNumber()
    {
        passcodeText.text =
            passcodeText.text.Substring(0, passcodeText.text.Length - 1);
    }

    public void SaveRuleAndGoBack()
    {
        // DON'T CHANGE THE ORDER <3 
        ruleList.SetActive(true);
        
        EcaRule ecaRule = buttonsHandle.CreateRule();
        popUpEcaRule = ecaRule; // Used in the pop-ups
        ruleList.SetActive(false);

        ShowSavePopup();
    }

    public void PressObjectManager()
    {
        StateObjectManager();
        //StateKeyboard();
    }

    public void PressCloseKeyboard()
    {
        StateNewRule();
    }

    //TODO: questa funzione deve anche salvare gli accoppiamenti tra placeholder e mesh scelta, oppure con un pulsante dedicato
    public void PressQuitObjectManager()
    {
        currentPlaceholder = null;
        currentReplacement = null;
        StateStart();
    }

    public void PressAddAction()
    {
        if (!canvasInterface.activeInHierarchy)
        {
            InstantiateCanvas(true);
        }

        //add action
        Instantiate(actionPrefab, contentActionTransform);
    }
    
    public void PressAddNewRule()
    {
        StateNewRule();
    }

    public void PressStopTesting()
    {
        if (testingSingleRule)
        {
            StateNewRule();
            testingSingleRule = false;
        }

        if (testingAllRules)
        {
            StateStart();
            testingAllRules = false;
        }
    }

    private void ExecuteRule(List<EcaRule> rules)
    {
        foreach (var rule in rules)
        {
            EcaAction eventEcaAction = rule.GetEvent();
            //makes the event happen
            EcaEventBus.GetInstance().Publish(eventEcaAction);
        }
    }

    public void PressTestRule()
    {
        if (ruleEditor.activeInHierarchy)
        {
            //Test single rule
            testingSingleRule = true;
            StateTesting();

            EcaRule ecaRule = buttonsHandle.CreateRule();
            //TODO handle this case with a popup
            if (ecaRule == null) return; //not valid rule
            ExecuteRule(new List<EcaRule>() {ecaRule});
        }
        else
        {
            //test all rules
            testingAllRules = true;
            StateTesting();
            List<EcaRule> rules = new List<EcaRule>();
            foreach (var r in EcaRuleEngine.GetInstance().Rules())
            {
                rules.Add(r);
            }

            ExecuteRule(rules);
        }
    }

    public void PressAddCondition()
    {
        if (!canvasInterface.activeInHierarchy)
        {
            InstantiateCanvas(true);
        }

        //add condition
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

    public void PressShowRules()
    {
        if (!canvasInterface.activeInHierarchy)
        {
            InstantiateCanvas(true);
            StateStart();
            textShowHideRules.GetComponent<Text>().text = "Hide Rules";
            
            if(GameObject.Find("RuleEditor")!=null)
                ButtonsHandle.DiscardChanges();
        }
        else
        {
            StateStart();
            canvasInterface.SetActive(false);
            textShowHideRules.GetComponent<Text>().text = "Show Rules";
        }

    }


    public void PressRemoveRule(GameObject rule)
    {
        // For each rule created
        foreach (var r in RuleUtils.rulesDictionary)
        {
            // Check if the gameObject corresponds to the rule we want to delete

            // If so also delete the rule itself
            if (r.Value.prefab == rule)
            {
                //remove from rule engine
                EcaRuleEngine.GetInstance().Remove(r.Value.EcaRule);
                RuleUtils.rulesDictionary.Remove(r.Key);
                Destroy(rule);
                
                RuleUtils.SaveRulesToFile();
                
                return;
            }
        }
    }

    public void InstantiateCanvas(bool visible)
    {
        canvasInterface.SetActive(visible);
        Vector3 fwd = playerCamera.transform.forward;
        canvasInterface.gameObject.transform.position = playerCamera.transform.position + (fwd * 2f);
        canvasInterface.gameObject.transform.rotation = playerCamera.transform.rotation;
    }

    public void RepositionCanvas()
    {
        Vector3 fwd = playerCamera.transform.forward;
        canvasInterface.gameObject.transform.position = playerCamera.transform.position + (fwd * 2f);
        canvasInterface.gameObject.transform.rotation = playerCamera.transform.rotation;
    }

    public void PressEditRule(GameObject obj)
    {
        /*
         Unfortunately, Rule is a prefab, so we can't give as onClick() something like a gameobject,
         so, we search for HandUIContainer, where ruleEditor is not a Null reference
         and we set active as true. We can't simply use gameobject.Find because is not active 
        */
        UIManager uiManager = GameObject.Find("HandUIContainer").GetComponent<UIManager>();

        // Change state to enable rule modification
        uiManager.StateNewRule();


        //search for the rule
        RuleUtils.RulesStruct rulesStruct = RuleUtils.rulesDictionary[obj.name];
        ButtonsHandle.modifyingRuleId = obj.name;

        //remove from rule engine
        EcaRuleEngine.GetInstance().Remove(rulesStruct.EcaRule);

        //TODO gestione colori oggetti selezionati?
        //Event:
        DropdownHandler eventDropdownHandler = GameObject.Find("Event").GetComponent<DropdownHandler>();
        SetActionsDropdown(eventDropdownHandler, rulesStruct.ruleString.EventString);       

        //Action(s):
        var allActions = GameObject.FindGameObjectsWithTag("Action");
        var actions = from act in allActions where act.name != "ActionPrefab" select act;
        foreach(var ac in actions)
            Destroy(ac);
        for (int i = 0; i < rulesStruct.EcaRule.GetActions().Count; i++)
        {
            //we re-instantiate the first action when we discard changes, so if we instantiate that again
            GameObject singleAction = Instantiate(uiManager.actionPrefab, uiManager.contentActionTransform);

            SetActionsDropdown(singleAction.GetComponent<DropdownHandler>(), rulesStruct.ruleString.ActionsString[i]);
        }


        //Conditions:
        if (rulesStruct.ruleString.Conditions.Count > 0)
        {
            //if present, remove all conditions prefab
            if (GameObject.Find("SimpleConditionPrefab(Clone)") &&
                GameObject.Find("SimpleConditionPrefab(Clone)").activeInHierarchy)
            {
                Destroy(GameObject.Find("SimpleConditionPrefab(Clone)") );
            }

            var compositeConditionsIt = from act in GameObject.FindGameObjectsWithTag("CompositeCondition") where act.name != "CompositeConditionPrefab" select act;
            var compositeConditions = compositeConditionsIt.ToArray();
            foreach(var cc in compositeConditions)
                Destroy(cc);
            
            for (int i = 0; i < rulesStruct.ruleString.Conditions.Count; i++)
            {
                GameObject parentConditionList = GameObject.Find("PartsBuilder");
                parentConditionList.transform.Find("ConditionList").gameObject.SetActive(true);

                GameObject parentHeaderCondition = GameObject.Find("Labels");
                parentHeaderCondition.transform.Find("_headerCondition").gameObject.SetActive(true);
                GameObject prefab;
                prefab = i == 0 ? uiManager.simpleConditionPrefab :  uiManager.compositeConditionPrefab;
                GameObject simple=Instantiate(prefab, uiManager.contentConditionTransform);
                    
                SetConditionsDropdown(simple.GetComponent<ConditionDropdownHandler>(),
                    rulesStruct.ruleString.Conditions[i], false);
            }
        }
    }

    private void SetActionsDropdown(DropdownHandler dropdownHandler, ButtonsHandle.StringAction stringAction)
    {
        string prevEventSubj = stringAction.Subj;
        string prevEventVerb = stringAction.Verb;
        string prevEventObj = stringAction.Obj;
        string prevEventValue = stringAction.Value;

        //subj
        for (int i = 0; i < dropdownHandler.subj.options.Count; i++)
        {
            if (dropdownHandler.subj.options[i].text.Equals(prevEventSubj))
            {
                dropdownHandler.subj.value = i;
                break;
            }
        }

        //this function is called on awake, so we need to call it again
        dropdownHandler.Subjects = RuleUtils.FindSubjects();

        //verb
        for (int i = 0; i < dropdownHandler.verb.options.Count; i++)
        {
            if (dropdownHandler.verb.options[i].text.Equals(prevEventVerb))
            {
                dropdownHandler.verb.value = i;
                break;
            }
        }

        //obj
        if (!string.IsNullOrEmpty(prevEventObj))
        {
            //object value
            if (!string.IsNullOrEmpty(prevEventValue))
            {
                //e.g. active...
                for (int i = 0; i < dropdownHandler.objValueDrop.options.Count; i++)
                {
                    if (dropdownHandler.objValueDrop.options[i].text.Equals(prevEventObj))
                    {
                        dropdownHandler.objValueDrop.value = i;
                        break;
                    }
                }
                //prep
                //it shouldn't be necessary, because the prop is setted along with the change of 
                //the verb
                /*for (int i = 0; i < dropdownHandler.prepDrop.options.Count; i++)
                {
                    if (dropdownHandler.prepDrop.options[i].text.Equals(stringAction.Prep))
                    {
                        dropdownHandler.prepDrop.value = i;
                    }
                }*/
            }
            else
            {
                //object without value
                for (int i = 0; i < dropdownHandler.objDrop.options.Count; i++)
                {
                    if (dropdownHandler.objDrop.options[i].text.Equals(prevEventObj))
                    {
                        dropdownHandler.objDrop.value = i;
                        if (!string.IsNullOrEmpty(stringAction.ObjThe)) dropdownHandler.theGameObject.SetActive(true);
                        break;
                    }
                }
            }

            //InputField
            if (!string.IsNullOrEmpty(dropdownHandler.VerbSelectedType))
            {
                if (dropdownHandler.VerbSelectedType.Equals("String") ||
                    dropdownHandler.VerbSelectedType.Equals("Single") ||
                    dropdownHandler.VerbSelectedType.Equals("Int32") ||
                    dropdownHandler.VerbSelectedType.Equals("Double"))
                {
                    dropdownHandler.objField.text = prevEventObj;
                }
            }
        }

        //value
        if (!string.IsNullOrEmpty(prevEventValue))
        {
            if (prevEventValue.Contains("#")) // if it's a color
                // Gets the proper color for the dropdown
                prevEventValue = DropdownHandler.colorDictHex.FirstOrDefault(x => x.Value == prevEventValue).Key;

            for (int i = 0; i < dropdownHandler.value.options.Count; i++)
            {
                if (dropdownHandler.value.options[i].text.Equals(prevEventValue))
                {
                    dropdownHandler.value.value = i;
                }
            }

            //InputField
            if (!string.IsNullOrEmpty(dropdownHandler.ObjSelectedType))
            {
                if (dropdownHandler.ObjSelectedType.Equals("String") ||
                    dropdownHandler.ObjSelectedType.Equals("Single") ||
                    dropdownHandler.ObjSelectedType.Equals("Int32") ||
                    dropdownHandler.ObjSelectedType.Equals("Double"))
                {
                    dropdownHandler.objField.text = prevEventValue;
                }
            }
        }
    }

    private void SetConditionsDropdown(ConditionDropdownHandler dropdownHandler,
        ButtonsHandle.StringCondition stringAction,
        bool composite)
    {
        string prevEventToCheck = stringAction.ToCheck;
        string prevEventProperty = stringAction.Property;
        string prevEventCheckSymbol = stringAction.CheckSymbol;
        string prevEventCompareWidth = stringAction.CompareWith;

        //toCheck
        for (int i = 0; i < dropdownHandler.toCheck.options.Count; i++)
        {
            if (dropdownHandler.toCheck.options[i].text.Equals(prevEventToCheck))
            {
                dropdownHandler.toCheck.value = i;
            }
        }

        //property
        for (int i = 0; i < dropdownHandler.property.options.Count; i++)
        {
            if (dropdownHandler.property.options[i].text.Equals(prevEventProperty))
            {
                dropdownHandler.property.value = i;
            }
        }

        //check symbol
        for (int i = 0; i < dropdownHandler.checkSymbol.options.Count; i++)
        {
            if (dropdownHandler.checkSymbol.options[i].text.Equals(prevEventCheckSymbol))
            {
                dropdownHandler.checkSymbol.value = i;
            }
        }

        //comparewidth
        for (int i = 0; i < dropdownHandler.compareWithDrop.options.Count; i++)
        {
            if (dropdownHandler.compareWithDrop.options[i].text.Equals(prevEventCompareWidth))
            {
                dropdownHandler.compareWithDrop.value = i;
            }
        }

        //there is an inputfield to set, the symbol is inside mathematical symbols
        foreach (var symbol in ConditionDropdownHandler.matemathicalSymbols)
        {
            if (prevEventCheckSymbol.Equals(symbol))
            {
                dropdownHandler.compareWithInputField.text = prevEventCompareWidth;
            }
            else dropdownHandler.compareWithInputField.text = prevEventCompareWidth;
        }

        if (composite)
        {
            if (stringAction.AndOr != "")
            {
                for (int i = 0; i < dropdownHandler.andOr.options.Count; i++)
                {
                    if (dropdownHandler.andOr.options[i].text.Equals(stringAction.AndOr))
                    {
                        dropdownHandler.andOr.value = i;
                    }
                }
            }
        }
    }


    public void ShowSavePopup()
    {
        if (popUpEcaRule != null) // if the rule is valid
        {
            // Then we ask the user if they want to save or discard the rule

            // If the user wants to save the rule then we add it to the rule engine
            // And to the ruleList

            // Set the the PopUp_SaveDiscard to active..?
            popUpSaveDiscard.SetActive(true);
        }
        else
        {
            // Open pop-up saying the rule isn't complete or simply not valid
            // pop-up only has an OK button that takes me back to the RuleList

            // DON'T DISCARDE CHANGES
            popUpGoBack.SetActive(true);
        }
    }

    public void PressSavePopUp()
    {
        EcaRuleEngine.GetInstance().Add(popUpEcaRule);
        ButtonsHandle.DiscardChanges();
        
        RuleUtils.SaveRulesToFile();
        
        popUpSaveDiscard.SetActive(false);
        popUpGoBack.SetActive(false);
        popUpIncompleteRule.SetActive(false);

        StateStart();

        CreateOrModifyRulePrefab();

        popUpEcaRule = null;
        PopUpRuleString = new ButtonsHandle.RuleString();
    }

    public void PressDiscardPopUp()
    {
        ButtonsHandle.DiscardChanges();
        popUpEcaRule = null;
        PopUpRuleString = new ButtonsHandle.RuleString();

        popUpSaveDiscard.SetActive(false);
        popUpGoBack.SetActive(false);
        popUpIncompleteRule.SetActive(false);

        StateStart();
    }

    public void PressOkIncompletePopup()
    {
        popUpIncompleteRule.SetActive(false);
        popUpSaveDiscard.SetActive(false);
        popUpGoBack.SetActive(false);

        StateNewRule();
    }


    public void CreateOrModifyRulePrefab()
    {
        if (ButtonsHandle.modifyingRuleId.Length > 0)
        {
            foreach (var r in RuleUtils.rulesDictionary.ToList())
            {
                // Check if the gameObject corresponds to the rule we want to edit
                if (r.Key == ButtonsHandle.modifyingRuleId)
                {
                    GameObject prefab = RuleUtils.rulesDictionary[r.Key].prefab;
                    ButtonsHandle.CreateRuleRow(prefab, popUpEcaRule);
                    RuleUtils.rulesDictionary[r.Key] = new RuleUtils.RulesStruct(prefab, popUpEcaRule, PopUpRuleString);
                    ButtonsHandle.modifyingRuleId = "";
                    break;
                }
            }
        }
        else
        {
            //if we don't find the rule, we have to create a new one
            string newRuleUuid = Guid.NewGuid().ToString();
            GameObject gameObject = ButtonsHandle.CreateRuleRow(null, popUpEcaRule);
            gameObject.name = newRuleUuid;

            if (!RuleUtils.rulesDictionary.ContainsKey(newRuleUuid))
            {
                RuleUtils.rulesDictionary.Add(newRuleUuid, new RuleUtils.RulesStruct(gameObject, popUpEcaRule, PopUpRuleString));
            }
        }
    }

    public void PressSaveFloppy()
    {
        EcaRule ecaRule = buttonsHandle.CreateRule();
        popUpEcaRule = ecaRule; // Used in the pop-ups

        // IF the rule is valid, we simply save it. No interaction needed
        if (popUpEcaRule != null)
        {
            PressSavePopUp();
        }
        // Otherwise we alert the user that the rule is not valid
        else
        {
            popUpIncompleteRule.SetActive(true);
        }
    }

    public void PressQuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}