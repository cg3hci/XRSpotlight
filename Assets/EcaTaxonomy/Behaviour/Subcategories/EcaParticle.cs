using System;
using UnityEngine;
using EcaRules;
using ECAScripts.Utils;

/// <summary>
/// <b>Particle</b> is a <see cref="EcaBehaviour"/> that lets the object emit particles.
/// </summary>
[EcaRules4All("particle")]
[RequireComponent(typeof(EcaBehaviour))]
[DisallowMultipleComponent]
public class EcaParticle : MonoBehaviour
{
    /// <summary>
    /// <b>On</b> is a boolean that indicates if the particle system is active.
    /// </summary>
    [EcaStateVariable("on", EcaRules4AllType.Boolean)]
    public ECABoolean on = new ECABoolean(ECABoolean.BoolType.OFF);

    public ParticleSystem ps;

    //TODO: Questa azione VA in conflitto con azioni all'interno di ECAObject (ad es. Turns di Electronic)
    /// <summary>
    /// <b>Turns</b> is used to turn on/off the particle system.
    /// </summary>
    /// <param name="on">The status of the particle system.</param>
    [EcaAction(typeof(EcaParticle), "turns", typeof(ECABoolean))]
    public void Turns(ECABoolean on)
    {
        this.@on = on;

        if (on && ps != null)
        {
            ps.Stop();
            ps.Play();
        }
        else if (!(on) && ps != null)
        {
            ps.Stop();
        }
    }

    /// <summary>
    /// <b>Start</b> initializes the particle system and starts it if the <see cref="on"/> variable is set to true.
    /// </summary>
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            if (on)
                ps.Play();
            else
                ps.Stop();
        }
    }
}