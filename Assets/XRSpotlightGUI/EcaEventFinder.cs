using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class EcaEventFinder : MonoBehaviour
{
    class UnityEventInfo
    {
        private MethodInfo method;
        private GameObject gameObjectScript;
        private string typeOfEvent;
        private string scriptContainingTheEvent;
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

    void FindInteractableEventsByGameObject(GameObject gameObject)
    {
        List<UnityEventInfo> unityEventInfos = new List<UnityEventInfo>();
        
        //onclick events
        UnityEvent onClick = gameObject.GetComponent<Interactable>().OnClick;
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
            if (receiverBase.GetType().Name == "InteractableOnFocusReceiver")
            {
                InteractableOnFocusReceiver interactableReceiver = receiverBase as InteractableOnFocusReceiver;
                if (interactableReceiver.OnFocusOff!=null)
                {
                    unityEventInfos=unityEventInfos.Concat(FindEventsInfo(interactableReceiver.OnFocusOff, i, receivers[i].Name)).ToList();
                }
                if (interactableReceiver.OnFocusOn!=null)
                {
                    unityEventInfos=unityEventInfos.Concat(FindEventsInfo(interactableReceiver.OnFocusOn, i, receivers[i].Name)).ToList();
                }
            }
            // unityEventInfos=unityEventInfos.Concat(FindEventsInfo(receivers[i].Event, i, receivers[i].Name)).ToList();
        }
        
        Debug.Log(unityEventInfos);
    }

    private List<UnityEventInfo> FindEventsInfo(UnityEvent unityEvent, int index, string eventType="OnClick")
    {
        List<UnityEventInfo> result = new List<UnityEventInfo>();
        Object target = unityEvent.GetPersistentTarget(index);
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

    private UnityEventInfo FindSingleEventInfo(string methodName, MonoBehaviour component, GameObject gameObjectParameter, string eventType)
    {
        MethodInfo methodInfo = component.GetType().GetMethod(methodName);
        if (methodInfo != null)
        {
            return new UnityEventInfo(methodInfo, gameObjectParameter, eventType, component.GetType().Name);
        }
        return null;
    }
}
