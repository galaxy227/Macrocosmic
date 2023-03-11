using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIUnit : MonoBehaviour
{
    [Header("Title")]
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    [Header("Mask")]
    public RectMask2D Mask;
    public DelayType DelayType;
    private float secondsToFade;

    public Unit Unit
    {
        get { return unit; }
    }
    protected Unit unit;

    protected virtual void OnEnable()
    {
        unit = InputManager.SelectedUnit;

        StartCoroutine(HandleMask());
    }

    // Title
    protected virtual void UpdateCanvas()
    {
        Name.text = unit.Name;
        Description.text = unit.Description;
    }

    // Mask
    private IEnumerator HandleMask()
    {
        // Declare variables
        secondsToFade = DelayHelper.GetDelayTime(DelayType);
        float initialWidth = Mask.gameObject.GetComponent<RectTransform>().sizeDelta.x;
        float minimumPadding = 0f;
        Mask.padding = new Vector4(initialWidth, minimumPadding);
        bool isIncompleteFade = false;

        do
        {
            isIncompleteFade = false;

            float increment = (initialWidth / secondsToFade) * Time.unscaledDeltaTime;
            float newPadding = Mathf.Clamp(Mask.padding.x - increment, minimumPadding, initialWidth);

            Mask.padding = new Vector4(newPadding, minimumPadding);

            if (newPadding > minimumPadding)
            {
                isIncompleteFade = true;
            }

            yield return null;

        } while (isIncompleteFade);

        StopCoroutine(HandleMask());
    }
}
