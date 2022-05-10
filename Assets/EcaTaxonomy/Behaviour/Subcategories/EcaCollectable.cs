using System;
using UnityEngine;
using EcaRules;

/// <summary>
/// <b>Collectable</b> is a <see cref="EcaBehaviour">Behaviour</see> that lets an object to be taken inside a player/object owned inventory, or instantly used
/// for interacting with other objects in the scene
/// <example>An object is collected, then a lock on a door unlocks</example>
/// </summary>
[EcaRules4All("collectable")]
[RequireComponent(typeof(EcaBehaviour))]
[DisallowMultipleComponent]
public class EcaCollectable : MonoBehaviour
{
   
}