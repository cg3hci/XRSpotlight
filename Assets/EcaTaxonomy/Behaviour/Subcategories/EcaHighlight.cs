using System;
using UnityEngine;
using EcaRules;
using ECAScripts;
using ECAScripts.Utils;
using UnityEngine.UI;
using EcaRules.Visualization;

/// <summary>
/// <b>Highlight</b> is a <see cref="EcaBehaviour">Behaviour</see> that is used to highlight the objects that are in the scene.
/// </summary>
[EcaRules4All("highlight")]
[RequireComponent(typeof(EcaBehaviour))]
[DisallowMultipleComponent]
public class EcaHighlight : MonoBehaviour
{
    /// <summary>
    /// <b>Color</b> is the color that will be used to highlight the objects.
    /// </summary>
    [EcaStateVariable("color", EcaRules4AllType.Color)]
    public Color color;
    /// <summary>
    /// <b>On</b> is a boolean that tells if the highlight is on or off.
    /// </summary>
    [EcaStateVariable("on", EcaRules4AllType.Boolean)]
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
    [EcaAction(typeof(EcaHighlight), "changes", "color", "to", typeof(Color))]
    public void ChangesColor(Color c)
    {
        color = c;
        outline.OutlineColor = color;
    }

    /// <summary>
    /// <b>TurnsOn</b> turns the highlight on or off.
    /// </summary>
    /// <param name="on"></param>
    [EcaAction(typeof(EcaHighlight), "turns", typeof(ECABoolean))]
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