using System.Reflection;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

public class EcaEventFinder : MonoBehaviour
{
    // Start is called before the first frame update
    class ListenerInfo
    {
         MethodInfo listener, reinstantor;
          Component component;
    }
    void Start()
    {
        GameObject vanxVintage = GameObject.Find("VanxVintage");
        /*List<InteractableEvent> events = vanxVintage.GetComponent<Interactable>().InteractableEvents;
        
        for(int i=0; i<events.Count; i++)
        {
            InteractableEvent ev = events[i];
            UnityEvent e = ev.Event;
            int eCount = e.GetPersistentEventCount();
            for(int j=0;j<eCount;j++)
            {
                Object targetE = e.GetPersistentTarget(i);
                GameObject target = GameObject.Find(targetE.name);
                string methodNameE = e.GetPersistentMethodName(i);
                MonoBehaviour[] componentsE = target.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour component in componentsE)
                {
                    MethodInfo methodInfo = component.GetType().GetMethod(methodNameE);
                    if (methodInfo != null)
                    {
                        Debug.Log("Found method " + methodNameE + " on " + component.GetType().Name);
                    }
                }
            }
        }
        
        
        //On click normale
        //aggiunta listener runtime
        //vanxVintage.GetComponent<Interactable>().OnClick.AddListener(() => {    Debug.Log("click add listener"); });
        */
        
        //UnityEventBase v = vanxVintage.GetComponent<Interactable>().OnClick.GetPersistentListenerState();
        
        UnityEvent onClicko = vanxVintage.GetComponent<Interactable>().OnClick;
        var r = vanxVintage.GetComponent<Interactable>().GetReceivers<ReceiverBase>();
        int eventCount = onClicko.GetPersistentEventCount();
        for (int i = 0; i < eventCount; i++)
        {
            Object target = onClicko.GetPersistentTarget(i);
            GameObject gameObject = GameObject.Find(target.name);
            string methodName = onClicko.GetPersistentMethodName(i);
            MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                MethodInfo methodInfo = component.GetType().GetMethod(methodName);
                if (methodInfo != null)
                {
                    Debug.Log("Found method " + methodName + " on " + component.GetType().Name);
                }
            }
        }
        
        //ricerca dei metodi:
        
        
        //Debug.Log(events.Count);
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void showTag()
    {
        GameObject.Find("infoBlackShoes").GetComponent<Canvas>().enabled = true;
        
    }
    
    public void showTagVanx()
    {
        GameObject.Find("infoBlackVanx").GetComponent<Canvas>().enabled = true;
        
    }
    
    

    public void onClickNormale()
    {
        Debug.Log("Questa è una onClick normale");
    }   
    public void onClickNormale2()
    {
        Debug.Log("Questa è una onClick normale 2");
    }
    
    public void onClickReceiver()
    {
        Debug.Log("Questa è una onClick receiver");
    }

    public void onFocusReceiver()
    {
        Debug.Log("On focus receiver");
    }
}
