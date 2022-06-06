using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummerCollectionButton : MonoBehaviour
{
    public GameObject SummerPants, SummerHat, TryShirt;
    // Start is called before the first frame update
    void Start()
    {
        //Summerpants: Vector3(8.98999977,1.26499999,-26.3040009)
        //Hat: Vector3(8.98999977,2.31200004,-26.2530003)
        //TryShirt: Vector3(8.9829998,1.83099997,-26.2989998) Rot: Vector3(0,270.365662,0)
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WearTheMannequinForSummer()
    {
        SummerPants.transform.position = new Vector3(8.98999977F, 1.26499999F, -26.3040009F);
        SummerHat.transform.position = new Vector3(8.98999977F, 2.31200004F, -26.2530003F);
        TryShirt.transform.position = new Vector3(8.9829998F,1.83099997F,-26.2989998F);
        TryShirt.transform.Rotate(0, 270.36F, 0);
    }
}
