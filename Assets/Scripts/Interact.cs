using UnityEngine;
using EcaRules;

/*
 * This is an example of script that publishes events inside a template
 * NOTE: It doesn't necessarily have to be like this, the only interesting part
 * is at line 29, which is the syntax of a  "Subject / Verb / Object" Action Publish
 */
public class Interact : MonoBehaviour
{
    private GameObject otherGameObject;

    private void OnTriggerEnter(Collider other)
    {
        otherGameObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == otherGameObject)
        {
            otherGameObject = null;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        otherGameObject = other.gameObject;
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject == otherGameObject)
        {
            otherGameObject = null;
        }
    }

    private void Update()
    {
        if (otherGameObject != null && Input.GetButtonDown("Fire1"))
            EcaEventBus.GetInstance().Publish(new EcaAction(otherGameObject, "interacts with", this.gameObject));
    }
}