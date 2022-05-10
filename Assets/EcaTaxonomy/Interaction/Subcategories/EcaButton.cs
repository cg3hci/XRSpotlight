using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;

/// <summary>
/// <b>Button</b> is an <see cref="EcaInteraction"/> subclass that represents a button.
/// When a <see cref="EcaButton"/> is pressed, it will trigger an event defined by the End User Developer.
/// </summary>
[EcaRules4All("button")]
[RequireComponent(typeof(EcaInteraction))] 
[DisallowMultipleComponent]
public class EcaButton : MonoBehaviour
{
    /// <summary>
    /// <b>Presses</b> is a <i>passive</i> function that represents the pressing of the button.
    /// </summary>
    /// <param name="c">The <see cref="EcaCharacter"/> who presses the button. </param>
    [EcaAction(typeof(EcaCharacter), "pushes", typeof(EcaButton))]
    public void _Presses(EcaCharacter c)
    {
        //no need for implementation, see Interactable class (behaviour)
    }
}
