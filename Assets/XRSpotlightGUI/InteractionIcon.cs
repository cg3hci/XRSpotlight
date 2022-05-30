using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

[ExecuteInEditMode]
public class InteractionIcon : MonoBehaviour
{
    // an array of game objects which will have a
    // line drawn to in the Scene editor
    public GameObject[] GameObjects;

    private void OnEnable()
    {
        Interactable[] interactables = GameObject.FindObjectsOfType<Interactable>();
        GameObjects =  new GameObject[interactables.Length];
        int c = 0;
        foreach (var i in interactables)
        {
            GameObjects[c] = i.gameObject;
            c++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
