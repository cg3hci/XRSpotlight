using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using UnityEngine;
using UnityEngine.Events;
using XRSpotlightGUI.Configuration;
using Object = System.Object;

namespace XRSpotlightGUI
{
    public class InferenceEngine
    {
        private Mapping mapping;
        private Dictionary<string, Element> elementIndex;

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
            elementIndex = new Dictionary<string, Element>();
            if (this.mapping == null)
                return null;

            List<GameObject> gobjs = new List<GameObject>();
            foreach (var element in this.mapping.elements)
            {
                if(! element.isComponent)
                    continue;
                
                Type componentType = ClassForName(element.className);
                
                if(componentType == null)
                    continue;
                elementIndex.Add(element.className, element);
                
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

        public InferredRule[] InferRuleByGameObject(GameObject gameObject)
        {
            if (elementIndex == null)
            {
                FindInteractableObjects();
            }
            
            List<InferredRule> rules = new List<InferredRule>();

            foreach (var component in gameObject.GetComponents(typeof(Component)))
            {
                if(component.GetType().FullName == null)
                    continue;
                
                if(! elementIndex.ContainsKey(component.GetType().FullName))
                    continue;
                
                var element = elementIndex[component.GetType().FullName];

                foreach (var evt in element.events)
                {
                    InferredRule rule = FindRule(evt.definition, rules);
                    if (rule == null)
                    {
                        rule = new InferredRule()
                        {
                            // TODO read modalities from file
                            modalities = new Modalities()
                            {
                                gaze = false,
                                hand = true,
                                remote = true,
                                touch = true
                            },
                            trigger = this.PhaseFromString(evt.definition)
                        };
                        rules.Add(rule);
                    }

                    UnityEvent unityEvent = FollowReferencePath(evt.reference, component) as UnityEvent;
                    if(unityEvent == null)
                        continue;

                    for (var i = 0; i < unityEvent.GetPersistentEventCount(); i++){
                        if(unityEvent.GetPersistentTarget(i) == null)
                            continue;
                        InferredAction action = new InferredAction(
                            unityEvent.GetPersistentTarget(i), 
                               $"executes {unityEvent.GetPersistentMethodName(i)}",
                            unityEvent.GetPersistentMethodName(i));
                        
                        rule.Add(action);
                    }

                }

            }

            return rules.ToArray();
        }

        private object FollowReferencePath(MemberReference[] references, object component)
        {
            var current = component;
            foreach (var reference in references)
            {
                Type t = component.GetType(); 
                if (reference.member == "field")
                {
                    FieldInfo fieldInfo = t.GetField(reference.name); 
                    current = fieldInfo.GetValue(component);
                    if (current == null) return null;
                }
                // TODO add the descent to other member types
            }
            return current; 
        }

        private InferredRule FindRule(string phase, List<InferredRule> rules)
        {

            Phases p = this.PhaseFromString(phase);
            if (p == Phases.None) return null;

            foreach (var rule in rules)
            {
                if (rule.trigger == p) return rule;
            }

            return null; 
        }

        private Phases PhaseFromString(string phase)
        {
            Phases p = Phases.None;

            if (phase.Equals("address")) p = Phases.Addressed;
            if (phase.Equals("select")) p = Phases.Selected;
            if (phase.Equals("move")) p = Phases.Moved;
            if (phase.Equals("release")) p = Phases.Released;

            return p;
        }

        private Type ClassForName(string name)
        {
            Type componentType = null;
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (ass.FullName.StartsWith("System."))
                    continue;
                componentType = ass.GetType(name);
                if (componentType != null)
                    break;
            }

            return componentType;
        }
    }


    public enum Toolkits
    {
        MRTK, SteamVR
    }
    
}