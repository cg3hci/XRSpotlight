using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;
/// <summary>
/// <b>Interaction</b> contains all the elements allowing some interaction with the scene and its objects.
/// The difference between interactions and <see cref="EcaRules.Behaviour">Behaviours </see> is that interactions
/// are the ones that a typical user would perceive as physical entities of their own, which would exists
/// independently from other objects.
/// </summary>
[ECARules4All("interaction")]
[RequireComponent(typeof(ECAObject))] 
[DisallowMultipleComponent]
public class Interaction : MonoBehaviour
{
 
}
