using System;
using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;

/// <summary>
/// <b>Bounds</b> is an <see cref="EcaInteraction"/> subclass that represents the scene bounds.
/// </summary>
[EcaRules4All("bounds")]
[RequireComponent(typeof(EcaInteraction))] //gerarchia 
[DisallowMultipleComponent]
public class EcaBounds : MonoBehaviour
{
    /// <summary>
    /// <b>Scale</b> is the scale of the scene bounds.
    /// </summary>
    [EcaStateVariable("scale", EcaRules4AllType.Float)] public float scale;

    /// <summary>
    /// <b>Scales</b> sets the scale of the scene bounds.
    /// </summary>
    /// <param name="newScale">The new scale value.</param>
    [EcaAction(typeof(EcaBounds), "scales-to", typeof(float))]
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
