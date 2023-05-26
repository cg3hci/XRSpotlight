using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TurnOnTV : MonoBehaviour
{
    public GameObject tv;

    private VideoPlayer videoPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnOnTv()
    {
        videoPlayer = tv.AddComponent<VideoPlayer>();
        videoPlayer.url = "Assets/Resources/run.mp4";
        videoPlayer.Play();
    }
    
    public void TurnOffTv()
    {
        videoPlayer.Pause();
    }
}
