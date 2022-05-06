using System.Collections;
using System.Collections.Generic;
using ECARules4All.RuleEngine;
using UnityEngine;

/// <summary>
/// <b>Terrain</b> is an <see cref="Environment"/> subclass that represents a terrain.
/// </summary>
[ECARules4All("terrain")]
[RequireComponent(typeof(Environment))]
[DisallowMultipleComponent]
public class Terrain : MonoBehaviour
{
}