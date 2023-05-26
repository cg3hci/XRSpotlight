using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
#if MRTK
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
#elif STEAMVR
using Valve.VR.InteractionSystem;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;
using XRSpotlightGUI;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class RuleEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/XRSpotlight")]

    public static void ShowExample()
    {
        RuleEditor wnd = GetWindow<RuleEditor>();
        wnd.titleContent = new GUIContent("XRSpotlight");
    }

    private Label activeLabel;
    private Foldout interactableList;
    private bool listFold = true;
    //private VisualElement objInspection, propertiesMenu, supportedBehaviours;
    private bool showEmpty = false;
    private InferenceEngine engine;
    private Button activeButton;
    private Type inspectorType;
    private bool highlighting = false;
    private List<InferredRule[]> rulesSelectedObject;
    private List<InferredBehaviour[]> behavioursSelectedObject;
    private List<GameObject> selected;
    private AggregationType aggregation = AggregationType.Phase;
    private VisualElement panelContent;

    
    private const double maxDistance = 20.0;
    public void CreateGUI()
    {  
        this.showEmpty = false;
        engine = InferenceEngine.GetInstance();
        
        var assembly = typeof(UnityEditor.EditorWindow).Assembly;
        inspectorType = assembly.GetType("UnityEditor.InspectorWindow");
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/XRSpotlightGUI/RuleEditor.uss");
        // Each editor window contains a root VisualElement object
        VisualElement panel = rootVisualElement;
        
        ScrollView root = new ScrollView();
        panel.Add(root);
        interactableList = new Foldout();
        interactableList.text = "Interactable Objects";
        //by default the interactable list is hidden
        interactableList.value = false;
        interactableList.AddToClassList("section");
        root.styleSheets.Add(styleSheet);

        root.Add(interactableList);

        CreateInteractableList(); 
        
        
        activeLabel = new Label("Active object: ");
        root.Add(activeLabel);
 
        Selection.selectionChanged += () => { UpdatePanels(); };
        

        var space = new VisualElement();
        space.AddToClassList("spacer-field");
        root.Add(space);

        panelContent = new VisualElement();
        panelContent.name = "panelContent";
        root.Add(panelContent);
        

        rulesSelectedObject = new List<InferredRule[]>();
        behavioursSelectedObject = new List<InferredBehaviour[]>();
        selected = new List<GameObject>();



    }

    private void UpdatePanels()
    {
        rulesSelectedObject.Clear();
        behavioursSelectedObject.Clear();
        selected.Clear();
        panelContent.Clear();
        
        if (IsActiveObject())
        {
            activeLabel.text = "Active object: " + Selection.activeObject.name;
           
            
            foreach (var obj in Selection.objects)
            {
                if (obj is GameObject gameObject)
                {
                    (InferredRule[], InferredBehaviour[]) rulesAndBehaviours = engine.InferRuleByGameObject(gameObject);
                    rulesSelectedObject.Add(rulesAndBehaviours.Item1);
                    behavioursSelectedObject.Add(rulesAndBehaviours.Item2);
                    selected.Add(gameObject);
                }

                
                panelContent.Clear();
                for (int i = 0; i < selected.Count; i++)
                {
                    PopulatePropertiesMenu(i);
                    PopulateSupportedBehaviours(i);
                    PopulateInspectionPanel(i);
                }
            }
            
        }
        else if(Selection.activeGameObject == null || Selection.activeGameObject!= null && Selection.activeObject.name == SceneManager.GetActiveScene().name)
        {
            activeLabel.text = "Active object: " + "None";
            //PopulateInspectionPanel(null);
        }
    }

    public void ChangeTrigger(InferredRule r, Phases p)
    {
        var cpm = CopyPasteManager.GetInstance();
        cpm.Copy(r);
        cpm.Paste(r.gameObject, p, true); 
        foreach (var action in r.actions)
        {
            var co = new SerializedObject(r.gameObject.GetComponent(Type.GetType(action.componentAssemblyName)));
            var coPtr = co.FindProperty($"{action.inspector}.m_PersistentCalls.m_Calls.Array");
            for (int i = 0; i < coPtr.arraySize; i++)
            {
                coPtr.DeleteArrayElementAtIndex(i);
            }
            co.ApplyModifiedProperties();
        }
        
        this.UpdatePanels();
    }
    
    private void PopulateSupportedBehaviours(int i)
    {
        var selectedObject = selected[i];
        var supportedBehaviours = new VisualElement();
        panelContent.Add(supportedBehaviours);  
        
        var heading = new Toolbar();
        var headingToggle = new Foldout();

        var headingLabel = new Label($"Behaviours for {selectedObject.name}");
        heading.Add(headingLabel);
        heading.AddToClassList("obj-heading");

        var behavioursList = new VisualElement(); 
        
        supportedBehaviours.Clear();
        behavioursList.AddToClassList("behaviour-box");
            
        
        VisualElement row = new VisualElement();
        row.AddToClassList("behaviour-row");

        var groupByComponent = new Dictionary<string, List<InferredBehaviour>>();
        foreach (var behaviour in behavioursSelectedObject[i])
        {
            if (!groupByComponent.ContainsKey(behaviour.componentName))
            {
                groupByComponent.Add(behaviour.componentName, new List<InferredBehaviour>(){behaviour});
            }
            else 
            {
                groupByComponent[behaviour.componentName].Add(behaviour);
            }
        }
        
        foreach (var behaviour in groupByComponent)
        {
            VisualElement b = new VisualElement();
            b.AddToClassList("behaviour-item");
            
            Toggle toggle = new Toggle();
            toggle.value = true;
            toggle.RegisterValueChangedCallback(evt =>
            {
                GameObject obj = Selection.activeGameObject;
                Behaviour component = (Behaviour)obj.GetComponent(behaviour.Key);
                component.enabled = evt.newValue;
            });
            
            var definitionRow = new VisualElement();
            definitionRow.AddToClassList("behaviour-definition-row");

            foreach (var element in behaviour.Value)
            {
                foreach (var def in element.definitions)
                {
                    var label = new Label(def);
                    label.AddToClassList("behaviour-value");
                    definitionRow.Add(label);
                }
            }
            
            b.Add(definitionRow);
            b.Add(toggle);
            string componentName = "("+behaviour.Key.Substring(behaviour.Key.LastIndexOf('.') + 1)+")";
            b.Add(new Label(componentName));
            row.Add(b);
        }

        behavioursList.Add(row);
        
        headingToggle.Add(behavioursList);
        supportedBehaviours.Add(heading);
        supportedBehaviours.Add(headingToggle);
    }
    
    private void PopulatePropertiesMenu(int i)
    {
        
        var heading = new Toolbar();
        var headingToggle = new Foldout();

        var headingLabel = new Label($"Interaction Properties for {selected[i].name}");
        heading.Add(headingLabel);
        heading.AddToClassList("obj-heading");

        var propertiesList = new VisualElement(); 
        
        var propertiesMenu = new VisualElement();
        panelContent.Add(propertiesMenu);

        propertiesList.AddToClassList("properties-container");

        VisualElement iconCol = new VisualElement();
        iconCol.AddToClassList("properties-icon-col");
        propertiesList.Add(iconCol);
        
        //Collider icon
        VisualElement colliderIcon = new VisualElement();
        colliderIcon.AddToClassList("collider-icon");
        iconCol.Add(colliderIcon);
        Image image = new Image(); 
        Collider[] colliders = Utils.findCollider(selected[i]);
        bool hasCollider = colliders.Length > 0;
        List<string> colliderLabels= new List<string>();
        if (hasCollider)
        {
            image.AddToClassList("checkedIcon");
            colliderLabels.AddRange(colliders.Select(collider => "(" + collider.GetType() + ")"));
        }
        else
        {
            image.AddToClassList("uncheckedIcon");
        }
        colliderIcon.Add(image);

        //Rigidbody icon
        VisualElement rigidBodyIcon = new VisualElement();
        rigidBodyIcon.AddToClassList("collider-icon");
        iconCol.Add(rigidBodyIcon);
        Image image2 = new Image();
        Component[] rigidbodies = Utils.findRigidbody(selected[i]);
        bool hasRigidBody = rigidbodies.Length > 0;
        List<string> rigidBodyLabels = new List<string>();
        if (hasRigidBody)
        {
            image2.AddToClassList("checkedIcon");
            rigidBodyLabels.AddRange(rigidbodies.Select(rigidbody => "(" + rigidbody.GetType() + ")"));
        }
        else
        {
            image2.AddToClassList("uncheckedIcon");
        }
        rigidBodyIcon.Add(image2);
        
        VisualElement labelCol = new VisualElement();
        labelCol.AddToClassList("properties-label-col");
        propertiesList.Add(labelCol);
        
        Label label = new Label("Collider");
        label.AddToClassList("properties-value-col");
        labelCol.Add(label);
        
        Label labelR = new Label("Rigidbody");
        labelR.AddToClassList("properties-value-col");
        labelCol.Add(labelR);
        
        VisualElement typeCol = new VisualElement();
        typeCol.AddToClassList("properties-type-col");
        propertiesList.Add(typeCol);

        Label colliderLabel = new Label("(None)");
        colliderLabel.AddToClassList("properties-value-col");
        if (colliderLabels.Count > 0)
        {
            colliderLabel.text = "";
            foreach (var labelType in colliderLabels)
            {
                colliderLabel.text += labelType;
            }
        }
        typeCol.Add(colliderLabel);
        
        
        Label rigidBodyLabel = new Label("(None)");
        rigidBodyLabel.AddToClassList("properties-value-col");
        if (rigidBodyLabels.Count > 0)
        {
            rigidBodyLabel.text = "";
            foreach (var labelType in rigidBodyLabels)
            {
                rigidBodyLabel.text += labelType;
            }
        }
        
        
        typeCol.Add(rigidBodyLabel);
        
        
        /*#if MRTK
        //[Valentino] This is DEFINITELY hard-coded, for now it's here as a first step to get the system working.
        //TODO: Remove this hard-coded stuff and make it more generic (if possible).
        Interactable interactable = selected[i].GetComponent<Interactable>();
        if(interactable != null)
        {
            VisualElement voiceCommandRow = new VisualElement();
            voiceCommandRow.AddToClassList("properties-row");
        
        
            Label labelV = new Label("Voice triggered by: ");
            labelV.AddToClassList("collider-text");
            voiceCommandRow.Add(labelV);
            
            var voiceCommand = interactable.VoiceCommand;
            var needsToBeFocused = interactable.voiceRequiresFocus;

            if (needsToBeFocused)
            {
                voiceCommand += " (requires focus)";
            }
            else
            {
                voiceCommand += " (does not require focus)";
            }
            Label voiceCommandLabel = new Label(voiceCommand);
            voiceCommandLabel.AddToClassList("collider-text");
            voiceCommandRow.Add(voiceCommandLabel);
            
            propertiesList.Add(voiceCommandRow);
        }

#endif*/
        
        
        headingToggle.Add(propertiesList);
        propertiesMenu.Add(heading);
        propertiesMenu.Add(headingToggle);
    }

    private bool IsActiveObject()
    {
        return activeLabel != null && Selection.activeObject != null && Selection.activeObject.GetType() != typeof(Behaviour)
               && Selection.activeObject.GetType() != typeof(ScriptableObject) && Selection.activeObject.GetType() != typeof(SceneAsset);; 
    }

    
    public void ShowCandidatesInPaste(
        List<string> selectionOptions, 
        InferredRule ruleToPaste,
        Phases p,
        InferredAction action, 
        GameObject destination, 
        Modalities modalities)
    {
        rootVisualElement.Clear();
        var selectionOptionsUnicode = selectionOptions.Select(option => Utils.ConvertSlashToUnicodeSlash(option)).ToList();
        // Create a new field and assign it its value.
        string scriptName = null;
        int index = action.scriptName.IndexOf(",");
        if (index >= 0)
            scriptName= action.scriptName.Substring(0, index);
        else scriptName = action.scriptName;
        var normalField = new PopupField<string>("We have found many gameobjects with the script " + scriptName + ", please choose among those candidates: ",
                                                 selectionOptionsUnicode, 0);
        var popUp = new VisualElement();
        popUp.Add(normalField);
        popUp.AddToClassList("popup-field");

        Button button = new Button(() =>
        {
            var selected = normalField.value;
            CopyPasteManager.GetInstance().PasteActionInCandidate(
                ruleToPaste, p, action, destination, modalities, 
                GameObject.Find(Utils.ConvertUnicodeSlashToSlash(selected)) );
            rootVisualElement.Clear();
            CreateGUI();
            UpdatePanels();
        });
        
        // Mirror value of uxml field into the C# field.
        normalField.RegisterCallback<ChangeEvent<string>>((evt) =>
        {
            button.text = "Choose " + evt.newValue;
            
        });

        
        button.text = "Choose " + normalField.value;
        popUp.Add(button);
        rootVisualElement.Add(popUp);
    }

    private void CreateInteractableList()
    {
        if (interactableList == null) return;
        interactableList.Clear();
        GameObject[] interactables = engine.FindInteractableObjects();
        foreach (var interactable in interactables)
        {
            var interactableBtn = new Button();
            interactableBtn.text = interactable.name;
            interactableBtn.clickable.clicked += () =>
            {
                Selection.activeObject = interactable;
                activeButton?.RemoveFromClassList("interactableBtnPressed");
                interactableBtn.AddToClassList("interactableBtnPressed");
                activeButton = interactableBtn;
            };
            
            interactableBtn.AddToClassList("interactableBtn");
            interactableList.Add(interactableBtn);
        }
    }

    private void PopulateInspectionPanel(int i)
    {

        var objInspection = new VisualElement();
        panelContent.Add(objInspection);
        objInspection.Add(CreateInspectionHeading(i));
        objInspection.Add(CreatePasteButton(i));

    }


    private Button CreatePasteButton(int i)
    {
        var pasteButton = new Button();
        pasteButton.text = "Paste";
        pasteButton.AddToClassList("paste-button");
        pasteButton.name = "paste-button";
        pasteButton.clickable.clicked += () =>
        {
            if (IsActiveObject()) 
            {
                Debug.Log("Pasting in" + selected[i].name);
                CopyPasteManager.GetInstance().Paste(selected[i]);
                this.UpdatePanels();
            }else
            {
                Debug.Log("No rule to copy selected");
            }
        };
        return pasteButton;
    }
    
    private VisualElement CreateInspectionHeading(int i)
    {
        
        var result = new VisualElement();
        var heading = new Toolbar();
        var headingToggle = new Foldout();

        var headingLabel = new Label($"Interaction Rules for {selected[i].name}");
        heading.AddToClassList("obj-heading");

        if (activeButton!= null && selected[i].name != activeButton.text)
        {
            activeButton?.RemoveFromClassList("interactableBtnPressed");
            activeButton = null;
        }

        VisualElement rulesList = CreateInferredRules(Aggregate(rulesSelectedObject[i]), i);
        
        headingToggle.Add(rulesList);
        
        heading.Add(headingLabel);
        result.Add(heading);
        result.Add(headingToggle);
        return result;

    }

    private List<InferredRuleGroup> Aggregate(InferredRule[] rules)
    {
        switch (aggregation)
        {
            case AggregationType.Modality:
                return AggregateByModality(rules);
            
            case AggregationType.Action:
                return AggregateByAction(rules);
            
            case AggregationType.Phase:
                return AggregateByPhase(rules);
        }
        return AggregateByAction(rules);
    }

    private List<InferredRuleGroup> AggregateByPhase(InferredRule[] rules)
    {
        Array.Sort(rules);
        List<InferredRuleGroup> groups = new List<InferredRuleGroup>();
        for (int i = 0; i < Enum.GetValues(typeof(Phases)).Length; i++)
        {
            InferredRuleGroup group = new InferredRuleGroup()
            {
                title = label4Phase((Phases)i)
            };
            groups.Add(group);
        }

        foreach (var rule in rules)
        {
            int index = (int)rule.trigger;
            groups[index].rules.Add(rule);
            rule.interaction = Utils.GetInteractionString(rule);
        }

        return groups;
    }

    private List<InferredRuleGroup> AggregateByAction(InferredRule[] rules)
    {
        List<InferredRuleGroup> groups = new List<InferredRuleGroup>();
        groups.Add(new InferredRuleGroup());
        groups[0].title = "Events triggering the same Action";
        List<InferredRule> allRules = groups[0].rules;
        foreach (var rule in rules)
        {
            foreach (var a in rule.actions)
            {
                var existing = allRules.FindAll(x => x.actions[0].Equals(a));
                bool found = false;
                foreach (var e in existing)
                {
                    
                    //if (e.trigger == rule.trigger && e.modalities.IncludesModalities(rule.modalities))
                    //{
                    // aggregate the rule
                    if (a.eventPath != null)
                    {
                        e.interaction += $"/ {a.eventPath[a.eventPath.Length - 1].name}";
                        e.actions.Add(a);
                    }
                        
                    found = true;
                    //}
                }

                if (!found)
                {
                    InferredRule toAdd = new InferredRule()
                    {
                       
                        modalities = rule.modalities.Clone() as Modalities,
                        actions = new List<InferredAction>(1),
                        gameObject = rule.gameObject,
                        part = rule.part,
                        toolkit = rule.toolkit,
                        trigger = rule.trigger
                    };
                    if (a.eventPath != null)
                    {
                        toAdd.interaction = $"When {toAdd.gameObject.name} raises {a.eventPath[a.eventPath.Length - 1].name}";
                    }
                    toAdd.actions.Add(a);
                    
                    
                    
                    allRules.Add(toAdd);
                }
            }
        }
        return groups;
    }
    
    private List<InferredRuleGroup> AggregateByModality(InferredRule[] rules)
    {
        InferredRule[,] groups = new InferredRule[
            Enum.GetValues(typeof(ModalitiesEnum)).Length, 
            Enum.GetValues(typeof(Phases)).Length];

        for (int i = 0; i < Enum.GetValues(typeof(ModalitiesEnum)).Length; i++)
        {
            for (int j = 0; j < Enum.GetValues(typeof(Phases)).Length; j++)
            {
                groups[i, j] = new InferredRule()
                {
                    modalities = new Modalities(),
                    trigger = (Phases) j,
                    toolkit = Utils.toolkit,
                    actions = new List<InferredAction>(),
                    
                };
                
                groups[i,j].modalities.SetModality((ModalitiesEnum) i, true);
            }
        }

        foreach (var rule in rules)
        {
            int j = (int)rule.trigger;
            for (int i = 0; i < rule.modalities.modality.Length; i++)
            {
                if (rule.modalities.modality[i])
                {
                    var group = groups[i, j];
                    group.gameObject = rule.gameObject;
                    group.interaction =
                        $"When {rule.gameObject.name} is {Label4ModalityPhase((ModalitiesEnum)i, (Phases)j)}";
                    group.actions.AddRange(rule.actions);
                }
            }
        }

        List<InferredRuleGroup> ruleGroups = new List<InferredRuleGroup>();
        
       
        for (int i = 0; i < Enum.GetValues(typeof(ModalitiesEnum)).Length; i++)
        {
            InferredRuleGroup ruleGroup = new InferredRuleGroup();
            var mod = (ModalitiesEnum)i;
            ruleGroup.title = Label4Modality(mod);
            ruleGroup.pasteButton = true;
            ruleGroup.pasteModality = mod;
            for (int j = 0; j < Enum.GetValues(typeof(Phases)).Length; j++)
            {
                if (groups[i, j].actions.Count > 0)
                {
                    ruleGroup.rules.Add(groups[i,j]);
                }
                    
            }
            
            ruleGroups.Add(ruleGroup);
            
        }

        return ruleGroups;

    }

    private string Label4Modality(ModalitiesEnum mod)
    {
        switch (mod)
        {
            case ModalitiesEnum.gaze:
                return "Gaze pointing and air-tap selection";
            case ModalitiesEnum.grab:
                return "Hand pointing and grab selection";
            case ModalitiesEnum.touch:
                return "Hand pointing and touch selection";
            case ModalitiesEnum.remote:
                return "Laser pointing with remote controllers";
            case ModalitiesEnum.hand:
                return "Laser pointing with hands";
        }

        return "";
    }

    private VisualElement CreateModalityList(InferredRule rule)
    {
        var modality = new VisualElement();
        modality.AddToClassList("modality");
        
        Label modalityHead = new Label();
        modalityHead.AddToClassList("action");

       if (rule.interaction == null)
       {
           /*
            TODO decommentare questo pezzo di codice e mettere l'else dopo l'if se si rivogliono le fasi
           if (string.IsNullOrEmpty(rule.part))
               modalityHead.text = $"When {rule.gameObject.name} is {label4Phase(rule.trigger)}";
           else
               modalityHead.text = $"When the {rule.part} of {rule.gameObject.name} is {label4Phase(rule.trigger)}"; */
           rule.interaction = Utils.GetInteractionString(rule);
           
       }
       modalityHead.text = Utils.CutModalityString(rule.interaction).Item1;
        

        modalityHead.AddToClassList("prefix"); 
        modality.Add(modalityHead);


        var modalityIcons = new VisualElement();
        modalityIcons.AddToClassList("actionEvent");
        
        
        foreach (ModalitiesEnum m in Enum.GetValues(typeof(ModalitiesEnum)))
        {
            var mImage = new Image();
            mImage.AddToClassList("modalityImage");
            mImage.tooltip = Label4Modality(m);

            switch (m)
            {
                case ModalitiesEnum.gaze:
                    mImage.AddToClassList("gazeIcon");
                    break;
                case ModalitiesEnum.grab:
                    mImage.AddToClassList("grabIcon");
                    break;
                case ModalitiesEnum.hand:
                    mImage.AddToClassList("handIcon");
                    break;
                case ModalitiesEnum.remote:
                    mImage.AddToClassList("remoteIcon");
                    break;
                case ModalitiesEnum.touch:
                    mImage.AddToClassList("touchIcon");
                    break;
            }

            if (!rule.modalities.GetModality(m))
            {
                mImage.AddToClassList("missingModality");
            }
            modalityIcons.Add(mImage);
        }
        modality.Add(modalityIcons);

        return modality;
    }
    
    private VisualElement CreateThen()
    {
        var thenGroup = new VisualElement();
        thenGroup.AddToClassList("modality");

        var modalityHead = new Label("Then");
        modalityHead.AddToClassList("prefix");
        modalityHead.AddToClassList("indent");
        thenGroup.Add(modalityHead);

        return thenGroup;
    }
    
    private VisualElement CreateInferredRules(List<InferredRuleGroup> groups, int i, bool similarityMode = false)
    {
        VisualElement rule = new VisualElement();
        rule.AddToClassList("rule");

        rule.Add(CreateDropdown());
        var space = new VisualElement();
        space.AddToClassList("spacer-field");
        rule.Add(space);

        foreach (InferredRuleGroup group in groups)
        {
            if (group.title != null)
            {
                var titleGroup = new VisualElement();
                var titleLabel = new Label();
                titleLabel.AddToClassList("action-group");
                titleLabel.text = group.title;
                titleGroup.AddToClassList("rule-title");
                titleGroup.Add(titleLabel);
                rule.Add(titleGroup);

                if (similarityMode)
                { 
                    var selectBtn = new Button();
                    selectBtn.tooltip = "Select this object";
                    selectBtn.AddToClassList("spotlight");
                    selectBtn.AddToClassList("black-bgr");
                    titleGroup.Add(selectBtn);
                    selectBtn.clicked += () =>
                    {
                        Selection.activeObject = group.rules[0].gameObject;
                        this.UpdatePanels();
                    };
                }
            }

            if (group.rules.Count == 0 && group.title != null)
            {
                var noRules = new Label();
                noRules.text = "No rules";
                noRules.AddToClassList("rule-row");
                rule.Add(noRules);
            }

            Foldout similar = new Foldout();
            similar.SetValueWithoutNotify(false);
            for (int j = 0 ; j < group.rules.Count; j ++)
            {
                var inferredRule = group.rules[j];
                if (!similarityMode)
                {
                    var ruleRow = new VisualElement();
                    ruleRow.AddToClassList("rule-row");
                    ruleRow.Add(CreatePhaseIcon(inferredRule.trigger));
                    CreateInferredActions(ruleRow, inferredRule, false);
                    rule.Add(ruleRow);
                }
                else
                {
                    VisualElement c = similar;
                    switch (j)
                    {
                        case 0:
                            c = rule;
                            goto default;
                        case 1:
                            similar.text = $"Other similar rules for {inferredRule.gameObject.name}... ";
                            rule.Add(similar);
                            goto default;
                        default:
                            var ruleRow = new VisualElement();
                            ruleRow.AddToClassList("rule-row");
                            ruleRow.Add(CreatePhaseIcon(inferredRule.trigger));
                            CreateInferredActions(ruleRow, inferredRule, true);
                            c.Add(ruleRow);
                            break;
                                
                    }
                }
            }
            
           
            
            if (group.pasteButton)
            {
                // we have only one modality, we can add a paste modality button
                Button pasteModality = new Button(() =>
                {
                    if (IsActiveObject()) 
                    {
                        Debug.Log($"Pasting in {selected[i].name} using the {Label4Modality(group.pasteModality)} modality");
                        Modalities destModalities = new Modalities();
                        destModalities.SetModality(group.pasteModality, true);
                        CopyPasteManager.GetInstance().Paste(selected[i], destModalities);
                        this.UpdatePanels();
                    }else
                    {
                        Debug.Log("No rule to copy selected");
                    }
                });

                pasteModality.text = $"Paste with the {Label4Modality(group.pasteModality)}";
                
                rule.Add(pasteModality);
            }
        }
        
        return rule;
    }

    private ToolbarMenu CreateDropdown()
    {
        var toolbar = new ToolbarMenu();
        toolbar.AddToClassList("dropdown");
        switch (aggregation)
        {
            case AggregationType.Action:
                toolbar.text= "Aggregate by action";
                break;
            
            case AggregationType.Phase:
                toolbar.text= "Aggregate by phase";
                break;
            
            case AggregationType.Modality:
                toolbar.text= "Aggregate by modality";
                break;
        }
        toolbar.menu.AppendAction("Aggregate by modality", x =>
        {
            toolbar.text = "Aggregate by modality";
            this.aggregation = AggregationType.Modality;
            this.UpdatePanels();
        });
        toolbar.menu.AppendAction("Aggregate by action", x =>
        {
            toolbar.text = "Aggregate by action";
            this.aggregation = AggregationType.Action;
            this.UpdatePanels();
        });
        toolbar.menu.AppendAction("Aggregate by phase", x =>
        {
            toolbar.text = "Aggregate by phase";
            this.aggregation = AggregationType.Phase;
            this.UpdatePanels();
        });

        return toolbar;
    }

    private void CreateInferredActions(VisualElement ruleRow, InferredRule rule, bool similarityMode = false)
    {
        var thenGroup = new VisualElement();
        thenGroup.AddToClassList("then");
        ruleRow.Add(thenGroup);
        
        VisualElement modality = CreateModalityList(rule);
        
        thenGroup.Add(modality);
        //thenGroup.Add(CreateThen());
        
        /*var thenGroup2 = new VisualElement();
        thenGroup2.AddToClassList("then");
         ruleRow.Add(thenGroup2);*/
        VisualElement modality2 = new VisualElement();
        modality2.AddToClassList("modality");
        Label modality2Head = new Label();
        modality2Head.AddToClassList("action");
        if(rule.interaction != null) modality2Head.text = Utils.CutModalityString(rule.interaction).Item2;
        modality2Head.AddToClassList("prefix");
        modality2.Add(modality2Head);
        thenGroup.Add(modality2);
        
        CreateRuleButtons(rule, modality);

        if (rule.actions == null)
        {
            CreateActionCell(thenGroup, null, rule.gameObject, rule, similarityMode);
        }
        else
        {

            Foldout similar = new Foldout();
            similar.text = $"Other similar rules for ${rule.gameObject.name}";
            for (int j = 0; j < rule.actions.Count; j++)
            {
                var act = rule.actions[j];
                CreateActionCell(thenGroup, act, rule.gameObject, rule, similarityMode);
            }
        }
    }

    private void CreateRuleButtons(InferredRule rule, VisualElement parent)
    {
        var changeBtn = new Button();
        changeBtn.tooltip = "Change the rule trigger...";
        changeBtn.clickable.clicked += () =>
        {
            ChangeTrigger ct = new ChangeTrigger();
            ct.Rule = rule;
            ct.editor = this;
            ct.ShowModalUtility();
        };

        
        changeBtn.AddToClassList("change-btn");
        parent.Add(changeBtn);

        
        
        var copyBtn = new Button();
        copyBtn.tooltip = "Copy the rule!";
        copyBtn.clickable.clicked += () =>
        {
            CopyPasteManager.GetInstance().Copy(rule);
            Debug.Log("Copying rule " + rule);
        };

        
        copyBtn.AddToClassList("copy-btn");
        parent.Add(copyBtn);
        
        
        
    }
    
    private void CreateActionCell(
        VisualElement thenRow, 
        InferredAction action, 
        GameObject gameObject, 
        InferredRule rule,
        bool similarityMode)
    {

        var actionGroup = new VisualElement();
        actionGroup.AddToClassList("action-group");
        actionGroup.AddToClassList("indent");
        thenRow.Add(actionGroup);
        
        var actionLabel = new Label();
        if (action?.obj == null)
        {
            actionLabel.text = action == null ? "No Action" : action.action;
        }
        else
        {
            var argument = action.argument == null ? "" : $"with {action.argument}";
            // [davide] please do not remove the following line, I use it for debugging 
            var distance = ""; //action.distance < Double.MaxValue ? $"(distance: {action.distance})" : "";

            actionLabel.text =$"{action.obj.name} {action.action} {argument} {distance}";
        }

        
        var actionEvent = new Label();
        actionEvent.AddToClassList("actionEvent");
        

        if (action.eventPath != null)
        {
            Type t = null;
            if (action.eventPath.Length == 1)
            {
                t = Type.GetType(action.componentAssemblyName);
            }

            if (action.eventPath.Length > 1)
            {
                t = Type.GetType(action.eventPath[action.eventPath.Length - 2].type);
            }
            if (t != null)
            {
                actionEvent.text = $"({t.Name}.{action.eventPath[action.eventPath.Length -1].name})";
            }
        }
        
       
        
        actionLabel.AddToClassList("action");

        var spotBtn = new Button();
        spotBtn.tooltip = "Show me the definition!";
        spotBtn.AddToClassList("spotlight");
        
        var findButton = new Button();
        findButton.AddToClassList("findButton");
        findButton.tooltip = "Find similar actions";
        
        if (action?.method == null)
        {
            spotBtn.style.visibility = Visibility.Hidden;
            findButton.style.visibility = Visibility.Hidden;
        }
        else
        {
            spotBtn.clicked += () =>
            {
                if (highlighting)
                {
                    // reset highlighting
                    Highlighter.Stop();
                    this.highlighting = false;
                    spotBtn.RemoveFromClassList("active-btn");
                }
                else
                {
                    // [davide] please do not remove the following two lines, I use them from time to time for debugging purposes
                    //var so = new SerializedObject(gameObject.GetComponent<ObjectManipulator>());
                    //var prt = so.FindProperty(action.inspector);

                    //Select the object in the scene view (useful when multiple objects are selected)
                    Selection.activeObject = gameObject;
                    var found =  Highlighter.Highlight("Inspector", 
                        $"{action.inspector}.m_PersistentCalls.m_Calls.Array.data[{action.index}].m_MethodName",
                        HighlightSearchMode.Identifier);
                    if (found)
                    {
                        this.highlighting = true;
                        spotBtn.AddToClassList("active-btn");
                    }
                }
            };
            
            findButton.clicked += () =>
            {
                FindSimilarActions(action, rule);
            };
        }

        var deleteBtn = new Button();
        deleteBtn.tooltip = "Delete this action";
        deleteBtn.AddToClassList("delete");
        deleteBtn.clicked += () =>
        {
            DeleteAction(action, gameObject);
            this.UpdatePanels();
        };

        if (!similarityMode)
        {
            actionGroup.Add(actionLabel);
        }
        else
        {
            var similarityGroup = new VisualElement();
            similarityGroup.AddToClassList("action");
            actionLabel.RemoveFromClassList("action");
            actionLabel.AddToClassList("similarityActionLabel");
            similarityGroup.Add(actionLabel);
            var similarityBar = new ProgressBar();
            similarityBar.AddToClassList("progressBar");
            var threshold = Math.Sqrt(2 * maxDistance * maxDistance);
            similarityBar.value = Convert.ToSingle((threshold - action.distance) / threshold) * 100;
            similarityBar.title = similarityBar.value.ToString("F0") + "% similar";
            similarityGroup.Add(similarityBar);
            actionGroup.Add(similarityGroup);
        }
        
        actionGroup.Add(actionEvent);
        actionGroup.Add(deleteBtn);
        actionGroup.Add(findButton);
        actionGroup.Add(spotBtn); 
        
        
    }

    private void DeleteAction(InferredAction action, GameObject gameObject)
    {
        var co = new SerializedObject(gameObject.GetComponent(Type.GetType(action.componentAssemblyName)));
        var coPtr = co.FindProperty($"{action.inspector}.m_PersistentCalls.m_Calls.Array");
        coPtr.DeleteArrayElementAtIndex(action.index);
        co.ApplyModifiedProperties();
    }

    private void FindSimilarActions(InferredAction action, InferredRule rule)
    {
        var results = new List<InferredRuleGroup>();
        
        
        var interactions=CopyPasteManager.GetInstance().FindInteractionInToolkit(action, 
            CopyPasteManager.GetInstance().GetMapping4Toolkit(Utils.toolkit), rule.modalities, rule.trigger);
        var replacements = CopyPasteManager.GetInstance()
            .FindReplacementCandidates(action.objectPath, Type.GetType(action.scriptName));
        var similarity = new List<InferredAction>();
        
        var interactableGameObjects = engine.FindInteractableObjects();
        
        //per ogni gameobject
        foreach (var interactable in interactableGameObjects)
        {
            //per ogni regola
            foreach (var interactableRule in engine.InferRuleByGameObject(interactable).Item1 )
            {
                var sameObject = rule.gameObject.Equals(interactableRule.gameObject) ? 0.0 : 1.0;
                //per ogni azione
                foreach (var interactableAction in interactableRule.actions)
                {
                    double[] distance = {maxDistance, maxDistance};
                    //regole possibili
                    for (int i = 0; i < interactions.Count; i++)
                    {
                        //verifichiamo se tra le regole trovate una sia strutturalmente uguale a quelle possibili
                        if (interactions[i].eventPath.Length == interactableAction.eventPath.Length)
                        {
                            for(int j=0;j<interactions[i].eventPath.Length;j++)
                            {
                                if (interactions[i].eventPath[j].name.Equals(interactableAction.eventPath[j].name))
                                {
                                    
                                    distance[0] = interactions[i].distance + sameObject * 3;
                                }
                            }
                            
                        }
                    }
                    
                    for(int i = 0; i < replacements.Count; i++)
                    {
                        if (Utils.GetGameObjectPath(replacements[i].obj).Equals(interactableAction.objectPath))
                        {
                            var sameAction = action.method.Equals(interactableAction.method) ? 1.0 : 3.0;
                            distance[1] = sameAction * replacements[i].distance;
                        }
                    }

                    interactableAction.distance = Math.Sqrt(distance[0]*distance[0] + distance[1]*distance[1]);
                    var threshold = Math.Sqrt(2 * maxDistance * maxDistance);
                    
                    // we filter interactions that are too far
                    if (interactableAction.distance < threshold )
                    {
                        
                        similarity.Add(interactableAction);
                        
                        var res = results.Find( x => x.title.Equals(interactableRule.gameObject.name));
                        if (res == null)
                        {
                            InferredRuleGroup group = new InferredRuleGroup();
                            group.title = interactableRule.gameObject.name;
                            group.rules.Add(interactableRule);
                            results.Add(group);
                        }
                        else
                        {
                            if (!res.rules.Contains(interactableRule))
                            {
                                res.rules.Add(interactableRule);
                            } 
                        }
                    }
                }
            }
        }

        foreach (var g in results)
        {
            foreach (var r in g.rules)
            {
                r.actions.Sort((x,y) => x.distance.CompareTo(y.distance));
            }
            
            g.rules.Sort((x,y) => x.actions[0].distance.CompareTo(y.actions[0].distance));
        }
        
        results.Sort((x,y) => 
            x.rules[0].actions[0].distance.CompareTo(y.rules[0].actions[0].distance));
        //result.Sort((x, y) => x.distance.CompareTo(y.distance));
        
        panelContent.Clear();
        var ruleViz = CreateInferredRules(results, 0, true);
        panelContent.Add(ruleViz);
        //UpdatePanels();
    }

    private VisualElement CreatePhaseIcon(Phases phase)
    {
        var icon = new Image();
        icon.AddToClassList("phase-icon");
        switch (phase)
        { 
            case Phases.Left: icon.AddToClassList("left");
                break;
            case Phases.Entered: icon.AddToClassList("pointed");
                break;
            case Phases.Selected: icon.AddToClassList("selected");
                break;
            case Phases.Moved: icon.AddToClassList("dragged");
                break;
            case Phases.Released: icon.AddToClassList("released");
                break;

        }
        return icon;
    }

    
    public static string Label4ModalityPhase(ModalitiesEnum m, Phases p)
    {
        switch (m)
        {
            case ModalitiesEnum.gaze:
                switch (p)
                {
                    case Phases.Entered:
                        return "gaze-pointed";
                    case Phases.Selected:
                        return "air-tapped";
                    case Phases.Moved:
                        return "gaze-dragged";
                    case Phases.Released:
                        return "air-tap released";
                    case Phases.Left:
                        return "gaze-unpointed";
                    default:
                        return "none";
                }
                
            case ModalitiesEnum.touch:
                switch (p)
                {
                    case Phases.Entered:
                        return "finger-pointed";
                    case Phases.Selected:
                        return "touched";
                    case Phases.Moved:
                        return "dragged";
                    case Phases.Released:
                        return "untouched";
                    case Phases.Left:
                        return "finger-unpointed";
                    default:
                        return "none";
                }
                    
            case ModalitiesEnum.grab:
                switch (p)
                {
                    case Phases.Entered:
                        return "approached";
                    case Phases.Selected:
                        return "grabbed";
                    case Phases.Moved:
                        return "near-moved";
                    case Phases.Released:
                        return "ungrabbed";
                    case Phases.Left:
                        return "left";
                    default:
                        return "none";
                }
                
            case ModalitiesEnum.hand:
                switch (p)
                {
                    case Phases.Entered:
                        return "hovered";
                    case Phases.Selected:
                        return "clicked";
                    case Phases.Moved:
                        return "far-moved";
                    case Phases.Released:
                        return "uncliked";
                    case Phases.Left:
                        return "left";
                    default:
                        return "none";
                }
                
            case ModalitiesEnum.remote:
                switch (p)
                {
                    case Phases.Entered:
                        return "focused";
                    case Phases.Selected:
                        return "triggered";
                    case Phases.Moved:
                        return "hold";
                    case Phases.Released:
                        return "trigger-released";
                    case Phases.Left:
                        return "unfocused";
                    default:
                        return "none";
                }


        }

        return "none";
    }
    
    private string label4Phase(Phases p)
    {
        switch (p)
        {
            case Phases.Left: return "Left";
            case Phases.Entered: return "Entered";
            case Phases.Selected: return "Selected";
            case Phases.Moved: return "Moved";
            case Phases.Released: return "Released";
        }

        return null;
    }

    private  VisualElement CreatePhaseCell(string phaseName)
    {
        var phase = new VisualElement();
        phase.AddToClassList("phase");
        var whenLabel = new Label("When");
        whenLabel.AddToClassList("when");
        phase.Add(whenLabel);
        var phaseLabel = new Label(phaseName);
        phaseLabel.AddToClassList("phase-label");
        phase.Add(phaseLabel);
        return phase;
    }

    private void CreateModalAddingComponent()
    {
        var modal = new VisualElement();
        modal.AddToClassList("modal");
        Label label = new Label("It seems that the component you're trying to copy is missing. Do you want to add it?");
        modal.Add(label);
        Button pasteBtn = rootVisualElement.Q<VisualElement>("paste-button") as Button;
        Button yesBtn = new Button(() =>
        {
            // add the component
        });
        yesBtn.text = "Yes";
        Button noBtn = new Button(() =>
        {
            // do nothing
            pasteBtn.SetEnabled(true);
            modal.visible = false;
        });
        noBtn.text = "No";
        modal.Add(yesBtn);
        modal.Add(noBtn);

        // TODO [davide] commented since we pass to a list of selected objects
        //objInspection.Add(modal);
        
        modal.PlaceBehind( pasteBtn);
        pasteBtn.SetEnabled(false);
    }

    internal enum AggregationType
    {
        Modality, Action, Phase
    }
}