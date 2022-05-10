using System;
using UnityEngine;
using EcaRules;
using ECAScripts.Utils;

/// <summary>
/// <b>Switch</b> is a <see cref="EcaBehaviour"/> that can be use to let an object have an on/off state, useful for
/// objects like lights, doors, etc.
/// </summary>
[EcaRules4All("switch")]
[RequireComponent(typeof(EcaBehaviour))]
[DisallowMultipleComponent]
public class EcaSwitch : MonoBehaviour
{
    /// <summary>
    /// <b>On</b> is the state of the switch.
    /// </summary>
    [EcaStateVariable("on", EcaRules4AllType.Boolean)]
    public ECABoolean on;

    /// <summary>
    /// <b>Turns</b> defines if the switch is on or off.
    /// </summary>
    /// <param name="on">The new state of the switch.</param>
    [EcaAction(typeof(EcaSwitch), "turns", typeof(ECABoolean))]
    public void Turns(ECABoolean on)
    {
        this.on = on;
    }
}