using System;
using UnityEngine;
using EcaRules;
using ECAScripts.Utils;

/// <summary>
/// <b>Lock</b> is a <see cref="EcaBehaviour"/> that locks the <see cref="ECAObject"/> it is attached to.
/// It works in a similar way to the <see cref="EcaKeypad"/> behaviour, but it needs to by unlock by other means (like a key).
/// </summary>
[EcaRules4All("lock")]
[RequireComponent(typeof(EcaBehaviour))]
[DisallowMultipleComponent]
public class EcaLock : MonoBehaviour
{
    /// <summary>
    /// <b>locked</b> defines whether the lock is open or not.
    /// </summary>
    [EcaStateVariable("locked", EcaRules4AllType.Boolean)]
    public ECABoolean locked = new ECABoolean(ECABoolean.BoolType.YES);

    /// <summary>
    /// <b>Opens</b> sets the lock to open.
    /// </summary>
    [EcaAction(typeof(EcaLock), "opens")]
    public void Opens()
    {
        locked.Assign(ECABoolean.BoolType.NO);
    }
    
    /// <summary>
    /// <b>Closes</b> sets the lock to closed.
    /// </summary>
    [EcaAction(typeof(EcaLock), "closes")]
    public void Closes()
    {
        locked.Assign(ECABoolean.BoolType.YES);
    }
}