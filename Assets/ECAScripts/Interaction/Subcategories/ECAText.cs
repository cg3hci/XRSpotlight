using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ECARules4All.RuleEngine;
using UnityEngine;

/// <summary>
/// <b>ECAText</b> is an <see cref="Interaction"/> subclass that represents a text element.
/// </summary>
[ECARules4All("text")]
[RequireComponent(typeof(Interaction))] //gerarchia 
[DisallowMultipleComponent]
public class ECAText : MonoBehaviour
{
    /// <summary>
    /// <b>Content</b> is the text content of the text element.
    /// </summary>
    [StateVariable("content", ECARules4AllType.Text)] public string content;
    
    /// <summary>
    /// <b>ChangesContent</b> is the text content of the text element.
    /// </summary>
    /// <param name="c">The new text content.</param>
    [Action(typeof(ECAText), "changes","content", "to", typeof(string))]
    public void ChangesContent(string c)
    {
        this.content = c;
    }
    
    /// <summary>
    /// <b>Appends</b> adds a string after the current content.
    /// </summary>
    /// <param name="t"> The string to append to. </param>
    [Action(typeof(ECAText), "appends", typeof(string))]
    public void Appends(string t)
    {
        this.content += t;
    }
    
    /// <summary>
    /// <b>Deletes</b> deletes a string from the current content.
    /// </summary>
    /// <param name="t">The substring to replace with blank. </param>
    [Action(typeof(ECAText), "deletes", typeof(string))]
    public void Deletes(string t)
    {
        content = content.Replace (t, ""); // to replace the specific text with blank
    }
}
