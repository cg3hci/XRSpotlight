using UnityEngine;
using EcaRules;

/// <summary>
/// <b>Shield</b> class allows to create a shield for defending from a <see cref="EcaWeapon"/>.
/// </summary>
[EcaRules4All("shield")]
[RequireComponent(typeof(EcaWeapon))]
[DisallowMultipleComponent]
public class EcaShield : MonoBehaviour
{
    /// <summary>
    /// <b>Blocks</b>: This action allows to block the <see cref="EcaWeapon"/> attack.
    /// </summary>
    /// <param name="ecaWeapon"></param>
    [EcaAction(typeof(EcaShield), "blocks", typeof(EcaWeapon))]
    public void Blocks(EcaWeapon ecaWeapon)
    {
        if (ecaWeapon.power > 0.0f)
        {
            EcaCharacter c = GetComponentInParent<EcaCharacter>();
            if (c != null)
            {
                c.life -= ecaWeapon.power / (1 + GetComponent<EcaWeapon>().power);
            }
        }
    }
}