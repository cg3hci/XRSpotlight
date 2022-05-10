using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;


/// <summary>
/// In <b>Prop</b> category we represent generic objects that can be placed in a scene and manipulated by characters.
/// The possible sub-categories are, in this case, several; we can have passive actions, such as <c>wear</c> in Clothing
/// script.
/// </summary>
[EcaRules4All("prop")]
[RequireComponent(typeof(ECAObject))] 
[DisallowMultipleComponent]
public class EcaProp : MonoBehaviour
{
    /// <summary>
    /// <b>Price</b>: The price of the prop object.
    /// </summary>
    [EcaStateVariable("price", EcaRules4AllType.Float)] public float price;
}
