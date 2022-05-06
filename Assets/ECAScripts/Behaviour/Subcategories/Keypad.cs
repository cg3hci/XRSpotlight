using System;
using UnityEngine;
using ECARules4All.RuleEngine;
using Behaviour = ECARules4All.RuleEngine.Behaviour;

/// <summary>
/// <b>Keypad</b> is a <see cref="ECARules4All.RuleEngine.Behaviour"/> that lets an object to receive codes and trigger
/// actions when the code is correct.
/// </summary>
[ECARules4All("keypad")]
[RequireComponent(typeof(Behaviour))]
[DisallowMultipleComponent]
public class Keypad : MonoBehaviour
{
    /// <summary>
    /// <b>Keycode</b> is the code that the keypad will accept.
    /// </summary>
    [StateVariable("keycode", ECARules4AllType.Text)]
    public string keycode;
    /// <summary>
    /// <b>Input</b> is the input that the keypad is currently holding.
    /// </summary>
    [StateVariable("input", ECARules4AllType.Text)]
    public string input;

    /// <summary>
    /// <b>Inserts</b> inserts the whole input into the <see cref="input"/> variable.
    /// </summary>
    /// <param name="input"> The complete code to be checked </param>
    [Action(typeof(Keypad), "inserts", typeof(string))]
    public void Inserts(string input)
    {
        this.input = input;
    }
    
    /// <summary>
    /// <b>Adds</b> adds a single character to the <see cref="input"/> variable.
    /// </summary>
    /// <param name="input"></param>
    //TODO: verb not present in grammar
    [Action(typeof(Keypad), "adds", typeof(string))]
    public void Add(string input)
    {
        this.input += input;
    }
    
    /// <summary>
    /// <b>Resets</b> clears the <see cref="input"/> variable.
    /// </summary>
    [Action(typeof(Keypad), "resets")]
    public void Resets()
    {
        input = "";
    }
}