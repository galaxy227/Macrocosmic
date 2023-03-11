using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VectorGraphics;

public class TimeBar : MonoBehaviour
{
    [Header("Pause")]
    public GameObject PanelPauseHighlight;
    public SVGImage PauseIcon;
    public Sprite PlaySprite;
    public Sprite PauseSprite;
    [Header("Speed Control")]
    public Sprite ActiveSprite;
    public Sprite InactiveSprite;
    public Button SlowestButton;
    public Button SlowButton;
    public Button NormalButton;
    public Button FastButton;
    public Button FastestButton;
    [Header("Text")]
    public TextMeshProUGUI days;
    public TextMeshProUGUI years;

    public static TimeBar Instance
    {
        get { return instance; }
    }
    private static TimeBar instance;

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
    }
    private void Start()
    {
        OnStart();
    }
    private void Update()
    {
        SetTimeText();
    }

    // Text
    private void SetTimeText()
    {
        days.text = TimeController.Instance.Days.ToString();
        years.text = TimeController.Instance.Years.ToString();
    }

    // Speed Control
    public void SetSpeedControl(SpeedTypeComponent speedTypeComponent)
    {
        if (TimeController.Instance.SpeedType == SpeedType.Paused)
        {
            TimeController.Instance.lastActiveSpeedType = speedTypeComponent.SpeedType;
        }
        else
        {
            TimeController.Instance.SetSpeedType(speedTypeComponent.SpeedType);
        }

        // Speed Control
        UpdateSpeedControl();
    }
    private void UpdateSpeedControl()
    {
        SpeedType speedType;

        if (TimeController.Instance.SpeedType == SpeedType.Paused)
        {
            speedType = TimeController.Instance.lastActiveSpeedType;
        }
        else
        {
            speedType = TimeController.Instance.SpeedType;
        }

        switch (speedType)
        {
            case SpeedType.Slowest:
                SlowestButton.image.sprite = ActiveSprite;
                SlowButton.image.sprite = InactiveSprite;
                NormalButton.image.sprite = InactiveSprite;
                FastButton.image.sprite = InactiveSprite;
                FastestButton.image.sprite = InactiveSprite;
                break;
            case SpeedType.Slow:
                SlowestButton.image.sprite = ActiveSprite;
                SlowButton.image.sprite = ActiveSprite;
                NormalButton.image.sprite = InactiveSprite;
                FastButton.image.sprite = InactiveSprite;
                FastestButton.image.sprite = InactiveSprite;
                break;
            case SpeedType.Normal:
                SlowestButton.image.sprite = ActiveSprite;
                SlowButton.image.sprite = ActiveSprite;
                NormalButton.image.sprite = ActiveSprite;
                FastButton.image.sprite = InactiveSprite;
                FastestButton.image.sprite = InactiveSprite;
                break;
            case SpeedType.Fast:
                SlowestButton.image.sprite = ActiveSprite;
                SlowButton.image.sprite = ActiveSprite;
                NormalButton.image.sprite = ActiveSprite;
                FastButton.image.sprite = ActiveSprite;
                FastestButton.image.sprite = InactiveSprite;
                break;
            case SpeedType.Fastest:
                SlowestButton.image.sprite = ActiveSprite;
                SlowButton.image.sprite = ActiveSprite;
                NormalButton.image.sprite = ActiveSprite;
                FastButton.image.sprite = ActiveSprite;
                FastestButton.image.sprite = ActiveSprite;
                break;
        }
    }

    // Pause UI
    private void SetPauseSprite()
    {
        if (TimeController.Instance.SpeedType == SpeedType.Paused)
        {
            // Pause
            PauseIcon.sprite = PlaySprite;
        }
        else
        {
            // Pause
            PauseIcon.sprite = PauseSprite;
        }
    }
    private void SetPauseButtonHighlight()
    {
        if (TimeController.Instance.SpeedType == SpeedType.Paused)
        {
            PanelPauseHighlight.SetActive(true);
        }
        else
        {
            PanelPauseHighlight.SetActive(false);
        }
    }

    // Events
    private void OnChangeTime()
    {
        UpdateSpeedControl();

        SetPauseSprite();
        SetPauseButtonHighlight();
    }

    // Utility
    private void OnStart()
    {
        TimeController.ChangeTime.AddListener(OnChangeTime);
    }
}
