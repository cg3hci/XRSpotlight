using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwTargetDetect : MonoBehaviour
{
    /*private Vector3 startPos;
    private bool targetDetected = false;
    private bool isTimerOn;
    */

    private GameObject pan;
    // Start is called before the first frame update
    void Start()
    {
        pan = GameObject.Find("Pan");
        
    }

    public void TransformEgg()
    {
        pan.transform.GetChild(0).gameObject.SetActive(true);
        Destroy(gameObject);
    }


    /*public void targetDetector()
    {
        //start a timer for 2 seconds, if the correct gameObject is hit then the timer will stop and the target will transform
        //if the timer runs out then the target will return to its original position
        StartCoroutine(targetDetectorTimer());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTimerOn && collision.gameObject.name == "Pan")
        {
            targetDetected = true;
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            collision.gameObject.GetComponent<EggInPanUtils>().isInPan = true;
            Destroy(gameObject);
        }
            
    }

    IEnumerator targetDetectorTimer()
    {
        isTimerOn = true;
        yield return new WaitForSeconds(2);
        isTimerOn = false;
        if (!targetDetected)
        {
            transform.position = startPos;
        }
    }*/

}
