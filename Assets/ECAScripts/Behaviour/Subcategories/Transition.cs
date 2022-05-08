using System;
using UnityEngine;
using EcaRules;
using UnityEngine.SceneManagement;
using Behaviour = EcaRules.Behaviour;

/// <summary>
/// <b>Transition</b> is a <see cref="Behaviour"/> that is used to trigger a transition to another scene.
/// </summary>
[ECARules4All("transition")]
[RequireComponent(typeof(Behaviour))]
[DisallowMultipleComponent]
public class Transition : MonoBehaviour
{
    /// <summary>
    /// <b>Reference</b> is the Unity Scene to transition to.
    /// </summary>
    [StateVariable("reference", ECARules4AllType.Identifier)]
    public Scene reference;

    /// <summary>
    /// <b>Teleports</b> changes the current scene to the scene referenced by <see cref="reference"/>.
    /// </summary>
    /// <param name="reference"></param>
    [Action(typeof(Transition), "teleports to", typeof(Scene))]
    public void Teleports(Scene reference)
    {
        if (reference.name != SceneManager.GetActiveScene().name)
        {
            SceneManager.LoadScene(reference.name);
        }

        //DOUBT: come identificare il giocatore nella scena? è giusto che sia Transition e non ECAobject?
        //Giocatore.posizione = reference.position.GetPosition();
        //TODO: test per controllare che la scena esista
    }
}