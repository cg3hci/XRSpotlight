using System;
using System.Collections;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using ECARules4All.RuleEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Action = ECARules4All.RuleEngine.Action;

public class XRRaycastPointer : MonoBehaviour
{
    public XRRayInteractor ray;

    public Vector3 pos = new Vector3();
    private Vector3 norm = new Vector3();
    private int index = 0;
    private bool validTarget = false;
    public GameObject UIlastSelectedObject;
    private GameObject lastSelectedObject;

    public GameObject LastSelectedObject
    {
        get => lastSelectedObject;
        set => lastSelectedObject = value;
    }

    public Text TextPosition
    {
        get => textPosition;
        set => textPosition = value;
    }

    private Text textObject;

    public GameObject UIlastSelectedPosition;
    private Text textPosition;

    void Start()
    {
        ray = GetComponent<XRRayInteractor>();
        textObject = UIlastSelectedObject.GetComponent<Text>();
        textPosition = UIlastSelectedPosition.GetComponent<Text>();

        textObject.text = "Last selected object";
        textPosition.text = "Last selected position";
    }

    public void GetTargetObject(SelectEnterEventArgs args)
    {
        if (args.interactable.gameObject.GetComponent<ECAObject>() != null)
        {
            Debug.Log("target object " + args.interactable.name);
            lastSelectedObject = args.interactable.gameObject;
            textObject.text = "Object: " + lastSelectedObject.name;
            EventBus.GetInstance().Publish(new Action(gameObject, "interacts with", args.interactable.gameObject)); 
        }
    }

    public void ReleaseTargetObject(SelectExitEventArgs args)
    {
        if (args.interactable.gameObject.GetComponent<ECAObject>() != null)
        {
            Debug.Log("released target object " + args.interactable.name);
            EventBus.GetInstance().Publish(new Action(gameObject, "stops-interacting with", args.interactable.gameObject));
        }
    }

    public void GetRaycastHIt()
    {
        ray.TryGetHitInfo(out pos, out norm, out index, out validTarget);
        Debug.Log("position " + pos.ToString());
        textPosition.text = "Position: " + pos.ToString();
    }
}
