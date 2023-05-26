#if MRTK
using Microsoft.MixedReality.Toolkit.UI;
#endif
using UnityEngine;
using XRSpotlightGUI;

[ExecuteInEditMode]
public class InteractionIcon : MonoBehaviour
{
    // an array of game objects which will have a
    // line drawn to in the Scene editor
    public GameObject[] GameObjects;
    public bool ShowInteractions = true;
    
    private void OnEnable()
    {
        /*Interactable[] interactables = GameObject.FindObjectsOfType<Interactable>();
        GameObjects =  new GameObject[interactables.Length];
        int c = 0;
        foreach (var i in interactables)
        {
            GameObjects[c] = i.gameObject;
            c++;
        }*/
        var engine = InferenceEngine.GetInstance();
        GameObjects = engine.FindInteractableObjects();
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
