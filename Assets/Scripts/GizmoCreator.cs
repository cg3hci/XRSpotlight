using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GizmoCreator : MonoBehaviour
{
    //This class may be or may be not be used in a future version of the project.
    //It is used to create a gizmo in the Scene view on user multiple selection or given a "gizmos" list.
    //Add this script to a GameObject and then add the gizmo to the "gizmos" list in the inspector.
    //it also needs adjustments for selections greater than 9.
    int value = 1;
    private int currentSelection = 0;
    private bool update = false;
    
    public GameObject[] gizmos;

    private void OnDrawGizmos()
    {
        if (Selection.activeGameObject == null)
        {
            Gizmos.DrawSphere(transform.position, 0f);
            value = 0;
        }

        if (gizmos.Length > 0)
        {
            value = 0;
            foreach (var elem in gizmos)
            {
                Gizmos.DrawIcon(elem.transform.position, numberToIcon(value), true);
                value++;
            }
        }
        else if (Selection.objects.Length > 1)
        {
            if (currentSelection != Selection.objects.Length)
            {
                update = true;
                currentSelection = Selection.objects.Length;
            }

            if (update)
            {
                update = false;
                value = 0;
                foreach (var obj in Selection.objects)
                {
                    if (obj is GameObject o)
                    {
                        Gizmos.DrawIcon(o.transform.position, numberToIcon(value), true);
                        value++;
                    }
                }
            }
        }
        else Gizmos.DrawIcon(Selection.activeGameObject.transform.position, numberToIcon(value), true);
    }

    private String numberToIcon(int value)
    {
        //It needs files inside the "Gizmos" folder (Assets/Gizmos).
        //If this feature will be used, make sure that the files are inside the folder.
        switch (value)
        {
            case 0:
                return "0.png";
            case 1:
                return "1.png";
            case 2:
                return "2.png";
            case 3:
                return "3.png";
            case 4:
                return "4.png";
            case 5:
                return "5.png";
            case 6:
                return "6.png";
            case 7:
                return "7.png";
            case 8:
                return "8.png";
            case 9:
                return "9.png";
            default:
                return "0.png";
        }

        {
        }
    }
}