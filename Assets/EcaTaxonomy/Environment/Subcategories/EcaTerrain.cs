using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;

/// <summary>
/// <b>Terrain</b> is an <see cref="EcaEnvironment"/> subclass that represents a terrain.
/// </summary>
[EcaRules4All("terrain")]
[RequireComponent(typeof(EcaEnvironment))]
[DisallowMultipleComponent]
public class EcaTerrain : MonoBehaviour
{
}