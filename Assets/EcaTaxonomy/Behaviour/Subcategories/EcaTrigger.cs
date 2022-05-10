using System;
using UnityEngine;
using EcaRules;

/// <summary>
/// <b>Trigger</b> is a <see cref="EcaBehaviour"/> that can be used to trigger an action without an explicit request
/// from the player. If the action is player initiated, then refer to <see cref="EcaInteractable"/>
/// </summary>
[EcaRules4All("trigger")]
[RequireComponent(typeof(EcaBehaviour))]
[DisallowMultipleComponent]
public class EcaTrigger : MonoBehaviour
{
    /// <summary>
    /// <b>Triggers</b> emits an event when the trigger is activated.
    /// </summary>
    /// <param name="ecaAction"> The event to trigger in the scene.</param>
    [EcaAction(typeof(EcaTrigger), "triggers", typeof(EcaAction))]
    public void Triggers(EcaAction ecaAction)
    {
        EcaEventBus.GetInstance().Publish(ecaAction);
        //DOUBT: l'evento lo deve descrivere a mano l'End user Developer?
    }
}