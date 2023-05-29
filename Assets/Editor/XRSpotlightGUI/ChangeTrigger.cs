using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using XRSpotlightGUI;

public class ChangeTrigger : EditorWindow
{
    public InferredRule Rule { get; set; }
    public Phases Selected { get; internal set; }
    
    public RuleEditor editor { get; set; }

    public ChangeTrigger()
    {
        Selected = Phases.None;
    }

    public void CreateGUI()
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/XRSpotlightGUI/RuleEditor.uss");

        VisualElement panel = rootVisualElement;
        ScrollView root = new ScrollView();
        panel.styleSheets.Add(styleSheet);

        panel.Add(root);

        Label hello = new Label("Change the Rule Trigger");
        hello.AddToClassList("rule-title");
        panel.Add(hello);

        Label change = new Label("Select new trigger for the rule");

        
        foreach (Phases p in Enum.GetValues(typeof(Phases)))
        {
            if (Rule == null  || p == Phases.None || p == Rule.trigger)
                continue;

            string l = "";
            foreach (ModalitiesEnum m in Enum.GetValues(typeof(ModalitiesEnum)))
            {
                if (!Rule.modalities.GetModality(m))
                    continue;
                
                if (String.IsNullOrEmpty(l))
                {
                    l = RuleEditor.Label4ModalityPhase(m, p);
                }
                else
                {
                    l += $"/ {RuleEditor.Label4ModalityPhase(m, p)}";
                }
            }
 
            Button phaseButton = new Button();
            phaseButton.text = $"When {Rule.gameObject.name} is " + l;
            phaseButton.clicked += () =>
            {
                this.Selected = p;
                editor.ChangeTrigger(Rule, this.Selected);
                this.Close();
            };
            panel.Add(phaseButton);

        }

       
        Button cancel = new Button();
        cancel.text = "Cancel";
        cancel.AddToClassList("spacer-big");
        cancel.clicked += () =>
        {
            this.Close();
        };
        panel.Add(cancel);

    }
}
