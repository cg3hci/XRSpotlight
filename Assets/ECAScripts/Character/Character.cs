using System;
using System.Collections;
using UnityEngine;
using EcaRules;
using ECAScripts.Utils;
using UnityEditor;


/// <summary>
/// A <b>Character</b> is a type of ECAObject, it represents an animal, a humanoid, a robot or a generic creature.
/// A Character can be autonomous or controlled by the player.
/// </summary>
[ECARules4All("character")]
[RequireComponent(typeof(ECAObject), typeof(Animator))]
[DisallowMultipleComponent]
public class Character : MonoBehaviour
{
    /// <summary>
    /// <b>life</b> is the life of the character.
    /// </summary>
    [StateVariable("life", ECARules4AllType.Float)] public float life;
    /// <summary>
    /// <b>playing</b> to identify whether it is player-controlled or not.
    /// </summary>
    [StateVariable("playing", ECARules4AllType.Boolean)] public ECABoolean playing;
    private bool isBusyMoving = false;
    private Animator anim;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    /// <summary>
    /// <b>Interacts</b>: a character can interact with other interactable objects.
    /// There is no need for implementation, see <see cref="Interactable"/> for more details.
    /// </summary>
    /// <param name="o">The interactable object</param>
    [Action(typeof(Character), "interacts with", typeof(Interactable))]
    public void Interacts(ECAObject o)
    {
    }
    
    /// <summary>
    /// <b>Stops interaction</b>: a character can stop the interaction with other interactable objects.
    /// There is no need for implementation, see <see cref="Interactable"/> for more details.
    /// </summary>
    /// <param name="o">The interactable object</param>
    [Action(typeof(Character), "stops-interacting with", typeof(Interactable))]
    public void StopsInteracting(ECAObject o)
    {
    }

    /// <summary>
    /// <b>Jumps</b>: The character can jump to a given position
    /// </summary>
    /// <param name="p">The position where to jump</param>
    [Action(typeof(Character), "jumps to", typeof(Position))]
    public void Jumps(Position p)
    {
        float speed = 5.0F;
        Vector3 endMarker = new Vector3(p.x, p.y, p.z);
        StartCoroutine(MoveObject(speed, endMarker));
    }

    private IEnumerator MoveObject( float speed, Vector3 endMarker)
    {
        isBusyMoving = true;
        Vector3 startMarker = gameObject.transform.position;
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(startMarker, endMarker);
        while (gameObject.transform.position != endMarker)
        {
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.

            gameObject.transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
            GetComponent<ECAObject>().p.Assign(gameObject.transform.position);

            yield return null;
        }
        GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
        isBusyMoving = false;
    }
    /// <summary>
    /// <b>Jumps</b>: The character can jump to a given path
    /// </summary>
    /// <param name="p">The path</param>
    [Action(typeof(Character), "jumps on", typeof(Path))]
    public void Jumps(Path p)
    {
        StartCoroutine(WaitForOrderedMovement(p));
    }

    private IEnumerator WaitForOrderedMovement(Path p)
    {
        foreach (Position pos in p.Points)
        {
            while (isBusyMoving)
            {
                yield return null;
            }

            Jumps(pos);
        }
    }


    /// <summary>
    /// <b>Starts animation</b>: The character can start an animation
    /// </summary>
    /// <param name="s">The string that represents the already built animation of the character game object</param>
    [Action(typeof(Character), "starts-animation", typeof(string))]
    public void StartsAnimation(string s)
    {
        //In order to make it works the template builder must specify a trigger that is directly connected to
        //the idle state and brings to the correct animation
        anim.Play(s);
    }
}