using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;

/// <summary>
/// <b>Button</b> is an <see cref="Interaction"/> subclass that represents a button.
/// When a <see cref="Button"/> is pressed, it will trigger an event defined by the End User Developer.
/// </summary>
[ECARules4All("button")]
[RequireComponent(typeof(Interaction))] 
[DisallowMultipleComponent]
public class Button : MonoBehaviour
{
    /// <summary>
    /// <b>Presses</b> is a <i>passive</i> function that represents the pressing of the button.
    /// </summary>
    /// <param name="c">The <see cref="Character"/> who presses the button. </param>
    [Action(typeof(Character), "pushes", typeof(Button))]
    public void _Presses(Character c)
    {
        //no need for implementation, see Interactable class (behaviour)
    }
}
