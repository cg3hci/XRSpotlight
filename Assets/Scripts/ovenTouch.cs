using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ovenTouch : MonoBehaviour
{
    public GameObject pan;
    public bool panInOven;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.name == "Pan")
        {
            panInOven = true;
            pan = collision.gameObject;
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.transform.name == "Pan")
        {
            panInOven = false;
            pan = null;
        }
    }
}
