using System;
using UnityEngine;
using EcaRules;
using UnityEngine.SceneManagement;

/// <summary>
/// <b>Transition</b> is a <see cref="EcaBehaviour"/> that is used to trigger a transition to another scene.
/// </summary>
[EcaRules4All("transition")]
[RequireComponent(typeof(EcaBehaviour))]
[DisallowMultipleComponent]
public class EcaTransition : MonoBehaviour
{
    /// <summary>
    /// <b>Reference</b> is the Unity Scene to transition to.
    /// </summary>
    [EcaStateVariable("reference", EcaRules4AllType.Identifier)]
    public EcaScene reference;

    /// <summary>
    /// <b>Teleports</b> changes the current scene to the scene referenced by <see cref="reference"/>.
    /// </summary>
    /// <param name="reference"></param>
    [EcaAction(typeof(EcaTransition), "teleports to", typeof(EcaScene))]
    public void Teleports(EcaScene reference)
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