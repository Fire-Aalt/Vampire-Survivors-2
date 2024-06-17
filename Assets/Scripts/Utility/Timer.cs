using System;
using UnityEngine;


public class Timer
{
    public event Action OnTimerDone;

    private float startTime;
    private float duration;
    private float targetTime;

    private readonly bool repeat;

    public bool IsActive { get; private set; }

    public Timer(bool repeat = false)
    {
        this.repeat = repeat;
    }

    public Timer(float duration, bool repeat = false)
    {
        this.duration = duration;
        this.repeat = repeat;
    }

    public void StartTimer()
    {
        IsActive = true;
        startTime = Time.time;
        targetTime = startTime + duration;
    }

    public void StartTimer(float duration)
    {
        this.duration = duration;
        IsActive = true;
        startTime = Time.time;
        targetTime = startTime + duration;
    }

    public void StopTimer()
    {
        IsActive = false;
    }

    public void Tick()
    {
        if (!IsActive) return;

        if (Time.time > targetTime) 
        {
            StopTimer();
            OnTimerDone?.Invoke();
            if (repeat)
            {
                StartTimer();
            }
        }
    }
}

