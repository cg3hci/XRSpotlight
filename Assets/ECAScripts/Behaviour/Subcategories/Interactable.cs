using System;
using UnityEngine;
using EcaRules;
using Behaviour = EcaRules.Behaviour;

/// <summary>
/// <b>Interactable</b> is a <see cref="Behaviour">Behaviour</see> that can be attached to an object in order to make it
/// interactable with the player collison. If the action is not player initiated, then refer to <see cref="Trigger"/>
/// </summary>
[ECARules4All("interactable")]
[RequireComponent(typeof(Behaviour))]
[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class Interactable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //PROVE
        EcaEventBus.GetInstance().Publish(new EcaAction(other.gameObject, "interacts with", this.gameObject));
        
    }

    private void OnCollisionEnter(Collision other)
    {
        EcaEventBus.GetInstance().Publish(new EcaAction(other.gameObject, "interacts with", this.gameObject));
    }
    
    private void OnTriggerExit(Collider other)
    {
        //PROVE
        EcaEventBus.GetInstance().Publish(new EcaAction(other.gameObject, "stops-interacting with", this.gameObject));
        
    }

    private void OnCollisionExit(Collision other)
    {
        EcaEventBus.GetInstance().Publish(new EcaAction(other.gameObject, "stops-interacting with", this.gameObject));
    }
}