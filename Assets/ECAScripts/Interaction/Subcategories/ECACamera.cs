using System;
using System.Collections;
using System.Collections.Generic;
using EcaRules;
using ECAScripts.Utils;
using UnityEngine;

/// <summary>
/// <b>ECACamera</b> is an <see cref="Interaction"/> subclass that allows the user to interact with the camera the
/// script is attached to.
/// </summary>
[ECARules4All("camera")]
[RequireComponent(typeof(Interaction), typeof(Camera))] //gerarchia 
[DisallowMultipleComponent]
public class ECACamera : MonoBehaviour
{
    /// <summary>
    /// <b>Camera</b> is the <see cref="Camera"/> component the script will control.
    /// </summary>
    private Camera camera;
    //In vr la 3a persona è una telecamera dall'alto fissa
    //TODO: Nel parser non ci sono regole che hanno un parametro per modificare zoom in/out
    /// <summary>
    /// <b>POV</b> is the camera's point of view.
    /// <p>Possible values:</p>
    /// <list type="bullet">
    ///<item>
    ///<term>First</term>
    ///</item>
    ///<item>
    ///<term>Third</term>
    ///</item>
    /// </list>
    /// </summary>
    public enum POV{First, Third};
    /// <summary>
    /// <b>POV</b> is the camera's point of view.
    /// </summary>
    [StateVariable("pov", ECARules4AllType.Identifier)] public POV pov;
    /// <summary>
    /// <b>zoomLevel</b> is the camera's zoom level.
    /// </summary>
    [StateVariable("zoomLevel", ECARules4AllType.Float)] public float zoomLevel = 60;
    /// <summary>
    /// <b>Playing</b> is a boolean that indicates whether the camera is currently playing.
    /// </summary>
    [StateVariable("playing", ECARules4AllType.Boolean)] public ECABoolean playing;
    
    //TODO: conflict between grammar and documentation, float parameter only in documentation
    /// <summary>
    /// <b>ZoomsIn</b> reduces the camera's zoom level by the specified amount.
    /// <p>If the resulting zoom is less than 30 the zoom is set to 30.</p>
    /// </summary>
    /// <param name="amount">The amount of zoom to remove </param>
    [Action(typeof(ECACamera), "zooms-in", typeof(float))]
    public void ZoomsIn(float amount)
    {
        if(zoomLevel - amount > 30)
        {
            zoomLevel -= amount;
        }
        else
        {
            zoomLevel = 30;
        }
        camera.fieldOfView = zoomLevel;
    }
    //TODO: conflict between grammar and documentation, float parameter only in documentation
    /// <summary>
    /// <b>ZoomsOut</b> increases the camera's zoom level by the specified amount.
    /// <p> If the resulting zoom is greater than 100 the zoom is set to 100.</p>
    /// </summary>
    /// <param name="amount">The amount of zoom to add</param>
    [Action(typeof(ECACamera), "zooms-out", typeof(float))]
    public void ZoomsOut(float amount)
    {
        if(zoomLevel + amount < 100)
        {
            zoomLevel += amount;
        }
        else
        {
            zoomLevel = 100;
        }
        camera.fieldOfView = zoomLevel;
    }
    
    //TODO: conflict between grammar and documentation, float parameter only in documentation
    /// <summary>
    /// <b> ChangesPov</b> changes the camera's point of view.
    /// </summary>
    /// <param name="pov">The new <see cref="POV"/> value. </param>
    [Action(typeof(ECACamera), "changes", "POV", "to", typeof(POV))]
    public void ChangesPOV(POV pov)
    {
        this.pov = pov;
    }
    
    private void Start()
    {
        camera = GetComponent<Camera>();
        if (zoomLevel > 100) zoomLevel = 100;
        else if (zoomLevel < 30) zoomLevel = 30;
        camera.fieldOfView = zoomLevel;
    }
}
