using System;
using UnityEngine;
using EcaRules;
using Behaviour = EcaRules.Behaviour;

/// <summary>
/// <b>Collectable</b> is a <see cref="Behaviour">Behaviour</see> that lets an object to be taken inside a player/object owned inventory, or instantly used
/// for interacting with other objects in the scene
/// <example>An object is collected, then a lock on a door unlocks</example>
/// </summary>
[ECARules4All("collectable")]
[RequireComponent(typeof(Behaviour))]
[DisallowMultipleComponent]
public class Collectable : MonoBehaviour
{
   
}