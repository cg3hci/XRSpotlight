using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using XRSpotlightGUI;


public class RuleEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/RuleEditor")]

    public static void ShowExample()
    {
        RuleEditor wnd = GetWindow<RuleEditor>();
        wnd.titleContent = new GUIContent("RuleEditor");
    }

    private Label activeLabel;
    private Foldout interactableList;
    private bool listFold = true;
    private VisualElement objInspection;
    private bool showEmpty = false;
    private InferenceEngine engine;

    public void CreateGUI()
    {
        this.showEmpty = false;
        engine = new InferenceEngine(Toolkits.MRTK);
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/XRSpotlightGUI/RuleEditor.uss");
        // Each editor window contains a root VisualElement object
        VisualElement panel = rootVisualElement;
        ScrollView root = new ScrollView();
        panel.Add(root);
        interactableList = new Foldout();
        interactableList.text = "Interactable Objects";
        interactableList.AddToClassList("section");
        root.styleSheets.Add(styleSheet);

        root.Add(interactableList);
        CreateInteractableList(); 
        
        
        activeLabel = new Label("Active object: ");
        root.Add(activeLabel);
 
        Selection.selectionChanged += () => 
        {
            if (activeLabel != null && Selection.activeObject != null)
            {
                activeLabel.text = "Active object: " + Selection.activeObject.name;
                PopulateInspectionPanel(GameObject.Find(Selection.activeObject.name));
                
            }
        };
        
        // Import UXML
        //var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/XRSpotlightGUI/RuleEditor.uxml");
        //VisualElement labelFromUXML = visualTree.Instantiate();
        //root.Add(labelFromUXML);

        objInspection = new VisualElement();
        root.Add(objInspection);
        

    }

    public void OnHierarchyChange()
    {
        CreateInteractableList();
    }

    private void CreateInteractableList()
    {
        if (interactableList == null) return;
        interactableList.Clear();
        GameObject[] interactables = engine.FindInteractableObjects();
        foreach (var interactable in interactables)
        {
            VisualElement interLabel = new Label(interactable.name);
            interactableList.Add(interLabel);
        }
        
        // access to the toggle element (setting labels with icons)
        //Debug.Log(interactableList.hierarchy.ElementAt(0).GetType()); 
    }

    private void PopulateInspectionPanel(GameObject selected)
    {
        if (selected == null) return; 
        objInspection.Clear();
        objInspection.Add(CreateInspectionHeading(selected));

        var rules = EcaEventFinder.InferRuleByGameObject(selected);
        objInspection.Add(CreateInferredRules(rules));
    }
    
    private VisualElement CreateInspectionHeading(GameObject selected)
    {
        var heading = new Toolbar();
        var headingToggle = new Foldout();
        
        
        var headingLabel = new Label($"Interaction Rules for {selected.name}");
        heading.AddToClassList("obj-heading"); 
        
        heading.Add(headingToggle);
        heading.Add(headingLabel);

        return heading;

    }

    private VisualElement CreateModalityList(InferredRule rule)
    {
        var modality = new VisualElement();
        modality.AddToClassList("modality");
        
        var modalityHead = new Label($"When " + label4Phase(rule.trigger));
        modalityHead.AddToClassList("prefix");
        modality.Add(modalityHead);

        var gazeLabel = new Label(rule.modalities.gaze ? "gaze " : "");
        gazeLabel.AddToClassList("flex1");
        modality.Add(gazeLabel);
        
        var touchLabel = new Label(rule.modalities.touch ? "touch" : "");
        touchLabel.AddToClassList("flex1");
        modality.Add(touchLabel);
        
        var handLabel = new Label(rule.modalities.hand ? "hand" : "");
        handLabel.AddToClassList("flex1");
        modality.Add(handLabel);
        
        var remoteLabel = new Label(rule.modalities.remote ? "remote" : "");
        remoteLabel.AddToClassList("flex1");
        modality.Add(remoteLabel);
        
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
    
    

    private VisualElement CreateInferredRules(InferredRule[] rules)
    {
        VisualElement rule = new VisualElement();
        rule.AddToClassList("rule");

        
        
        
        foreach (var inferredRule in  rules)
        {
            var ruleRow = new VisualElement();
            ruleRow.AddToClassList("rule-row");
            rule.Add(ruleRow);
            ruleRow.Add(CreatePhaseIcon(inferredRule.trigger));
            CreateInferredActions(ruleRow, inferredRule);
        }

        return rule;
    }

    private void CreateInferredActions(VisualElement ruleRow, InferredRule rule)
    {
        var thenGroup = new VisualElement();
        thenGroup.AddToClassList("then");
        ruleRow.Add(thenGroup);
        
        thenGroup.Add(CreateModalityList(rule));
        //thenGroup.Add(CreateThen());

        if (rule.actions == null)
        {
            CreateActionCell(thenGroup, null);
        }
        else
        {
            foreach (var act in rule.actions)
            {
               CreateActionCell(thenGroup, act);
            }
        }
    }
    
    private void CreateActionCell(VisualElement thenRow, InferredAction action)
    {

        var actionGroup = new VisualElement();
        actionGroup.AddToClassList("action-group");
        actionGroup.AddToClassList("indent");
        thenRow.Add(actionGroup);
        
        var actionLabel = new Label();
        if (action == null)
        {
            actionLabel.text = "No Action";
        }
        else
        {
            actionLabel.text = $"the object {action.obj.name} {action.action}";
        }
       
        actionLabel.AddToClassList("action");

        var spotBtn = new Button();
        spotBtn.tooltip = "Show me the definition!";
        spotBtn.AddToClassList("spotlight");

        if (action == null)
        {
            spotBtn.style.visibility = Visibility.Hidden;
        }
        actionGroup.Add(actionLabel);
        actionGroup.Add(spotBtn); 
    }

    private VisualElement CreatePhaseIcon(Phases phase)
    {
        var icon = new Image();
        icon.AddToClassList("phase-icon");
        switch (phase)
        {
            case Phases.Addressed: icon.AddToClassList("pointed");
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

    private string label4Phase(Phases p)
    {
        switch (p)
        {
            case Phases.Addressed: return "Addressed";
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
}