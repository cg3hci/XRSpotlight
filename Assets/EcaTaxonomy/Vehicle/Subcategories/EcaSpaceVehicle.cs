using System.Collections;
using EcaRules;
using UnityEngine;

/// <summary>
/// A <b>SpaceVehicle</b> can fly in the air.
/// </summary>
[EcaRules4All("space-vehicle")]
[RequireComponent(typeof(EcaVehicle))]
[DisallowMultipleComponent]
public class EcaSpaceVehicle : MonoBehaviour
{
    private bool isBusyMoving;
    /// <summary>
    /// <b>Oxygen</b> is the resource that the <b>SpaceVehicle</b> uses to fly.
    /// </summary>
    [EcaStateVariable("oxygen", EcaRules4AllType.Float)] public float oxygen;   
    /// <summary>
    /// <b>Gravity</b> is the force of gravity that the <b>SpaceVehicle</b> experiences.
    /// </summary>
    [EcaStateVariable("gravity", EcaRules4AllType.Float)] public float gravity;

    /// <summary>
    /// <b>TakesOff</b> is the action that the <b>SpaceVehicle</b> can perform to take off.
    /// </summary>
    /// <param name="p">The new position</param>
    [EcaAction(typeof(EcaSpaceVehicle), "takes-off", typeof(EcaPosition))]
    public void TakesOff(EcaPosition p)
    {
        float speed = 20.0F;
        Vector3 endMarker = new Vector3(p.x, p.y, p.z);
        StartCoroutine(MoveObject(speed, endMarker));
    }
        
    /// <summary>
    /// <b>Lands</b>: The action that the <b>SpaceVehicle</b> can perform to land.
    /// </summary>
    /// <param name="p">The new position</param>
    [EcaAction(typeof(EcaSpaceVehicle), "lands", typeof(EcaPosition))]
    public void Lands(EcaPosition p)
    {
        float speed = 20.0F;
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
            yield return null;
        }

        isBusyMoving = false;
    }
}