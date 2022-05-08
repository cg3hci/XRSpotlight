using System;
using System.Collections;
using System.IO;
using UnityEngine;
using EcaRules;
using ECAScripts.Utils;
using UnityEngine.Networking;
using Behaviour = EcaRules.Behaviour;
using System.IO;
using UnityEngine.Networking;

/// <summary>
/// <b>Sound</b> is a <see cref="Behaviour"/> that works like a media player, and it is specific to audio files.
/// </summary>
[ECARules4All("sound")]
[RequireComponent(typeof(Behaviour))]
[DisallowMultipleComponent]
public class Sound : MonoBehaviour
{
    /// <summary>
    /// <b>Source</b> is the audio source that will be used to play the audio.
    /// </summary>
    [StateVariable("source", ECARules4AllType.Text)] public string source;
    /// <summary>
    /// <b>Volume</b> is the volume of the audio.
    /// </summary>
    [StateVariable("volume", ECARules4AllType.Float)] public float volume;
    /// <summary>
    /// <b>MaxVolume</b> is the maximum volume the audio can reach.
    /// </summary>
    [StateVariable("maxVolume", ECARules4AllType.Float)] public float maxVolume;
    /// <summary>
    /// <b>duration</b> is the duration of the audio.
    /// </summary>
    [StateVariable("duration", ECARules4AllType.Float)] public float duration;
    /// <summary>
    /// <b>currentTime</b> is the current time of the audio.
    /// </summary>
    [StateVariable("currentTime", ECARules4AllType.Float)] public float currentTime;
    /// <summary>
    /// <b>isPlaying</b> is a boolean that indicates if the audio is playing.
    /// </summary>
    [StateVariable("playing", ECARules4AllType.Boolean)] public ECABoolean playing = new ECABoolean(ECABoolean.BoolType.NO);
    /// <summary>
    /// <b> Paused </b> is a boolean that indicates if the audio is paused.
    /// </summary>
    [StateVariable("paused", ECARules4AllType.Boolean)] public ECABoolean paused = new ECABoolean(ECABoolean.BoolType.NO);
    /// <summary>
    /// <b> Stopped </b> is a boolean that indicates if the audio is stopped.
    /// </summary>
    [StateVariable("stopped", ECARules4AllType.Boolean)] public ECABoolean stopped = new ECABoolean(ECABoolean.BoolType.YES);
    /// <summary>
    /// <b>Player</b> is the audio player that will be used to play the audio.
    /// </summary>
    private AudioSource player;
    /// <summary>
    /// <b> SourcePath </b> is the path of the audio file.
    /// </summary>
    private string sourcePath;
    
    /// <summary>
    /// <b>Plays</b> starts the audio.
    /// </summary>
    [Action(typeof(Sound), "plays")]
    public void Plays()
    {
        this.playing.Assign(ECABoolean.BoolType.YES);
        this.stopped.Assign(ECABoolean.BoolType.NO);
        this.paused.Assign(ECABoolean.BoolType.NO);
        player.Play();
    }

    /// <summary>
    /// <b>Pauses</b> pauses the audio.
    /// </summary>
    [Action(typeof(Sound), "pauses")]
    public void Pauses()
    {
        this.playing.Assign(ECABoolean.BoolType.NO);
        this.stopped.Assign(ECABoolean.BoolType.NO);
        this.paused.Assign(ECABoolean.BoolType.YES);
        player.Pause();
    }

    /// <summary>
    /// <b>Stops</b> stops the audio.
    /// </summary>
    [Action(typeof(Sound), "stops")]
    public void Stops()
    {
        this.playing.Assign(ECABoolean.BoolType.NO);
        this.stopped.Assign(ECABoolean.BoolType.YES);
        this.paused.Assign(ECABoolean.BoolType.NO);
        player.Stop();
    }

    /// <summary>
    /// <b>ChangesVolume</b> changes the volume of the audio to the given value.
    /// <para>If the value is greater than the maximum volume, the volume will be set to the maximum volume; if
    /// the value is less than 0, the volume will be set to 0.</para>
    /// </summary>
    /// <param name="v">The new volume setting.</param>
    [Action(typeof(Sound), "changes", "volume", "to", typeof(float))]
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
        player.volume = volume;
    }

    /// <summary>
    /// <b>ChangesCurrentTime</b> changes the current time of the audio to the given value.
    /// </summary>
    /// <param name="c"> The new time value.</param>
    [Action(typeof(Sound), "changes", "current-time", "to", typeof(double))]
    public void ChangesCurrentTime(double c)
    {
        if (c <= duration)
        {
            //TODO: abbastanza esplicativo cosa ci sia da fare
        }
    }

    /// <summary>
    /// <b>ChangesSource</b> changes the source of the audio to the given path.
    /// <p>The path must be relative to the project's user-accessible Inventory folder.</p>
    /// <p>If the path is not valid, the audio will not be played.</p>
    /// </summary>
    /// <param name="newSource">The new audio file path.</param>
    [Action(typeof(Sound), "changes", "source", "to", typeof(string))]
    public void ChangesSource(string newSource)
    {
        source = newSource;
        sourcePath = "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, 
            System.IO.Path.Combine("Inventory", System.IO.Path.Combine("Audios", source)));
        StartCoroutine(ChangeAudioSource());
    }

    IEnumerator ChangeAudioSource()
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(sourcePath, AudioType.MPEG))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError ||
                uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                //throw new Exception("File unavailable or wrong path");
            }
            else
            {
                if (player.clip != null)
                {
                    Stops();
                    AudioClip reference = player.clip;
                    player.clip = null;
                    reference.UnloadAudioData();
                    DestroyImmediate(reference, false);
                }

                player.clip = DownloadHandlerAudioClip.GetContent(uwr);
            }
        }

        yield return null;
    }

    private void Update()
    {
        if (playing)
        {
            currentTime = player.time;
        }
    }

    private void Awake()
    {
        maxVolume = 1.0f;
        player = GetComponent<AudioSource>();

        if (source != null)
        {
            sourcePath = "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, System.IO.Path.Combine("Inventory", System.IO.Path.Combine("Audios", source)));
            StartCoroutine(ChangeAudioSource());
            duration = player.clip.length;
        }

        volume = volume > maxVolume ? maxVolume : volume;
        volume = volume < 0.0f ? 0.0f : volume;
        player.volume = volume;
    }
}