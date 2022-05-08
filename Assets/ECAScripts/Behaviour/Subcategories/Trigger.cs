using System;
using UnityEngine;
using EcaRules;
using Behaviour = EcaRules.Behaviour;

/// <summary>
/// <b>Trigger</b> is a <see cref="Behaviour"/> that can be used to trigger an action without an explicit request
/// from the player. If the action is player initiated, then refer to <see cref="Interactable"/>
/// </summary>
[ECARules4All("trigger")]
[RequireComponent(typeof(Behaviour))]
[DisallowMultipleComponent]
public class Trigger : MonoBehaviour
{
    /// <summary>
    /// <b>Triggers</b> emits an event when the trigger is activated.
    /// </summary>
    /// <param name="ecaAction"> The event to trigger in the scene.</param>
    [Action(typeof(Trigger), "triggers", typeof(EcaAction))]
    public void Triggers(EcaAction ecaAction)
    {
        EcaEventBus.GetInstance().Publish(ecaAction);
        //DOUBT: l'evento lo deve descrivere a mano l'End user Developer?
    }
}