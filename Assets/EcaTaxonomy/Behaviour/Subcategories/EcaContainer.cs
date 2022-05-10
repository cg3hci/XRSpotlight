using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EcaRules;
using ECAScripts.Utils;
using Object = UnityEngine.Object;
/// <summary>
/// <b>Container</b> is a <see cref="EcaBehaviour">Behaviour</see> that enables the object to hold other objects.
/// </summary>
[EcaRules4All("container")]
[RequireComponent(typeof(EcaBehaviour))]
[DisallowMultipleComponent]
public class EcaContainer : MonoBehaviour
{
    /// <summary>
    /// <b>Capacity</b> is the maximum number of objects that can be held by the container.
    /// </summary>
    [EcaStateVariable("capacity", EcaRules4AllType.Integer)] public int capacity;
    /// <summary>
    /// <b>objectsCount</b> is the number of objects that are currently held by the container.
    /// </summary>
    [EcaStateVariable("objectsCount", EcaRules4AllType.Integer)] public int objectsCount;
    /// <summary>
    /// <b>objectsList</b> is the list of objects that are currently held by the container.
    /// </summary>
    private List<GameObject> objectsList;

    /// <summary>
    /// <b>Start</b> instantiates the objectsList.
    /// </summary>
    private void Start()
    {
        objectsList = new List<GameObject>();
    }

    /// <summary>
    /// <b>Inserts</b> inserts an object into the container.
    /// </summary>
    /// <param name="o">The gameObject to be stored inside the container</param>
    [EcaAction(typeof(EcaContainer), "inserts", typeof(GameObject))]
    public void Inserts(GameObject o)
    {
        if (!objectsList.Contains(o))
        {
            if (objectsCount < capacity)
            {
                objectsList.Add(o);
                objectsCount++;
            }

            o.GetComponent<ECAObject>().isActive.Assign(ECABoolean.BoolType.NO);
        }
    }

    /// <summary>
    /// <b>Removes</b> removes an object from the container.
    /// </summary>
    /// <param name="o">The gameObject to be removed from the container</param>
    [EcaAction(typeof(EcaContainer), "removes", typeof(GameObject))]
    public void Removes(GameObject o)
    {
        objectsList.Remove(o);
        if (objectsList.Count < objectsCount)
        {
            objectsCount--;
        }

        Vector3 fwd = gameObject.transform.forward;
        o.transform.position = gameObject.transform.position + (fwd * 3);
        o.GetComponent<ECAObject>().isActive.Assign(ECABoolean.BoolType.YES);;
    }

    /// <summary>
    /// <b>Empties</b> empties the container.
    /// </summary>
    [EcaAction(typeof(EcaContainer), "empties")]
    public void Empties()
    {

        foreach (GameObject o in objectsList.ToList())
        {
            Removes(o);
        }

        objectsCount = 0;
    }
}