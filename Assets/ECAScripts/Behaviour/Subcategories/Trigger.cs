using System;
using UnityEngine;
using ECARules4All.RuleEngine;
using Action = ECARules4All.RuleEngine.Action;
using Behaviour = ECARules4All.RuleEngine.Behaviour;

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
    /// <param name="action"> The event to trigger in the scene.</param>
    [Action(typeof(Trigger), "triggers", typeof(Action))]
    public void Triggers(Action action)
    {
        EventBus.GetInstance().Publish(action);
        //DOUBT: l'evento lo deve descrivere a mano l'End user Developer?
    }
}