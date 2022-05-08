using System;
using EcaRules;
using ECAScripts.Utils;
using UnityEngine;

/// <summary>
/// <b>Food</b> is a class that represents something that can be eaten.
/// </summary>
[ECARules4All("food")]
[RequireComponent(typeof(Prop))]
[DisallowMultipleComponent]
public class Food: MonoBehaviour
{
    /// <summary>
    /// <b>Weight</b>: is the weight of the food.
    /// </summary>
    [StateVariable("weight", ECARules4AllType.Float)] public float weight;
    /// <summary>
    /// <b>Expiration</b>: is the expiration date of the food.
    /// </summary>
    [StateVariable("expiration", ECARules4AllType.Time)] public DateTime expiration = DateTime.MaxValue;
    /// <summary>
    /// <b>Description</b>: is the description of the food.
    /// </summary>
    [StateVariable("description", ECARules4AllType.Text)] public string description;
    /// <summary>
    /// <b>Eaten</b>: is true if the food has been eaten.
    /// </summary>
    [StateVariable("eaten", ECARules4AllType.Boolean)] public bool eaten;

    /// <summary>
    /// <b>_Eats</b> is the method that is called when the food is eaten. This is a passive action, so the Food type
    /// is not in the subject of the action, but on the object.
    /// </summary>
    /// <param name="c">The character that eats the food</param>
    [Action(typeof(Character), "eats", typeof(Food))]
    public void _Eats(Character c)
    {
        if (expiration > DateTime.Now)
        {
            GetComponent<ECAObject>().isActive.Assign(ECABoolean.BoolType.NO);
            GetComponent<ECAObject>().UpdateVisibility();
            eaten = true;
        }
    }

}