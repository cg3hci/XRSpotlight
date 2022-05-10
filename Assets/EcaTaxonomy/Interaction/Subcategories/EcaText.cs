using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcaRules;
using UnityEngine;

/// <summary>
/// <b>ECAText</b> is an <see cref="EcaInteraction"/> subclass that represents a text element.
/// </summary>
[EcaRules4All("text")]
[RequireComponent(typeof(EcaInteraction))] //gerarchia 
[DisallowMultipleComponent]
public class EcaText : MonoBehaviour
{
    /// <summary>
    /// <b>Content</b> is the text content of the text element.
    /// </summary>
    [EcaStateVariable("content", EcaRules4AllType.Text)] public string content;
    
    /// <summary>
    /// <b>ChangesContent</b> is the text content of the text element.
    /// </summary>
    /// <param name="c">The new text content.</param>
    [EcaAction(typeof(EcaText), "changes","content", "to", typeof(string))]
    public void ChangesContent(string c)
    {
        this.content = c;
    }
    
    /// <summary>
    /// <b>Appends</b> adds a string after the current content.
    /// </summary>
    /// <param name="t"> The string to append to. </param>
    [EcaAction(typeof(EcaText), "appends", typeof(string))]
    public void Appends(string t)
    {
        this.content += t;
    }
    
    /// <summary>
    /// <b>Deletes</b> deletes a string from the current content.
    /// </summary>
    /// <param name="t">The substring to replace with blank. </param>
    [EcaAction(typeof(EcaText), "deletes", typeof(string))]
    public void Deletes(string t)
    {
        content = content.Replace (t, ""); // to replace the specific text with blank
    }
}
