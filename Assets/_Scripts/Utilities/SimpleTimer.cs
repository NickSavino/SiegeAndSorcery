using UnityEngine;

/// <summary>
/// Basic Simple Timer Class
/// Two Modes: Countdown, Stopwatch
/// Default: Countdown
/// </summary>
public class SimpleTimer : MonoBehaviour
{
    private float _timerLength;
    private float _timerElapsedTime;
    private float _timerCountDownTimeLeft;

    private bool _isRunning;
    private bool _countDownComplete;

    public TimerMode _currentTimerMode = TimerMode.CountDown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isRunning && _currentTimerMode == TimerMode.CountDown)
        {
            _timerCountDownTimeLeft -= Time.deltaTime;
            if (_timerCountDownTimeLeft <= 0 )
            {
                StopTimer();
                _timerCountDownTimeLeft = 0;
                _countDownComplete = true;
            }
        }

        if (_isRunning && _currentTimerMode == TimerMode.StopWatch)
        {
            _timerElapsedTime += Time.deltaTime;
            StopTimer();
        }
    }

    public void SetTimerLength(float s)
    {
        if (_currentTimerMode == TimerMode.CountDown)
        {
            _timerLength = s;
            _timerCountDownTimeLeft = s;
        }
    }

    public void BeginTimer()
    {
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }

    public void SetTimerMode(TimerMode timerMode)
    {
        _currentTimerMode = timerMode;
    }

    public void ResetElapsedTime()
    {
        _timerElapsedTime = 0;
    }

    public float TimeLeft()
    {
        return _timerCountDownTimeLeft;
    }

    public bool IsCountdownFinished()
    {
        return _countDownComplete && _currentTimerMode == TimerMode.CountDown;
    }
}

public enum TimerMode  { StopWatch, CountDown };