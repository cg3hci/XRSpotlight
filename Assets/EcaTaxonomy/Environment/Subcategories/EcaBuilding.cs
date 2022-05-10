using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;

/// <summary>
/// <b> Building </b> is a <see cref="EcaEnvironment"/> subclass that represents a building.
/// </summary>
[EcaRules4All("building")]
[RequireComponent(typeof(EcaEnvironment))]
[DisallowMultipleComponent]
public class EcaBuilding : MonoBehaviour
{
}