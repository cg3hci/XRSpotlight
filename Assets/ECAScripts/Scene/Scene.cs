using System;
using EcaRules;
using UnityEngine;

/// <summary>
/// A <b>Scene</b> represents a setting in the environment a user can visit in a continuous way. We associate it to a
/// videogame level: when the level changes, the scene changes too. It defines passive actions for entering and leaving
/// the scene
/// </summary>
[ECARules4All("scene")]
[RequireComponent(typeof(ECAObject))]
[DisallowMultipleComponent]
public class Scene : MonoBehaviour
{
    //DOUBT: Aggiunto in questo file nome e posizione della scena di arrivo, può avere senso?
    //This component renames the GameObject to the Scene name, for "rule starting on start" purposes
    /// <summary>
    /// <b>Name</b>: the name of the scene
    /// </summary>
    [StateVariable("name", ECARules4AllType.Text)] public string name;
    /// <summary>
    /// <b>Position</b>: the position of the scene
    /// </summary>
    [StateVariable("position", ECARules4AllType.Position)] public Position position;

    private void Start()
    {
        name =  UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        gameObject.name = name;
        
    }

    /// <summary>
    /// <b>_Teleports</b>: a passive verb, see <see cref="Character"/> for the implementation
    /// </summary>
    //TODO: implement
    [Action(typeof(Character), "teleports to", typeof(Scene))]
    public void _Teleports()
    {
        
    }
}