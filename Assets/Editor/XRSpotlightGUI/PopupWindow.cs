using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PopupWindow : EditorWindow
{
    
    //public string selected = Selection.activeObject.name;
    public List<string> selectionOptions;

    [MenuItem("Window/UI Toolkit/Popup")]
    public static void PopupWindowShow(List<string> selectionOptions)
    {
        PopupWindow window =CreateInstance(typeof(PopupWindow)) as PopupWindow;
        window.selectionOptions = selectionOptions;
        window.ShowModalUtility();
        window.maximized = true;
        window.titleContent = new GUIContent("Choose a candidate");
        
    }
    
    private void CreateGUI()
    {
        /*EditorGUILayout.LabelField("We have found many candidates for :", Selection.activeGameObject.name);*/

        VisualElement panel = rootVisualElement;

        // Create a new field and assign it its value.
        var normalField = new PopupField<string>("Candidates:", selectionOptions, 0);
        normalField.value = selectionOptions[0];
        panel.Add(normalField);

        Button button = new Button(() =>
        {
            Close();
        });
        
        // Mirror value of uxml field into the C# field.
        normalField.RegisterCallback<ChangeEvent<string>>((evt) =>
        {
            button.text = "Choose " + evt.newValue;
        });

        
        button.text = "Choose " + normalField.value;
        
        panel.Add(button);
    }

    /*void OnInspectorUpdate()
    {
        Repaint();
    }*/
    
    
    
}