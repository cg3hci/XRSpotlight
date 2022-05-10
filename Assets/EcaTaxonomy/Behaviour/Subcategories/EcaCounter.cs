using System;
using UnityEngine;
using EcaRules;

/// <summary>
/// <b>Counter</b> is a <see cref="EcaBehaviour">Behaviour</see> that enables the object to keep track of countable events
/// <example> Player steps, interaction count</example>
/// </summary>
[EcaRules4All("counter")]
[RequireComponent(typeof(EcaBehaviour))]
[DisallowMultipleComponent]
public class EcaCounter : MonoBehaviour
{
    /// <summary>
    /// <b>count</b> is the current count of the counter
    /// </summary>
    [EcaStateVariable("count", EcaRules4AllType.Float)]
    public float count;

    /// <summary>
    /// <b>Changes</b> changes the count of the counter
    /// </summary>
    /// <param name="amount"> the amount to set </param>
    [EcaAction(typeof(EcaCounter), "changes","count", "to", typeof(float))]
    public void ChangesCount(float amount)
    {
        count = amount;
    }
}