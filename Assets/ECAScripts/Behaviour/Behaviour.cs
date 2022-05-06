using System;
using UnityEngine;

namespace ECARules4All.RuleEngine
{
    /// <summary>
    /// <b>Behaviour</b> is a Component that does not derive from the <see cref="ECAObject">ECAObject</see> family, it's possible to apply more than one
    /// in order to define extra behaviours 
    /// </summary>
    [ECARules4All("behaviour")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ECAObject))]
    public class Behaviour : MonoBehaviour
    {
        
    }
}