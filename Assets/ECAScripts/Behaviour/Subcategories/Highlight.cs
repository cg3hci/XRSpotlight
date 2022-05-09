using System;
using UnityEngine;
using EcaRules;
using ECAScripts;
using ECAScripts.Utils;
using UnityEngine.UI;
using EcaRules.Visualization;
using Behaviour = EcaRules.Behaviour;

/// <summary>
/// <b>Highlight</b> is a <see cref="Behaviour">Behaviour</see> that is used to highlight the objects that are in the scene.
/// </summary>
[ECARules4All("highlight")]
[RequireComponent(typeof(Behaviour))]
[DisallowMultipleComponent]
public class Highlight : MonoBehaviour
{
    /// <summary>
    /// <b>Color</b> is the color that will be used to highlight the objects.
    /// </summary>
    [StateVariable("color", ECARules4AllType.Color)]
    public Color color;
    /// <summary>
    /// <b>On</b> is a boolean that tells if the highlight is on or off.
    /// </summary>
    [StateVariable("on", ECARules4AllType.Boolean)]
    public ECABoolean on = new ECABoolean(ECABoolean.BoolType.OFF);

    /// <summary>
    /// <b>Outline</b> is an object that is used to highlight the object.
    /// </summary>
    private ECAOutline outline;

    /// <summary>
    /// <b>Start</b> applies the outline to the object.
    /// </summary>
    private void Start()
    {
        outline = gameObject.AddComponent<ECAOutline>();
        outline.OutlineColor = color;
        if (on)
        {
            outline.OutlineWidth = 5f;
        }
        else
        {
            outline.OutlineWidth = 0f;
        }    
    }
    
    /// <summary>
    /// <b>ChangesColor</b> changes the color of the outline.
    /// </summary>
    /// <param name="c"></param>
    [Action(typeof(Highlight), "changes", "color", "to", typeof(Color))]
    public void ChangesColor(Color c)
    {
        color = c;
        outline.OutlineColor = color;
    }

    /// <summary>
    /// <b>TurnsOn</b> turns the highlight on or off.
    /// </summary>
    /// <param name="on"></param>
    [Action(typeof(Highlight), "turns", typeof(ECABoolean))]
    public void Turns(ECABoolean on)
    {
        this.on = on;
        if (on)
        {
            outline.OutlineWidth = 5f;
        }
        else
        {
            outline.OutlineWidth = 0f;
        }
    }
}