using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Examples.UIRule.Prefabs;

public class HighlightColor : MonoBehaviour
{
    public Text label;
    public Toggle toggle;
    
   
    public void Highlight()
    {
        string phrase = label.text;
        string[] words = phrase.Split(' ');
        Debug.LogFormat("label: {0}    toggle.isOn: {1}", label.text, toggle.isOn);
        GameObject toHighLight = GameObject.Find(words[0]);
        // UIColors colore = new UIColors ();
        // Color colEvent = colore.red;
        Color colEvent = UIColors.red;
        RuleUtils.outlineColor(gameObject, colEvent);
    }
    
    public void OnPointerExit(PointerEventData dt) {
        print("test");
    }
    
}
