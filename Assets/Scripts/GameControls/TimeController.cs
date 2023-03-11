using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public enum SpeedType
{
    Paused,
    Slowest, // 1 Day per Second
    Slow,
    Normal,
    Fast,
    Fastest
}

public class TimeController : MonoBehaviour
{
    public SpeedType SpeedType;
    public SpeedType lastActiveSpeedType;
    private SpeedTypeComponent speedTypeComponent;

    // TIME FOR GAME
    public float customDeltaTime;
    private float customTimeScale;

    // Time values
    private int days;
    public int Days
    {
        get { return days; }
    }
    private int years;
    public int Years
    {
        get { return years; }
    }

    // Time rate
    private const float DayRate = 1f; // How many seconds per day

    // Time left until next increment
    private float customTimer;

    // Events
    public static UnityEvent ChangeTime;
    public static TimeController Instance
    {
        get { return instance; }
    }
    private static TimeController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        OnAwake();
    }
    private void Start()
    {
        OnStart();
    }
    private void Update()
    {
        customDeltaTime = Time.unscaledDeltaTime * customTimeScale;

        UpdateTimeValues();
    }

    // Update Time Values
    public void UpdateTimeValues()
    {
        // Date
        if (SpeedType != SpeedType.Paused)
        {
            customTimer += customDeltaTime;

            if (customTimer >= DayRate)
            {
                int daysPassed = (int)System.Math.Floor(customTimer / DayRate);
                float remainder = customTimer - (DayRate * daysPassed);

                days += daysPassed;
                customTimer = remainder;
            }

            if (days == 366)
            {
                years++;
                days = 1;
            }
            else if (days > 366)
            {
                int remainder = days % 365;
                days = days - remainder;
                int yearsPassed = days / 365;

                years += yearsPassed;
                days = remainder;
            }
        }
    }

    // Change SpeedType
    public void TogglePause()
    {
        if (SpeedType != SpeedType.Paused)
        {
            lastActiveSpeedType = SpeedType;
            SpeedType = SpeedType.Paused;
        }
        else
        {
            SpeedType = lastActiveSpeedType;
        }

        SetSpeedType(SpeedType);
    }
    public void IncrementSpeedType(bool increase)
    {
        int value = 0;

        if (increase) // Increase
        {
            value = 1;
        }
        else // Decrease
        {
            value = -1;
        }

        List<SpeedType> timeSpeedList = System.Enum.GetValues(typeof(SpeedType)).Cast<SpeedType>().ToList();

        //index = (int)SpeedType + value;
        //index = Mathf.Clamp(index, 1, timeSpeedList.Count - 1);
        //SetSpeedType(timeSpeedList[index]);

        int index = 0;

        if (SpeedType == SpeedType.Paused)
        {
            index = (int)lastActiveSpeedType + value;
            index = Mathf.Clamp(index, 1, timeSpeedList.Count - 1);
        }
        else
        {
            index = (int)SpeedType + value;
            index = Mathf.Clamp(index, 1, timeSpeedList.Count - 1);
        }

        speedTypeComponent.SpeedType = timeSpeedList[index];
        TimeBar.Instance.SetSpeedControl(speedTypeComponent);
    }

    // Set TimeScale
    public void SetSpeedType(SpeedType typeOfSpeed)
    {
        SpeedType = typeOfSpeed;

        if (SpeedType == SpeedType.Paused)
        {
            customTimeScale = GetTimeRate(SpeedType);
        }
        else if (SpeedType == SpeedType.Slowest)
        {
            customTimeScale = GetTimeRate(SpeedType);
        }
        else if (SpeedType == SpeedType.Slow)
        {
            customTimeScale = GetTimeRate(SpeedType);
        }
        else if (SpeedType == SpeedType.Normal)
        {
            customTimeScale = GetTimeRate(SpeedType);
        }
        else if (SpeedType == SpeedType.Fast)
        {
            customTimeScale = GetTimeRate(SpeedType);
        }
        else if (SpeedType == SpeedType.Fastest)
        {
            customTimeScale = GetTimeRate(SpeedType);
        }

        ChangeTime.Invoke();
    }
    private float GetTimeRate(SpeedType speedType)
    {
        float timeRate = 0;

        if (speedType == SpeedType.Paused)
        {
            timeRate = DayRate * 0f;
        }
        else if (speedType == SpeedType.Slowest)
        {
            timeRate = DayRate;
        }
        else if (speedType == SpeedType.Slow)
        {
            timeRate = DayRate * 2f;
        }
        else if (speedType == SpeedType.Normal)
        {
            timeRate = DayRate * 4f;
        }
        else if (speedType == SpeedType.Fast)
        {
            timeRate = DayRate * 8f;
        }
        else if (speedType == SpeedType.Fastest)
        {
            timeRate = DayRate * 16f;
        }

        return timeRate;
    }

    // Utility
    private void OnAwake()
    {
        if (ChangeTime == null)
        {
            ChangeTime = new UnityEvent();
        }
    }
    private void OnStart()
    {
        SetSpeedType(SpeedType.Fastest);

        speedTypeComponent = GetComponent<SpeedTypeComponent>();

        // Events
        GalaxyGenerator.AfterGenerate.AddListener(OnGenerate);
    }
    private void OnGenerate()
    {
        if (SpeedType == SpeedType.Paused)
        {
            SetSpeedType(lastActiveSpeedType);
        }

        ResetTime();
    }
    private void ResetTime()
    {
        lastActiveSpeedType = SpeedType.Slowest; // Default
        SetSpeedType(SpeedType.Paused); // Pause

        customTimer = 0;
        customDeltaTime = 0;

        days = 1;
        years = 1;
    }
    public void SetDate(int daysValue, int yearsValue)
    {
        days = daysValue;
        years = yearsValue;
    }
}