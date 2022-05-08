using System;
using EcaRules;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace EcaRules
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