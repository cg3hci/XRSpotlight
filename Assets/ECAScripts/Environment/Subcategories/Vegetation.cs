using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;

/// <summary>
/// <b>Vegetation</b> is an <see cref="Environment"/> subclass that represents a vegetation object.
/// </summary>
[ECARules4All("vegetation")]
[RequireComponent(typeof(Environment))]
[DisallowMultipleComponent]
public class Vegetation : MonoBehaviour
{
}