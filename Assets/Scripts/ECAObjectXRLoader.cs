using System;
using EcaRules;
using UnityEngine;
using InteactableMRTK = Microsoft.MixedReality.Toolkit.UI.Interactable;
namespace EcaRules
{
    public class ECAObjectXRLoader : MonoBehaviour
    {
        public GameObject canvasPanel;
        private bool canvasActive = false;
        private void Awake()
        {
            var ecaObjects = FindObjectsOfType<ECAObject>();

            foreach (var obj in ecaObjects)
            {
                obj.gameObject.AddComponent<InteactableMRTK>();
                InteactableMRTK interactable =  obj.gameObject
                    .GetComponent<InteactableMRTK>();
                interactable.OnClick.AddListener(() =>
                {
                    canvasPanel.SetActive(!canvasActive);
                    canvasActive = !canvasActive;
                });
                
            }
        }
    }
}