using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ECARules4All.RuleEngine;
using ECAScripts.Utils;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// <b>ECAVideo</b> is an <see cref="Interaction"/> that represents a video player.
/// </summary>
[ECARules4All("video")]
[RequireComponent(typeof(Interaction), typeof(VideoPlayer))] //gerarchia 
[DisallowMultipleComponent]
public class ECAVideo : MonoBehaviour
{
    /// <summary>
    /// <b>Source</b> is the video source.
    /// </summary>
    [StateVariable("source", ECARules4AllType.Text)]
    public string source;

    /// <summary>
    /// <b>Volume</b> is the video volume.
    /// </summary>
    [StateVariable("volume", ECARules4AllType.Float)]
    public float volume;

    /// <summary>
    /// <b> MaxVolume</b> is the video max volume.
    /// </summary>
    [StateVariable("maxVolume", ECARules4AllType.Float)]
    public float maxVolume = 1.0f;

    /// <summary>
    /// <b>Duration</b> is the video duration.
    /// </summary>
    //TODO: duration e currentTime, abbiamo cambiato il tipo da time a double
    [StateVariable("duration", ECARules4AllType.Float)]
    public double duration;

    /// <summary>
    /// <b>CurrentTime</b> is the video current time.
    /// </summary>
    [StateVariable("current-time", ECARules4AllType.Float)]
    public double currentTime;

    /// <summary>
    /// <b>Playing</b> defines whether the video is playing.
    /// </summary>
    [StateVariable("playing", ECARules4AllType.Boolean)]
    public ECABoolean playing = new ECABoolean(ECABoolean.BoolType.NO);

    /// <summary>
    /// <b>Paused</b> defines whether the video is paused.
    /// </summary>
    [StateVariable("paused", ECARules4AllType.Boolean)]
    public ECABoolean paused = new ECABoolean(ECABoolean.BoolType.NO);

    /// <summary>
    /// <b>Stopped</b> defines whether the video is stopped.
    /// </summary>
    [StateVariable("stopped", ECARules4AllType.Boolean)]
    public ECABoolean stopped = new ECABoolean(ECABoolean.BoolType.YES);

    /// <summary>
    /// <b>Player</b> is the video player to control through the script.
    /// </summary>
    private VideoPlayer player;

    /// <summary>
    /// <b>Plays</b> starts the video.
    /// </summary>
    [Action(typeof(ECAVideo), "plays")]
    public void Plays()
    {
        playing.Assign(ECABoolean.BoolType.YES);
        this.stopped.Assign(ECABoolean.BoolType.NO);
        this.paused.Assign(ECABoolean.BoolType.NO);
        player.Play();
    }

    /// <summary>
    /// <b>Pauses</b> pauses the video.
    /// </summary>
    [Action(typeof(ECAVideo), "pauses")]
    public void Pauses()
    {
        this.playing.Assign(ECABoolean.BoolType.NO);
        this.stopped.Assign(ECABoolean.BoolType.NO);
        this.paused.Assign(ECABoolean.BoolType.YES);
        player.Pause();
    }

    /// <summary>
    /// <b>Stops</b> stops the video.
    /// </summary>
    [Action(typeof(ECAVideo), "stops")]
    public void Stops()
    {
        this.playing.Assign(ECABoolean.BoolType.NO);
        this.stopped.Assign(ECABoolean.BoolType.YES);
        this.paused.Assign(ECABoolean.BoolType.NO);
        currentTime = 0;
        player.Stop();
    }

    /// <summary>
    /// <b>ChangesVolume</b> changes the video volume to the given value.
    /// If the value is greater than the max volume, the volume is set to the max volume.
    /// If the value is lower than 0, the volume is set to 0.
    /// </summary>
    /// <param name="v">The new video volume. </param>
    [Action(typeof(ECAVideo), "changes", "volume", "to", typeof(float))]
    public void ChangesVolume(float v)
    {
        if (v > maxVolume)
        {
            v = maxVolume;
        }

        if (v < 0)
        {
            v = 0;
        }

        volume = v;
        player.SetDirectAudioVolume(0, volume);
        //trackindex is set to 0, but there may be more than 1 audio track
    }

    /// <summary>
    /// <b>ChangesCurrentTime</b> changes the video current time to the given value.
    /// </summary>
    /// <param name="c">The new video current time. </param>
    //TODO: possibile conflitto tra grammatica e chiamata di funzione, abbiamo messo il trattino in current time
    [Action(typeof(ECAVideo), "changes", "current-time", "to", typeof(double))]
    public void ChangesCurrentTime(double c)
    {
        if (c <= duration)
        {
            var frameRate = player.frameRate;
            var seek = (frameRate * c);
            player.frame = (long) (seek);
        }
    }

    /// <summary>
    /// <b>ChangesSource</b> changes the video source to the given value.
    /// The new path must be relative to the user-accessible Inventory folder.
    /// </summary>
    /// <param name="newSource">The path for the new video file.</param>
    [Action(typeof(ECAVideo), "changes", "source", "to", typeof(string))]
    public void ChangesSource(string newSource)
    {
        source = newSource;
        player.url = "file://" + Path.Combine(Application.streamingAssetsPath, Path.Combine("Inventory", Path.Combine("Videos", source)));
        duration = player.length;
    }

    private void Update()
    {
        if (playing)
        {
            currentTime = (float) player.time;
        }
    }

    private void Awake()
    {
        maxVolume = 1.0f;
        player = GetComponent<VideoPlayer>();
        if (source != "")
        {
            player.url = "file://" + Path.Combine(Application.streamingAssetsPath, Path.Combine("Inventory", Path.Combine("Videos", source)));
            duration = player.length;
        }
        volume = volume > maxVolume ? maxVolume : volume;
        volume = volume < 0.0f ? 0.0f : volume;
        player.SetDirectAudioVolume(player.audioTrackCount, volume);
    }
}