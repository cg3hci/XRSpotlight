using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class YesOrNoGizmo : MonoBehaviour
{
    public bool correct;
    private Object toDestroy;
    private bool instantiated;

    public void showCorrectness()
    {
        if (!instantiated)
        {
            instantiated = true;
            if (correct)
            {
                toDestroy = Instantiate(Resources.Load("Yes"), transform.position + Vector3.up * 0.2f, Quaternion.identity);
            }
            else
            {
                toDestroy =  Instantiate(Resources.Load("No"), transform.position + Vector3.up * 0.2f, Quaternion.identity);
            }
        }
    }

    public void Update()
    {
        if (instantiated)
            ((GameObject) toDestroy).transform.position = transform.position + Vector3.up * 0.2f;
    }
    
    
    public void hideCorrectness()
    {
        if (instantiated)
        {
            Destroy(toDestroy);
            instantiated = false;
        }
    }
}
