using System;
using UnityEngine;
using ECARules4All.RuleEngine;
using ECAScripts.Utils;
using Behaviour = ECARules4All.RuleEngine.Behaviour;

/// <summary>
/// <b>Particle</b> is a <see cref="Behaviour"/> that lets the object emit particles.
/// </summary>
[ECARules4All("particle")]
[RequireComponent(typeof(Behaviour))]
[DisallowMultipleComponent]
public class Particle : MonoBehaviour
{
    /// <summary>
    /// <b>On</b> is a boolean that indicates if the particle system is active.
    /// </summary>
    [StateVariable("on", ECARules4AllType.Boolean)]
    public ECABoolean on = new ECABoolean(ECABoolean.BoolType.OFF);

    public ParticleSystem ps;

    //TODO: Questa azione VA in conflitto con azioni all'interno di ECAObject (ad es. Turns di Electronic)
    /// <summary>
    /// <b>Turns</b> is used to turn on/off the particle system.
    /// </summary>
    /// <param name="on">The status of the particle system.</param>
    [Action(typeof(Particle), "turns", typeof(ECABoolean))]
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