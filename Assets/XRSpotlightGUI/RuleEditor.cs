using Microsoft.MixedReality.Toolkit.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class RuleEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/RuleEditor")]
    public static void ShowExample()
    {
        RuleEditor wnd = GetWindow<RuleEditor>();
        wnd.titleContent = new GUIContent("RuleEditor");
    }

    private Label activeLabel;

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/XRSpotlightGUI/RuleEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/XRSpotlightGUI/RuleEditor.uss");
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);
        
        Interactable[] interactables = GameObject.FindObjectsOfType<Interactable>();
        foreach (var interactable in interactables)
        {
            VisualElement interLabel = new Label(interactable.gameObject.name);
            root.Add(interLabel);
        }

       
        activeLabel = new Label("Active object: ");
        root.Add(activeLabel);

        Selection.selectionChanged += () =>
        {
            activeLabel.text = "Active object: " + Selection.activeObject.name;
        };
    }
}