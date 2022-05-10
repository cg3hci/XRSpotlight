using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;

/// <summary>
/// <b>Vegetation</b> is an <see cref="EcaEnvironment"/> subclass that represents a vegetation object.
/// </summary>
[EcaRules4All("vegetation")]
[RequireComponent(typeof(EcaEnvironment))]
[DisallowMultipleComponent]
public class EcaVegetation : MonoBehaviour
{
}