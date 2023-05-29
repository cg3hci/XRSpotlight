using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if MRTK
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using XRSpotlightGUI.Configuration;
using Event = XRSpotlightGUI.Configuration.Event;
using Object = UnityEngine.Object;

namespace XRSpotlightGUI
{
    public class InferenceEngine
    {
        private static InferenceEngine singleton;

        private Mapping mapping;
        private Dictionary<string, Element> elementIndex;

        public static InferenceEngine GetInstance()
        {
            if (singleton == null)
            {
                singleton = new InferenceEngine(Utils.toolkit);
            }

            return singleton;
        }

        private InferenceEngine(Toolkits toolkit)
        {
            string json = "";
            switch (toolkit)
            {
                case Toolkits.MRTK:
                    json = File.ReadAllText("Assets/Editor/XRSpotlightGUI/Configuration/ConfigurationScripts/MRTKconfig.json");
                    break;
                case Toolkits.SteamVR:
                    json = File.ReadAllText("Assets/Editor/XRSpotlightGUI/Configuration/ConfigurationScripts/SteamVRconfig.json");
                    break; 
            }
            mapping = JsonUtility.FromJson<Mapping>(json); 
        }

        public GameObject[] FindInteractableObjects()
        {
            elementIndex = new Dictionary<string, Element>();
            if (this.mapping == null)
                return null;

            List<GameObject> gobjs = new List<GameObject>();
            foreach (var element in this.mapping.elements)
            {
                Type componentType = InferredRule.ClassForName(element.className);

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

            return gobjs.Distinct().ToArray();
        }

        public (InferredRule[], InferredBehaviour[]) InferRuleByGameObject(GameObject gameObject)
        {
            if (elementIndex == null)
            {
                FindInteractableObjects();
            }

            List<InferredRule> rules = new List<InferredRule>();
            List<InferredBehaviour> behaviours = new List<InferredBehaviour>();

            foreach (var component in gameObject.GetComponents(typeof(Component)))
            {
                if (component == null)
                {
                    continue;
                }
                if (component.GetType().FullName == null)
                    continue;

                if (!elementIndex.ContainsKey(component.GetType().FullName))
                    continue;

                var element = elementIndex[component.GetType().FullName];

#if MRTK
                // [davide] TEMPORARY WORKAROUND: Receivers are not serialized, they are re-created at each run
                // this does not allow us to inspect them in the Editor. The following is a slightly modified
                // version of the Receiver instantiation in MRTK, triggered by our code. 
                // This may potentially cause memory leaks in the UnityEditor (we are not destroying the Receivers)
                if (component is Interactable interactable)
                {

                    for (int i = 0; i < interactable.InteractableEvents.Count; i++)
                    {
                        var receiver = InteractableEvent.CreateReceiver(interactable.InteractableEvents[i]);
                        if (receiver != null)
                        {
                            interactable.InteractableEvents[i].Receiver = receiver;
                            interactable.InteractableEvents[i].Receiver.Host = interactable;
                        }
                        else
                        {
                            Debug.LogWarning(
                                $"Empty event receiver found on {gameObject.name}, you may want to re-create this asset.");
                        }
                    }
                }
#endif

                ResolvedReference componentReference = new ResolvedReference()
                {
                    resolved = component,
                    propertyPath = ""
                };
                AnalysisContext context = new AnalysisContext()
                {
                    rules = rules,
                    behaviours = behaviours,
                    gameObject = gameObject
                };
                AnalyseEvents(element, componentReference, context);
            }

            /*foreach (var rule in rules)
            {
                if (rule.actions.Count == 0)
                {
                    rules.Remove(rule);
                }
            }*/


            //AggregateByModality(rules);
            
            //rules.Sort();

            return (rules.ToArray(), behaviours.ToArray());
        }

        public UnityEvent GetEvent(MemberReference[] path, object source)
        {
            var startRef = new ResolvedReference()
            {
                resolved = source,
                propertyPath = ""
            };
            var resolved = FollowReferencePaths(path, startRef, true);
            if (resolved.Length > 1)
            {
                Debug.LogWarning($"Expected only one result in following path");
                return null;
            }

            if (resolved.Length == 0)
            {
                return null;
            }
            
            return resolved[0].resolved as UnityEvent;
        }

        private void FindBehaviours(Element element, AnalysisContext context)
        {
            InferredBehaviour behaviour = new InferredBehaviour(context.gameObject, element.className, element.definitionNoEvents);
            context.behaviours.Add(behaviour);
        }

        private void AnalyseEvents(Element element, ResolvedReference component, AnalysisContext context)
        {
            FindBehaviours(element, context);
            
            foreach (var evt in element.events)
            {
                InferRule(evt, component, context, element);
            }

            if (element.eventReferences == null){
                return;
            }
            
            foreach (var evtRef in element.eventReferences)
            {
                var resolved =  this.FollowReferencePaths(evtRef.reference, component);
                
                foreach (ResolvedReference reference in resolved)
                {
                    if (!elementIndex.ContainsKey(reference.resolved.GetType().FullName)) 
                        continue;

                    var referenced = elementIndex[reference.resolved.GetType().FullName];
                    context.prefix = evtRef.reference;
                    var elementType = InferredRule.ClassForName(element.className);
                    context.componentAssemblyName = elementType?.AssemblyQualifiedName;
                    AnalyseEvents(referenced, reference, context);
                }
            }
            
        }
        
        private bool HasModality(Event evt, string modality)
        {
            return evt.modality != null && ((IList)evt.modality).Contains(modality);
        }

        private void InferRule(Event evt, ResolvedReference component, AnalysisContext context, Element element)
        {

            string part = "";
            if (evt.subpart != null && evt.subpart.Length > 0)
            {
                if (!evt.subpart[evt.subpart.Length - 1].nullByDefault)
                {
                    var componentReference = new ResolvedReference()
                    {
                        resolved = component,
                        propertyPath = ""
                    };
                    object[] subparts = FollowReferencePaths(evt.subpart, componentReference);
                    if (subparts.Length > 1)
                    {
                        Debug.LogWarning($"Expected a path containing only one sub-part for reference {evt.subpart}");
                        return;
                    }
                }

                part = evt.subpart[evt.subpart.Length - 1].name;
            }

            InferredRule rule = null;//FindRule(evt.definition, part, context.rules);
            if (rule == null)
            {
                rule = new InferredRule()
                {
                    gameObject = context.gameObject,
                    modalities = new Modalities(),
                    toolkit = Utils.toolkit,
                    trigger = InferredRule.PhaseFromString(evt.definition),
                    part = part
                };
                
                rule.modalities.SetModality(ModalitiesEnum.gaze, HasModality(evt, "gaze"));
                rule.modalities.SetModality(ModalitiesEnum.hand, HasModality(evt, "hand"));
                rule.modalities.SetModality(ModalitiesEnum.grab, HasModality(evt, "grab"));
                rule.modalities.SetModality(ModalitiesEnum.remote, HasModality(evt, "remote"));
                rule.modalities.SetModality(ModalitiesEnum.touch, HasModality(evt, "touch"));
                
            }

            if (evt.reference != null)
            {
                // the mapping defines an event the developer can attach to
                ResolvedReference[] paths = FollowReferencePaths(evt.reference, component);

                if (paths.Length != 1)
                {
                    Debug.Log(
                        $"Found {paths.Length} references  {evt.reference} for component ${component.resolved}. Expected 1.");
                    return;
                }

                var path = paths[0];
                if (path.resolved is UnityEventBase unityEvent)
                {
                    //rule.interaction = path.memberReferences[path.memberReferences.Count - 1].name;
                    for (var i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                    {
                        if (unityEvent.GetPersistentTarget(i) == null)
                            continue;
                        InferredAction action = new InferredAction(
                            unityEvent.GetPersistentTarget(i),
                            $"calls {unityEvent.GetPersistentMethodName(i)}",
                            unityEvent.GetPersistentMethodName(i),
                            component.resolved.GetType().AssemblyQualifiedName);
                        action.index = i;
                        action.inspector = path.propertyPath;
                        action.eventPath = path.memberReferences.ToArray();
                        if (context.prefix != null)
                        {
                            action.componentAssemblyName = context.componentAssemblyName;
                        }

                        SerializedProperty currentCall =
                            new SerializedObject(
                                    rule.gameObject.GetComponent(Type.GetType(action.componentAssemblyName)))
                                .FindProperty(action.inspector + $".m_PersistentCalls.m_Calls.Array.data[{action.index}]");
                        if (currentCall != null)
                        {

                            action.callState = currentCall.FindPropertyRelative("m_CallState").enumValueIndex;
                            action.callMode = currentCall.FindPropertyRelative("m_Mode").enumValueIndex;
                            var m_Arguments = currentCall.FindPropertyRelative("m_Arguments");
                            switch (action.callMode)
                            {
                                case 2:
                                    // object argument
                                    var argument = m_Arguments.FindPropertyRelative("m_ObjectArgument")
                                        .objectReferenceValue;
                                    action.argument = Utils.GetGameObjectPath(argument);
                                    action.argumentType = argument.GetType().AssemblyQualifiedName;
                                    break;
                                case 3:
                                    // int parameter
                                    action.argument = "" + m_Arguments.FindPropertyRelative("m_IntArgument").intValue;
                                    break;
            
                                case 4:
                                    // float value
                                    action.argument =
                                        "" + m_Arguments.FindPropertyRelative("m_FloatArgument").floatValue;
                                    break;
            
                                case 5:
                                    // string value
                                    action.argument = m_Arguments.FindPropertyRelative("m_StringArgument").stringValue;
                                    break;
            
                                case 6:
                                    // bool value
                                    action.argument = "" + m_Arguments.FindPropertyRelative("m_BoolArgument").boolValue;
                                    break;
                            }

                        }

                        rule.Add(action);
                    }
                }
            }
            if(rule.actions.Count > 0)
                context.rules.Add(rule);

        }

        private string defaultActionToString(string defaultAction)
        {
            if (defaultAction.Equals("rotate")) return "changes its rotation";
            if (defaultAction.Equals("scale")) return "changes its scale";
            if (defaultAction.Equals("move")) return "changes its position";

            return "";

        }
        
        private ResolvedReference[] FollowReferencePaths(MemberReference[] references, ResolvedReference component, bool createMissingObjects = false)
        {
            List<ResolvedReference> toReturn = new List<ResolvedReference>();
            BuildReferences(references, component, 0, toReturn, createMissingObjects);

            return toReturn.ToArray();
        }
        
        private void  BuildReferences(
            MemberReference[] references, 
            ResolvedReference component, 
            int i, 
            List<ResolvedReference> resolved,
            bool createMissingObjects)
        {
            if (i >= references.Length)
            {
                resolved.Add(component);
                return;
            }

            var reference = references[i];
            Type t = component.resolved.GetType();
            if (reference.member == "field")
            {
                FieldInfo fieldInfo = t.GetField(reference.name);  
                if (fieldInfo == null)
                {
                    Debug.Log($"Cannot find the field {reference.name} in type {t.FullName}");
                    return;
                }
                var current = fieldInfo.GetValue(component.resolved);
                if (current == null)
                {
                    if (createMissingObjects)
                    {
                        var fieldType =
                            Type.GetType(references[i].type);
                         current = Activator.CreateInstance(fieldType ?? fieldInfo.FieldType );
                         fieldInfo.SetValue(component.resolved, current);
                    }
                    else
                    {
                        Debug.Log($"The field {reference.name} is null in object {component}");
                        return;
                    }
                    
                }
                var currentReference = CreateResolvedReference(component, current, reference);

                FollowValueOrList(references, i, resolved, currentReference, createMissingObjects);
            }

            if (reference.member == "property")
            {
                PropertyInfo propertyInfo = t.GetProperty(reference.name);
                if (propertyInfo == null)
                {
                    Debug.Log($"Cannot find the property {reference.name} in type {t.FullName}");
                    return;
                }
                var current = propertyInfo.GetValue(component.resolved);
                if (current == null)
                {
                    if (createMissingObjects)
                    {
                       var propertyType =
                            Type.GetType(references[i].type);
                        current = Activator.CreateInstance(propertyType ?? propertyInfo.PropertyType);
                        propertyInfo.SetValue(component.resolved, current);
                    }
                    else
                    {
                        Debug.Log($"The property {reference.name} is null in object {component}");
                        return;
                    }
                }
                var currentReference = CreateResolvedReference(component, current, reference);

                FollowValueOrList(references, i, resolved, currentReference, createMissingObjects);
            }
        }

        private ResolvedReference CreateResolvedReference(ResolvedReference component, object current,
            MemberReference reference = null)
        {
            ResolvedReference currentReference = new ResolvedReference()
            {
                resolved = current,
                propertyPath = GetPropertyPath(component, reference)
            };

            currentReference.memberReferences.AddRange(component.memberReferences);
            if (reference != null)
            {
                var referenceClone = reference.Clone() as MemberReference;
                referenceClone.type = current.GetType().AssemblyQualifiedName;
                currentReference.memberReferences.Add(referenceClone);
            }
            return currentReference;
        }

        private void FollowValueOrList(MemberReference[] references, int i, List<ResolvedReference> resolved, ResolvedReference current, bool createMissingObjects)
        {
            var currentArray = current.resolved as System.Collections.IList;
            if (currentArray != null)
            {
                if (createMissingObjects)
                {
                    // we are creating new events handlers, so we add a new element in the list and we continue 
                    // we take the type from action metadata. If none available, we fall back to the declared
                    // list item type, but this does not allow us to crete subclasses of the declared type.
                    
                    var itemType = Type.GetType(references[i].type);
                    
                    if (itemType == null)
                        itemType = current.resolved.GetType().GetGenericArguments().Single();
                    var arrayElement =
                        Activator.CreateInstance(itemType);
                    currentArray.Add(arrayElement);
                    var currentElement = new ResolvedReference()
                    {
                        resolved = arrayElement,
                        propertyPath = $"{current.propertyPath}.Array.data[{currentArray.Count -1}]"
                    };
                    BuildReferences(references, currentElement, i + 1, resolved, createMissingObjects);

                }else{
                    // we have multiple values, we are searching for existing events handlers
                    var c = 0;
                    foreach (var o in currentArray)
                    {

                        var currentElement = CreateResolvedReference(current, o);
                        currentElement.propertyPath = $"{current.propertyPath}.Array.data[{c}]";
                        var itemElement = currentElement.memberReferences.Last()?.Clone() as MemberReference;
                        itemElement.type = o.GetType().AssemblyQualifiedName;
                        currentElement.memberReferences[currentElement.memberReferences.Count - 1] = itemElement;
                        BuildReferences(references, currentElement, i + 1, resolved, createMissingObjects);
                        c++;
                    }
                }
            }
            else
            {
                BuildReferences(references, current, i + 1, resolved, createMissingObjects);
            }
        }
        
        private string GetPropertyPath(ResolvedReference component, MemberReference reference)
        {
            if (reference == null) return null;
           
            var inspectorProperty = reference.inspectorProperty;
            if (reference.inspectorProperty == null)
            {
                inspectorProperty =  reference.name;
            }

            if (!string.IsNullOrEmpty(component.propertyPath))
            {
                return inspectorProperty.Length > 0 ? $"{component.propertyPath}.{inspectorProperty}" : $"{component.propertyPath}";
            }

            return inspectorProperty;
        }

        private InferredRule FindRule(string phase, string part, List<InferredRule> rules)
        {
            Phases p = InferredRule.PhaseFromString(phase);
            if (p == Phases.None) return null;

            foreach (var rule in rules)
            {
                if (rule.trigger == p && rule.part == part) return rule;
            }

            return null;
        }
        
        
    }


    public enum Toolkits
    {
        MRTK,
        SteamVR
    }

    internal class ResolvedReference
    {
        public object resolved;
        public string propertyPath;
        public List<MemberReference> memberReferences = new List<MemberReference>();
    }

    internal class AnalysisContext
    {
        public List<InferredRule> rules;
        public List<InferredBehaviour> behaviours;
        public GameObject gameObject;
        public MemberReference[] prefix;
        public string componentAssemblyName;
    }
    
}