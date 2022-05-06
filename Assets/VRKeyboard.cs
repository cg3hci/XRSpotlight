using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRKeyboard : MonoBehaviour
{
    public UIManager uiManager;
    private InputField activeInputField;
    private bool capsLock = false;

    public void getInputField(InputField i)
    {
        activeInputField = i;
    }


    public void PressKey(GameObject key)
    {
        switch (key.name)
        {
            case "SPACE":
                activeInputField.text += new string(' ', 10);;
                break;
            case "BACKSPACE":
                activeInputField.text =
                    activeInputField.text.Substring(0, activeInputField.text.Length - 1);
                break;
            case "COMMA":
                activeInputField.text =
                    activeInputField.text += ",";
                break;
            case "PERIOD":
                activeInputField.text =
                    activeInputField.text += ".";
                break;
            case "SCORE":
                activeInputField.text =
                    activeInputField.text += "-";
                break;
            case "<":
                uiManager.PressCloseKeyboard();
                break;
            case "SHIFT":
                capsLock = !capsLock;
                if (capsLock)
                {
                    key.GetComponentInChildren<Text>().color = new Color(1, 0.192f, 0, 1f);
                    key.GetComponent<Image>().color = new Color(0,0,0,0.8f);
                }
                else
                {
                    key.GetComponentInChildren<Text>().color = new Color(1, 0.949f, 0.749f, 1f);
                    key.GetComponent<Image>().color = new Color(0,0,0,0.3f);
                }
                break;
            default:
                if (capsLock)
                {
                    activeInputField.text += key.name.ToString().ToUpper();
                }
                else
                {
                    activeInputField.text += key.name;
                }
                break;
        }
    }

    public void PressClose()
    {
    }
}
