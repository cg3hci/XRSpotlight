using System;
using UnityEngine;
using ECARules4All.RuleEngine;

/// <summary>
/// <b>Bullet</b>: this class it is a type of <see cref="Weapon"/> that is usually expelled from another object in the scene, usually a <see cref="Firearm"/> object
/// </summary>
[ECARules4All("bullet")]
[RequireComponent(typeof(Weapon))]
[DisallowMultipleComponent]
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// <b>Speed</b>: this is the speed of the bullet
    /// </summary>
    [StateVariable("speed", ECARules4AllType.Float)] public float speed;
    //TODO FUTURE: eventualmente avere un'evento per quando colpisce qualcosa
    

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}