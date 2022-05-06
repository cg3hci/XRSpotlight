using System;
using UnityEngine;
using ECARules4All;
using ECARules4All.RuleEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ECARules4All
{
    public class ECAObjectXRLoader : MonoBehaviour
    {
        private void Awake()
        {
            var ecaObjects = FindObjectsOfType<ECAObject>();

            foreach (var obj in ecaObjects)
            {
                obj.gameObject.AddComponent<XRSimpleInteractable>();
                XRSimpleInteractable interact = obj.GetComponent<XRSimpleInteractable>(); 
            }
        }
    }
}