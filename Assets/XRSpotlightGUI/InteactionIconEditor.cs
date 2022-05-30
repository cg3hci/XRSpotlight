using System;
using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( InteractionIcon ) )]
public class InteactionIconEditor : Editor
{
    // draw lines between a chosen game object
    // and a selection of added game objects

    void OnSceneGUI()
    {
        // get the chosen game object
        InteractionIcon t = target as InteractionIcon;

        if( t == null || t.GameObjects == null )
            return;

        // grab the center of the parent
        Vector3 center = t.transform.position;

        // iterate over game objects added to the array...
        for( int i = 0; i < t.GameObjects.Length; i++ )
        {
            // ... and draw a line between them
            if (t.GameObjects[i] != null)
            {
                Bounds bounds; 
                var r = t.GameObjects[i]. GetComponent<Renderer>();
                if (r != null)
                {
                    bounds = r.bounds;
                }
                else
                {
                    var c = t.GameObjects[i]. GetComponent<Collider>();
                    bounds = c.bounds;
                }
                
                Handles.color = Color.green;
                Handles.DrawWireCube(bounds.center, bounds.extents * 2);
            }
               
               
        }
    }
}
