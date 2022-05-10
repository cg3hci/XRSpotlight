using EcaRules;
using UnityEngine;

/// <summary>
/// The <b>Weapon</b> class is a base class for all weapons.
/// </summary>
[EcaRules4All("weapon")]
[RequireComponent(typeof(EcaProp))] 
[DisallowMultipleComponent]
public class EcaWeapon:MonoBehaviour
{
    /// <summary>
    /// <b>Power</b>: a float value that represents the power of the weapon.
    /// </summary>
    [EcaStateVariable("power", EcaRules4AllType.Float)] public float power;
}