using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggInPan : MonoBehaviour
{
    /*public bool isInPan = false;
    public bool isCooked = false;
    
    public bool isBeingDragged = false;
    
    public void drag()
    {
        isBeingDragged = true;
    }
    
    public void drop()
    {
        isBeingDragged = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isBeingDragged && isInPan && isCooked && collision.gameObject.transform.name == "Plate")
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            isInPan = false;
        }
    }*/
    
    private AudioSource fryingSound;
    private GameObject smoke;
    // Start is called before the first frame update
    void Start()
    {
        fryingSound = this.GetComponent<AudioSource>();
        fryingSound.Stop();
        smoke = this.transform.GetChild(1).gameObject;
        smoke.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void FryEgg()
    {
        fryingSound.Play();
        smoke.SetActive(true);
    }
    
    public void StopSmoke()
    {
        smoke.SetActive(false);
    }
}
