using ECARules4All.RuleEngine;
using UnityEngine;

/// <summary>
/// The <b>Weapon</b> class is a base class for all weapons.
/// </summary>
[ECARules4All("weapon")]
[RequireComponent(typeof(Prop))] 
[DisallowMultipleComponent]
public class Weapon:MonoBehaviour
{
    /// <summary>
    /// <b>Power</b>: a float value that represents the power of the weapon.
    /// </summary>
    [StateVariable("power", ECARules4AllType.Float)] public float power;
}