using System.Collections;
using System.Collections.Generic;
using ECARules4All.RuleEngine;
using UnityEngine;

/// <summary>
/// <b> Building </b> is a <see cref="Environment"/> subclass that represents a building.
/// </summary>
[ECARules4All("building")]
[RequireComponent(typeof(Environment))]
[DisallowMultipleComponent]
public class Building : MonoBehaviour
{
}