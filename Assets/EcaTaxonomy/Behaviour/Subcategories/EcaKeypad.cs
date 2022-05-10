using System;
using UnityEngine;
using EcaRules;

/// <summary>
/// <b>Keypad</b> is a <see cref="EcaBehaviour"/> that lets an object to receive codes and trigger
/// actions when the code is correct.
/// </summary>
[EcaRules4All("keypad")]
[RequireComponent(typeof(EcaBehaviour))]
[DisallowMultipleComponent]
public class EcaKeypad : MonoBehaviour
{
    /// <summary>
    /// <b>Keycode</b> is the code that the keypad will accept.
    /// </summary>
    [EcaStateVariable("keycode", EcaRules4AllType.Text)]
    public string keycode;
    /// <summary>
    /// <b>Input</b> is the input that the keypad is currently holding.
    /// </summary>
    [EcaStateVariable("input", EcaRules4AllType.Text)]
    public string input;

    /// <summary>
    /// <b>Inserts</b> inserts the whole input into the <see cref="input"/> variable.
    /// </summary>
    /// <param name="input"> The complete code to be checked </param>
    [EcaAction(typeof(EcaKeypad), "inserts", typeof(string))]
    public void Inserts(string input)
    {
        this.input = input;
    }
    
    /// <summary>
    /// <b>Adds</b> adds a single character to the <see cref="input"/> variable.
    /// </summary>
    /// <param name="input"></param>
    //TODO: verb not present in grammar
    [EcaAction(typeof(EcaKeypad), "adds", typeof(string))]
    public void Add(string input)
    {
        this.input += input;
    }
    
    /// <summary>
    /// <b>Resets</b> clears the <see cref="input"/> variable.
    /// </summary>
    [EcaAction(typeof(EcaKeypad), "resets")]
    public void Resets()
    {
        input = "";
    }
}