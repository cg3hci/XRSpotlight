using System;
using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;

/// <summary>
/// <b>Bounds</b> is an <see cref="Interaction"/> subclass that represents the scene bounds.
/// </summary>
[ECARules4All("bounds")]
[RequireComponent(typeof(Interaction))] //gerarchia 
[DisallowMultipleComponent]
public class Bounds : MonoBehaviour
{
    /// <summary>
    /// <b>Scale</b> is the scale of the scene bounds.
    /// </summary>
    [StateVariable("scale", ECARules4AllType.Float)] public float scale;

    /// <summary>
    /// <b>Scales</b> sets the scale of the scene bounds.
    /// </summary>
    /// <param name="newScale">The new scale value.</param>
    [Action(typeof(Bounds), "scales-to", typeof(float))]
    public void Scales(float newScale)
    {
        scale = newScale;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void Start()
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
