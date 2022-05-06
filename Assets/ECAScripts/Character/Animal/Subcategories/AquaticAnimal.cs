using System.Collections;
using System.Linq.Expressions;
using ECARules4All;
using ECARules4All.RuleEngine;
using UnityEngine;
/// <summary>
/// This class represents an animal that can swim.
/// </summary>
[ECARules4All("aquatic-animal")]
[RequireComponent(typeof(Animal))]
[DisallowMultipleComponent]
public class AquaticAnimal : MonoBehaviour
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
    [Action(typeof(AquaticAnimal), "swims to", typeof(Position))]
    public void Swims(Position p)
    {
        float speed = 0.5F;
        Vector3 endMarker = new Vector3(p.x, p.y, p.z);
        StartCoroutine(MoveObject(speed, endMarker));
    }

    /// <summary>
    /// <b>Swims</b>: This method is used to move the aquatic animal through a path with a swimming animation.
    /// </summary>
    /// <param name="p">The path</param>
    [Action(typeof(AquaticAnimal), "swims on", typeof(Path))]
    public void Swims(Path p)
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
    
    private IEnumerator WaitForOrderedMovement(Path p, string method)
    {
        foreach (Position pos in p.Points)
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