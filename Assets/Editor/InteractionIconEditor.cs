using System;
using EcaRules.Visualization;
using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( InteractionIcon ) )]
public class InteractionIconEditor : Editor
{
    // draw lines between a chosen game object
    // and a selection of added game objects
    private SerializedProperty ShowInteractions;
    
    void OnEnable()
    {
        ShowInteractions = serializedObject.FindProperty("ShowInteractions");
    }
    
    void OnSceneGUI()
    {
        // get the chosen game object
        
        InteractionIcon t = target as InteractionIcon;

        if( t == null || t.GameObjects == null )
            return;
        serializedObject.Update();
        EditorGUILayout.PropertyField(ShowInteractions);
        if (t.ShowInteractions)
        {
            OutlineObjects(t);
        }else DeOutlineObjects(t);
        serializedObject.Update();

        serializedObject.ApplyModifiedProperties();
    }
    
   /* public override void OnInspectorGUI()
    {
        InteractionIcon t = target as InteractionIcon;

        if( t == null || t.GameObjects == null )
            return;
        serializedObject.Update();
        EditorGUILayout.PropertyField(ShowInteractions);
        if (t.ShowInteractions)
        {
            OutlineObjects(t);
        }
        else DeOutlineObjects(t);
        serializedObject.Update();

        serializedObject.ApplyModifiedProperties();
    }*/

    static void DeOutlineObjects(InteractionIcon interactionIcon)
    {
        foreach (var t in interactionIcon.GameObjects)
        {
            if (t != null)
            {
                DestroyImmediate(t.GetComponent<ECAOutline>());
            }
        }
    }

    static void OutlineObjects( InteractionIcon interactionIcon)
    {
        // iterate over game objects added to the array...
        foreach (var t in interactionIcon.GameObjects)
        {
            // ... and draw an outline
            if (t != null)
            {
                OutlineColor(t, Color.red);
            }
        }
    }
    
    public static void OutlineColor(GameObject gameObject, Color color)
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
}
