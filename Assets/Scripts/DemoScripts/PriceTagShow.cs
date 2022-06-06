using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriceTagShow : MonoBehaviour
{
    public GameObject priceTag;
    // Start is called before the first frame update
    void Start()
    {
        priceTag.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPriceTag()
    {
        priceTag.SetActive(true);
    }
    
    public void HidePriceTag()
    {
        priceTag.SetActive(false);
    }
}
