using System;
using EcaRules;
using ECAScripts.Utils;
using UnityEngine;

/// <summary>
/// <b>Electronic</b> class is used to create and manage electronics objects, which are used to interact with the game.
/// </summary>
[EcaRules4All("electronic")]
[RequireComponent(typeof(EcaProp))]
[DisallowMultipleComponent]
public class EcaElectronic : MonoBehaviour
{
    /// <summary>
    /// <b>Brand</b> is the brand of the electronic.
    /// </summary>
    [EcaStateVariable("brand", EcaRules4AllType.Text)]
    public string brand;

    /// <summary>
    /// <b>Model</b> is the model of the electronic.
    /// </summary>
    [EcaStateVariable("model", EcaRules4AllType.Text)]
    public string model;

    /// <summary>
    /// <b>On</b> is the state of the electronic.
    /// </summary>
    [EcaStateVariable("on", EcaRules4AllType.Boolean)]
    public ECABoolean on = new ECABoolean(ECABoolean.BoolType.OFF);
    
    /// <summary>
    /// <b>TurnParticle</b>: The particle system to play when the electronic object is turned on.
    /// </summary>
    public ParticleSystem turnParticle;
    private GameObject particlePrefab;

    /// <summary>
    /// <b>Turns</b>: Turns the electronic on or off.
    /// </summary>
    /// <param name="on">A boolean for the new state of the electronic</param>
    [EcaAction(typeof(EcaElectronic), "turns", typeof(ECABoolean))]
    public void Turns(ECABoolean on)
    {
        this.@on = on;

        if (on && turnParticle)
        {
            turnParticle.Stop();
            turnParticle.Play();
        }
        else if (!on && turnParticle)
        {
            turnParticle.Stop();
        }
    }

    private void Start()
    {
        if (turnParticle == null)
        {
            particlePrefab = Instantiate(Resources.Load("Particles/Particle_Turns"), transform) as GameObject;
            turnParticle = particlePrefab.GetComponent<ParticleSystem>();
        }
        else
        {
            Instantiate(turnParticle, transform);
            turnParticle = transform.Find(turnParticle.name + "(Clone)").GetComponent<ParticleSystem>();
        }


        if (on)
        {
            turnParticle.Play();
        }
        else
        {
            turnParticle.Stop();
        }
    }
}