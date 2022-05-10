using System;
using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

/// <summary>
/// <b>ECAImage</b> is an <see cref="EcaInteraction"/> subclass that represents an image.
/// </summary>
[EcaRules4All("image")]
[RequireComponent(typeof(EcaInteraction))] //gerarchia 
[DisallowMultipleComponent]
public class EcaImage : MonoBehaviour
{
    /// <summary>
    /// <b>Source</b> is the source of the image.
    /// </summary>
    [EcaStateVariable("source", EcaRules4AllType.Text)] public string source;
    /// <summary>
    /// <b>Width</b> is the width of the image.
    /// </summary>
    [EcaStateVariable("width", EcaRules4AllType.Integer)] public int width;
    /// <summary>
    /// <b>Height</b> is the height of the image.
    /// </summary>
    [EcaStateVariable("height", EcaRules4AllType.Integer)] public int height;
    private RectTransform reference;
    private RawImage imageSrc;
    private string sourcePath;
    private void Start()
    {
        reference = GetComponent<RectTransform>();
        imageSrc = GetComponent<RawImage>();
        if (source != "")
        {
            sourcePath = "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, System.IO.Path.Combine("Inventory", System.IO.Path.Combine("Images", source)));
            StartCoroutine(ChangeImageSource());
        }
        width = (int) reference.rect.width;
        height = (int) reference.rect.height;
    }

    /// <summary>
    /// <b> ChangesWidth </b> changes the width of the image.
    /// </summary>
    /// <param name="newWidth">The new value of the width. </param>
    [EcaAction(typeof(EcaImage), "changes", "width", "to", typeof(int))]
    public void ChangesWidth(int newWidth)
    {
        if (newWidth > 0)
            width = newWidth;
    }
    
    /// <summary>
    /// <b>ChangesSource</b> changes the source of the image.
    /// The path of the new source is relative to the user-accessible Inventory folder.
    /// </summary>
    /// <param name="newSource">The path of the new image. </param>
    [EcaAction(typeof(EcaImage), "changes", "source", "to", typeof(string))]
    public void ChangesSource(string newSource)
    {
        source = newSource;
        sourcePath = "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, 
            System.IO.Path.Combine("Inventory", System.IO.Path.Combine("Images", source)));
        StartCoroutine(ChangeImageSource());
    }

    IEnumerator ChangeImageSource()
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(sourcePath))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError ||
                uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                //throw new Exception("File unavailable or wrong path " + sourcePath);
            }
            else
            {
                imageSrc.texture = DownloadHandlerTexture.GetContent(uwr);
            }
        }
        yield return null;
    }

    /// <summary>
    /// <b>ChangesHeight</b> changes the height of the image.
    /// </summary>
    /// <param name="newHeight">The new value of the height. </param>
    [EcaAction(typeof(EcaImage), "changes", "height", "to", typeof(int))]
    public void ChangesHeight(int newHeight)
    {
        if (newHeight > 0)
            height = newHeight;
    }

}