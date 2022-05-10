using UnityEngine;
using EcaRules;

/// <summary>
/// The <b>EdgedWeapon</b> class is a <b>Weapon</b> that has a sharp edge.
/// </summary>
[EcaRules4All("edged-weapon")]
[RequireComponent(typeof(EcaWeapon))]
[DisallowMultipleComponent]
public class EcaEdgedWeapon : MonoBehaviour
{
    /// <summary>
    /// <b>ParticleStab</b>: The particle system to play when the weapon stabs.
    /// </summary>
    public ParticleSystem particleStab;
    /// <summary>
    /// <b>ParticleSlice</b>: The particle system to play when the weapon slices.
    /// </summary>
    public ParticleSystem particleSlice;
    private GameObject particleStabPrefab = null;
    private GameObject particleSlicePrefab = null;

    /// <summary>
    /// <b>Stabs</b>: The action that occurs when a player stabs another ECAObject.
    /// </summary>
    /// <param name="obj">The ECAObject that has been stabbed</param>
    [EcaAction(typeof(EcaEdgedWeapon), "stabs", typeof(ECAObject))]
    public void Stabs(ECAObject obj)
    {
        EcaCharacter c = obj.gameObject.GetComponent<EcaCharacter>();
        EcaWeapon w = this.gameObject.GetComponent<EcaWeapon>();
        if (c != null && w != null)
        {
            c.life -= w.power;
            if (c.life < 0)
                c.life = 0;
        }

        if (particleStab == null)
        {
            particleStabPrefab = Instantiate(Resources.Load("Particles/Particle_Stab"), transform) as GameObject;
            particleStab = particleStabPrefab.GetComponent<ParticleSystem>();
        }
        else
        {
            Instantiate(particleStab, transform);
            particleStab = transform.Find(particleStab.name + "(Clone)").GetComponent<ParticleSystem>();
        }

        particleStab.Stop();
        particleStab.Play();
    }

    /// <summary>
    /// <b>Stabs</b>: The action that occurs when a player slices another ECAObject.
    /// </summary>
    /// <param name="obj">The ECAObject that has been sliced</param>
    [EcaAction(typeof(EcaEdgedWeapon), "slices", typeof(ECAObject))]
    public void Slices(ECAObject obj)
    {
        EcaCharacter c = obj.gameObject.GetComponent<EcaCharacter>();
        EcaWeapon w = gameObject.GetComponent<EcaWeapon>();
        if (c != null && w != null)
        {
            c.life -= w.power;
            if (c.life < 0)
                c.life = 0;


            if (particleSlice == null)
            {
                particleSlicePrefab = Instantiate(Resources.Load("Particles/Particle_Slice"), transform) as GameObject;
                particleSlice = particleSlicePrefab.GetComponent<ParticleSystem>();
            }
            else
            {
                Instantiate(particleSlice, transform);
                particleSlice = transform.Find(particleSlice.name + "(Clone)").GetComponent<ParticleSystem>();
            }

            particleSlice.Stop();
            particleSlice.Play();
        }
    }
}