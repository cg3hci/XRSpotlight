using System;
using UnityEngine;
using EcaRules;

/// <summary>
/// <b>Bullet</b>: this class it is a type of <see cref="EcaWeapon"/> that is usually expelled from another object in the scene, usually a <see cref="EcaFirearm"/> object
/// </summary>
[EcaRules4All("bullet")]
[RequireComponent(typeof(EcaWeapon))]
[DisallowMultipleComponent]
public class EcaBullet : MonoBehaviour
{
    /// <summary>
    /// <b>Speed</b>: this is the speed of the bullet
    /// </summary>
    [EcaStateVariable("speed", EcaRules4AllType.Float)] public float speed;
    //TODO FUTURE: eventualmente avere un'evento per quando colpisce qualcosa
    

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}