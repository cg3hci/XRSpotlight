using System;
using UnityEngine;
using EcaRules;
using Behaviour = EcaRules.Behaviour;

/// <summary>
/// <b>Timer</b> is a <see cref="Behaviour"/> that can be used to trigger an action after a certain amount of time.
/// </summary>
[ECARules4All("timer")]
[RequireComponent(typeof(Behaviour))]
[DisallowMultipleComponent]
public class Timer : MonoBehaviour
{
    /// <summary>
    /// <b>Duration</b> is the maximum time the timer will run for.
    /// </summary>
    [StateVariable("duration", ECARules4AllType.Float)] public float duration;
    /// <summary>
    /// <b>Current</b> is the current time the timer has been running for.
    /// </summary>
    [StateVariable("current-time", ECARules4AllType.Float)] public float current;
    /// <summary>
    /// <b>Active</b> is a boolean that indicates whether the timer is currently running.
    /// </summary>
    private bool active;
    /// <summary>
    /// <b>Elapsed</b> counts the time elapsed since the timer was started.
    /// </summary>
    private int elapsed;

    /// <summary>
    /// <b>ChangesDuration</b> sets the duration of the timer.
    /// </summary>
    /// <param name="amount">The new maximum time the timer will run for. </param>
    [Action(typeof(Timer), "changes","duration", "to", typeof(float))]
    public void ChangeDuration(float amount)
    {
        if (amount > 0.0f)
            duration = amount;
    }
    
    /// <summary>
    /// <b>ChangeCurrentTime</b> sets the current time of the timer.
    /// </summary>
    /// <param name="amount"> The new </param>
    [Action(typeof(Timer), "changes","current-time", "to", typeof(float))]
    public void ChangeCurrentTime(float amount)
    {
        if (amount < 0.0f)
            current = 0.0f;
        else if (amount > duration)
            current = duration;
        else current = amount;
        
    }

    /// <summary>
    /// <b>Starts</b> starts the timer.
    /// </summary>
    [Action(typeof(Timer), "starts")]
    public void Starts()
    {
        active = true;
    }

    /// <summary>
    /// <b>Stops</b> stops the timer.
    /// </summary>
    [Action(typeof(Timer), "stops")]
    public void Stops()
    {
        active = false;
    }
    
    
    //TODO: verb "pauses" present in grammar but no function in documentation
    /// <summary>
    /// <b>Pauses</b> pauses the timer.
    /// </summary>
    [Action(typeof(Timer), "pauses")]
    public void Pauses()
    {
        active = false;
    }
    
    /// <summary>
    /// <b>Elapses</b> emits an event when the timer has elapsed a set amount of time.
    /// </summary>
    /// <param name="seconds">The amount of time elapsed from the start.</param>
    [Action(typeof(Timer), "elapses-timer", typeof(int))]
    public void Elapses(int seconds)
    {
        EcaEventBus.GetInstance().Publish(new EcaAction(this.gameObject, "elapses-timer", seconds));
    }
    
    /// <summary>
    /// <b>Reaches</b> emits an event when the timer has reached a set amount of time.
    /// </summary>
    /// <param name="seconds">The amount of time that the time have to reach in order to trigger an event.</param>
    [Action(typeof(Timer), "reaches", typeof(int))]
    public void Reaches(int seconds)
    {
        EcaEventBus.GetInstance().Publish(new EcaAction(this.gameObject, "reaches", seconds));
    }

    /// <summary>
    /// <b>Resets</b> resets the timer to the maximum duration.
    /// </summary>
    [Action(typeof(Timer), "resets")]
    public void Resets()
    {
        active = false;
        current = duration;
    }

    
    private void Update()
    {
        if (active)
        {
            current -= Time.deltaTime;
            if (current < 0)
            {
                active = false;
                current = 0;
            }

            elapsed = (int)(duration - current);
            Elapses(elapsed);
            Reaches((int)current);
        }
    }
}