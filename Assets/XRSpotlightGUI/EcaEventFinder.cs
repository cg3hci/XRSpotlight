using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;
using XRSpotlightGUI;
using Object = UnityEngine.Object;

public class EcaEventFinder : MonoBehaviour
{
    class UnityEventInfo
    {
        public MethodInfo method;
        public GameObject gameObjectScript;
        public string typeOfEvent;
        public string scriptContainingTheEvent;
        public UnityEventInfo(MethodInfo method, GameObject gameObjectScript, string typeOfEvent, string scriptContainingTheEvent)
        {
            this.method = method;
            this.gameObjectScript = gameObjectScript;
            this.typeOfEvent = typeOfEvent;
            this.scriptContainingTheEvent = scriptContainingTheEvent;
        }
    }
    
    void Start()
    {
       // FindInteractableEventsByGameObject(GameObject.Find("OggettoProvaVicky"));
    }

    public static InferredRule[] InferRuleByGameObject(GameObject gameObject)
    {
        List<UnityEventInfo> infos = FindInteractableEventsByGameObject(gameObject);
        if (infos == null)
        {
            return Array.Empty<InferredRule>();
        }
        var rules = new Dictionary<Phases, InferredRule>();
        Phases phase = Phases.None;
        foreach (var info in infos)
        {
            // TODO: these mappings must be defined in the JSON configuration file
            if (info.typeOfEvent.Equals("OnClick")) phase = Phases.Selected; 
            if (info.typeOfEvent.Equals("OnFocusOn")) phase = Phases.Addressed; 
            
            if(phase == Phases.None) continue;

            if (!rules.ContainsKey(phase))
            {
                rules.Add(phase, new InferredRule());
                // TODO: modalities are fixed. These must be defined into the JSON configuration file.
                rules[phase].modalities = new Modalities()
                {
                    gaze = false,
                    hand = true,
                    remote = true,
                    touch = true
                };
                rules[phase].trigger = phase;
            }
            rules[phase].actions.Add(new InferredAction(
                info.gameObjectScript, 
                $"executes {info.scriptContainingTheEvent}.{info.method.Name}",
                info.method.Name));
           
        }

        return rules.Values.ToArray();
        
    }

    private static List<UnityEventInfo> FindInteractableEventsByGameObject(GameObject gameObject)
    {
        List<UnityEventInfo> unityEventInfos = new List<UnityEventInfo>();

        Interactable interactable = gameObject.GetComponent<Interactable>();
        if (interactable == null)
            return null;
        
        //onclick events
        UnityEvent onClick = interactable.OnClick;
        int eventCount = onClick.GetPersistentEventCount();
        for (int i = 0; i < eventCount; i++)
        {
            unityEventInfos=unityEventInfos.Concat(FindEventsInfo(onClick, i)).ToList();
        }
        
        //receivers
        var receivers = gameObject.GetComponent<Interactable>().GetReceivers<ReceiverBase>();
        int receiversCount = receivers.Count();
        for (int i = 0; i < receiversCount; i++)
        {
            ReceiverBase receiverBase = receivers.ElementAt(i);
            //non custom receiver type
            DetectNonCustomReceiverType(receiverBase, ref unityEventInfos, i);
            //TODO custom receiver type
            // unityEventInfos=unityEventInfos.Concat(FindEventsInfo(receivers[i].Event, i, receivers[i].Name)).ToList();
        }
        
        Debug.Log(unityEventInfos);
        return unityEventInfos;
    }

    private static List<UnityEventInfo> FindEventsInfo(UnityEvent unityEvent, int index, string eventType="OnClick")
    {
        List<UnityEventInfo> result = new List<UnityEventInfo>();
        Object target = unityEvent.GetPersistentTarget(index);
        if (target == null) return result;
        
        GameObject gameObjectParameter = GameObject.Find(target.name);
        string methodName = unityEvent.GetPersistentMethodName(index);
        MonoBehaviour[] components = gameObjectParameter.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            UnityEventInfo unityEventInfo = FindSingleEventInfo(methodName, component, gameObjectParameter, eventType);
            if(unityEventInfo != null)
            {
                result.Add(unityEventInfo);
            }
        }

        return result;
    }

    private static UnityEventInfo FindSingleEventInfo(string methodName, MonoBehaviour component, GameObject gameObjectParameter, string eventType)
    {
        MethodInfo methodInfo = component.GetType().GetMethod(methodName);
        if (methodInfo != null)
        {
            return new UnityEventInfo(methodInfo, gameObjectParameter, eventType, component.GetType().Name);
        }
        return null;
    }
    private static void DetectNonCustomReceiverType(ReceiverBase receiverBase, ref List<UnityEventInfo> unityEventInfos,
        int index)
    {
        string receiverName = receiverBase.GetType().Name;
        switch (receiverName)
        {
            case "InteractableOnFocusReceiver":
                InteractableOnFocusReceiver interactableReceiver = receiverBase as InteractableOnFocusReceiver;
                HandleInteractableOnFocusReceiver(interactableReceiver, ref unityEventInfos, index);
                break;
            /*case "InteractableOnClickReceiver":
                InteractableOnClickReceiver interactableOnClickReceiver = receiverBase as InteractableOnClickReceiver;
                return interactableOnClickReceiver;
            case "InteractableAudioReceiver":
                InteractableAudioReceiver interactableAudioReceiver = receiverBase as InteractableAudioReceiver;
                return interactableAudioReceiver;
            case "InteractableOnGrabReceiver":
                InteractableOnGrabReceiver interactableOnGrabReceiver = receiverBase as InteractableOnGrabReceiver;
                return interactableOnGrabReceiver;
            case "InteractableOnHoldReceiver":
                InteractableOnHoldReceiver interactableOnHoldReceiver = receiverBase as InteractableOnHoldReceiver;
                return interactableOnHoldReceiver;
            case "InteractableOnPressReceiver":
                InteractableOnPressReceiver interactableOnPressReceiver = receiverBase as InteractableOnPressReceiver;
                return interactableOnPressReceiver;
            case "InteractableOnToggleReceiver":
                InteractableOnToggleReceiver interactableOnToggleReceiver = receiverBase as InteractableOnToggleReceiver;
                return interactableOnToggleReceiver;
            case "InteractableOnTouchReceiver":
                InteractableOnTouchReceiver interactableOnTouchReceiver = receiverBase as InteractableOnTouchReceiver;
                return interactableOnTouchReceiver;*/
        }
    }
    
    private static void HandleInteractableOnFocusReceiver(InteractableOnFocusReceiver interactableOnFocusReceiver,
        ref List<UnityEventInfo> unityEventInfos, int index)
    {
        if (interactableOnFocusReceiver.OnFocusOff!=null)
        {
            unityEventInfos=unityEventInfos.Concat(FindEventsInfo(interactableOnFocusReceiver.OnFocusOff, index, interactableOnFocusReceiver.Name)).ToList();
        }
        if (interactableOnFocusReceiver.OnFocusOn!=null)
        {
            unityEventInfos=unityEventInfos.Concat(FindEventsInfo(interactableOnFocusReceiver.OnFocusOn, index, interactableOnFocusReceiver.Name)).ToList();
        }
    }
}



