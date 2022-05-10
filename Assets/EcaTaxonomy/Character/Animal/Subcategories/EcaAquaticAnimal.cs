using System.Collections;
using System.Linq.Expressions;
using EcaRules;
using UnityEngine;
/// <summary>
/// This class represents an animal that can swim.
/// </summary>
[EcaRules4All("aquatic-animal")]
[RequireComponent(typeof(EcaAnimal))]
[DisallowMultipleComponent]
public class EcaAquaticAnimal : MonoBehaviour
{
    private bool isBusyMoving = false;
    /// <summary>
    /// <b>IdleAnimation</b>: is the animation that is played when the aquatic animal is idle.
    /// </summary>
    public string IdleAnimation = "";
    /// <summary>
    /// <b>SwimAnimation</b>: is the animation that is played when the aquatic animal is swimming.
    /// </summary>
    public string SwimAnimation = "";

    /// <summary>
    /// <b>Swims</b>: This method is used to move the aquatic animal to a specific position with a swimming animation.
    /// </summary>
    /// <param name="p">The position where to swim</param>
    [EcaAction(typeof(EcaAquaticAnimal), "swims to", typeof(EcaPosition))]
    public void Swims(EcaPosition p)
    {
        float speed = 0.5F;
        Vector3 endMarker = new Vector3(p.x, p.y, p.z);
        StartCoroutine(MoveObject(speed, endMarker));
    }

    /// <summary>
    /// <b>Swims</b>: This method is used to move the aquatic animal through a path with a swimming animation.
    /// </summary>
    /// <param name="p">The path</param>
    [EcaAction(typeof(EcaAquaticAnimal), "swims on", typeof(EcaPath))]
    public void Swims(EcaPath p)
    {
        StartCoroutine(WaitForOrderedMovement(p, "swims"));
    }

    private IEnumerator MoveObject(float speed, Vector3 endMarker)
    {
        isBusyMoving = true;
        Animator anim = gameObject.GetComponent<Animator>();
        anim.Play(SwimAnimation);
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
        anim.Play(IdleAnimation);
    }
    
    private IEnumerator WaitForOrderedMovement(EcaPath p, string method)
    {
        foreach (EcaPosition pos in p.Points)
        {
            while (isBusyMoving)
            {
                yield return null;
            }

            switch (method)
            {
                case "swims": Swims(pos);
                    break;
            }

        }
    }
}