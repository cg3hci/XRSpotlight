using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class IconViewer : MonoBehaviour
{
    private RawImage image;
    private Object[] dati;
    private int i;
    private int numItems;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<RawImage>();
        //Working Resources procedure
        /*
        dati = Resources.LoadAll("Inventory\\Meshes\\");
        numItems = dati.Length;
        Debug.Log("Objects number: " + numItems);
        image.texture = AssetPreview.GetAssetPreview(dati[0]);
        */
        image.texture = AssetPreview.GetAssetPreview(prefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            i = (i + 1) % numItems;
            image.texture = AssetPreview.GetAssetPreview(dati[i]);
        }
        else if (Input.GetButtonDown("Submit"))
        {
            i--;
            if (i < 0)
                i = numItems - 1;
            image.texture = AssetPreview.GetAssetPreview(dati[i]);
        }
    }
}
