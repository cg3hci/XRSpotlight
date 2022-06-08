using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using UnityEngine;
using UnityEngine.Events;
using XRSpotlightGUI.Configuration;
using Event = XRSpotlightGUI.Configuration.Event;
using Object = System.Object;

namespace XRSpotlightGUI
{
    public class InferenceEngine
    {
        private static InferenceEngine singleton;

        private Mapping mapping;
        private Dictionary<string, Element> elementIndex;

        public static InferenceEngine GetInstance(Toolkits toolkit)
        {
            if (singleton == null)
            {
                singleton = new InferenceEngine(toolkit);
            }

            return singleton;
        }

        private InferenceEngine(Toolkits toolkit)
        {
            switch (toolkit)
            {
                case Toolkits.MRTK:
                    string json =
                        File.ReadAllText("Assets/XRSpotlightGUI/Configuration/ConfigurationScripts/MRTKconfig.json");
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
                Type componentType = ClassForName(element.className);

                if (componentType == null)
                    continue;
                elementIndex.Add(element.className, element);

                if (!element.isComponent)
                    continue;

                var objs = GameObject.FindObjectsOfType(componentType);

                if (objs == null)
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
                if (component.GetType().FullName == null)
                    continue;

                if (!elementIndex.ContainsKey(component.GetType().FullName))
                    continue;

                var element = elementIndex[component.GetType().FullName];

                AnalyseEvents(element, rules, component);
            }

            return rules.ToArray();
        }

        private void AnalyseEvents(Element element, List<InferredRule> rules, object component)
        {
            foreach (var evt in element.events)
            {
                InferRule(evt, rules, component);
            }


            if (element.eventReferences != null)
            {
                foreach (var evtRef in element.eventReferences)
                {
                    var path = this.FollowReferencePath(evtRef.reference, component);
                    var pathArray = path as System.Collections.IList;
                    if (pathArray == null)
                        continue;
                    foreach (object o in pathArray)
                    {
                        if (!elementIndex.ContainsKey(o.GetType().FullName))
                            continue;

                        var referenced = elementIndex[o.GetType().FullName];

                        AnalyseEvents(referenced, rules, o);
                    }
                }
            }
        }

        private void InferRule(Event evt, List<InferredRule> rules, object component)
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

            object path = FollowReferencePath(evt.reference, component);

            if (path is UnityEventBase unityEvent)
            {
                for (var i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                {
                    if (unityEvent.GetPersistentTarget(i) == null)
                        continue;
                    InferredAction action = new InferredAction(
                        unityEvent.GetPersistentTarget(i),
                        $"executes {unityEvent.GetPersistentMethodName(i)}",
                        unityEvent.GetPersistentMethodName(i));

                    rule.Add(action);
                }
            }
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
                    if (fieldInfo != null)
                    {
                        current = fieldInfo.GetValue(component);
                        if (current == null) return null;
                    }
                }

                if (reference.member == "property")
                {
                    PropertyInfo propertyInfo = t.GetProperty(reference.name);
                    if (propertyInfo == null) return null;
                    current = propertyInfo.GetValue(component);
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

            if (phase.Equals("idle")) p = Phases.Idle;
            if (phase.Equals("address")) p = Phases.Addressed;
            if (phase.Equals("select")) p = Phases.Selected;
            if (phase.Equals("movement")) p = Phases.Moved;
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
        MRTK,
        SteamVR
    }
}