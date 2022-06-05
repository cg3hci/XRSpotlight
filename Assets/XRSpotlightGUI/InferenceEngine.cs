using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using XRSpotlightGUI.Configuration;

namespace XRSpotlightGUI
{
    public class InferenceEngine
    {
        private Mapping mapping;
        public InferenceEngine(Toolkits toolkit)
        {
            switch (toolkit)
            {
                case Toolkits.MRTK:
                    string json = File.ReadAllText("Assets/XRSpotlightGUI/Configuration/ConfigurationScripts/MRTKconfig.json");
                    this.mapping = JsonUtility.FromJson<Mapping>(json);
                    break;
            }
        }

        public GameObject[] FindInteractableObjects()
        {
            if (this.mapping == null)
                return null;

            List<GameObject> gobjs = new List<GameObject>();
            foreach (var element in this.mapping.elements)
            {

                Debug.Log($"Trying with {element.component}");
                Type componentType = null;
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (ass.FullName.StartsWith("System."))
                        continue;
                    componentType = ass.GetType(element.component);
                    if (componentType != null)
                        break;
                }
                
                if(componentType == null)
                    continue;
                 
                var objs =  GameObject.FindObjectsOfType(componentType);

                if(objs == null)  
                    continue;
                
                foreach (var o in objs)
                {
                    if (o is MonoBehaviour behaviour)
                    {
                        gobjs.Add(behaviour.gameObject); 
                    }
                    
                }
            }
            
            return gobjs.ToArray();
        }
    }


    public enum Toolkits
    {
        MRTK, SteamVR
    }
    
}