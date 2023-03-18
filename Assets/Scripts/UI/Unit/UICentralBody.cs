using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UICentralBody : UIUnit
{
    [Header("Summary")]
    public TextMeshProUGUI Size;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (unit != null)
        {
            UpdateCanvas();
        }
    }

    protected override void UpdateCanvas()
    {
        base.UpdateCanvas();

        UpdateSummary();
    }

    // Summary
    private void UpdateSummary()
    {
        if (unit is CentralBody centralBody)
        {
            Size.text = SetSizeTypeText(centralBody);
        }
    }
    private string SetSizeTypeText(CentralBody centralBody)
    {
        int value = (int)centralBody.SizeType;
        string result = System.Enum.GetName(typeof(SizeType), value);

        return result;
    }
}

//public class UICentralBody : MonoBehaviour
//{
//    // Title
//    public TextMeshProUGUI Name;
//    public TextMeshProUGUI Description;
//    // Summary
//    public TextMeshProUGUI Size;
//    [Header("Mask")]
//    public RectMask2D Mask;
//    public DelayType DelayType;
//    private float secondsToFade;

//    private void OnEnable()
//    {
//        StartCoroutine(HandleMask());
//        UpdateCentralBodyPanel();
//    }

//    private void UpdateCentralBodyPanel()
//    {
//        UpdateText();
//    }

//    // Text
//    private void UpdateText()
//    {
//        if (InputManager.SelectedUnit is Star)
//        {
//            Star star = (Star)InputManager.SelectedUnit;

//            // Name
//            Name.text = star.Name;

//            // Description
//            string starType = SetStarTypeText(star);
//            Description.text = starType + " " + "Star";

//            // Size
//            Size.text = SetSizeTypeText(star);
//        }
//        else if (InputManager.SelectedUnit is BlackHole)
//        {
//            BlackHole blackHole = (BlackHole)InputManager.SelectedUnit;

//            // Name
//            Name.text = blackHole.Name;

//            // Description
//            Description.text = "Black Hole";

//            // Size
//            Size.text = SetSizeTypeText(blackHole);
//        }
//    }
//    // Text Helpers
//    private string SetStarTypeText(Star star)
//    {
//        string result = "";

//        if (star.starType == Star.StarType.Blue)
//        {
//            result = "Hot";
//        }
//        else if (star.starType == Star.StarType.White)
//        {
//            result = "Warm";
//        }
//        else if (star.starType == Star.StarType.Yellow)
//        {
//            result = "Mild";
//        }
//        else if (star.starType == Star.StarType.Orange)
//        {
//            result = "Cool";
//        }
//        else if (star.starType == Star.StarType.Red)
//        {
//            result = "Cold";
//        }

//        return result;
//    }
//    private string SetSizeTypeText(CentralBody centralBody)
//    {
//        int value = (int)centralBody.size.sizeType;
//        string result = System.Enum.GetName(typeof(Celestial.Size.SizeType), value);

//        return result;
//    }
//    // Mask
//    private IEnumerator HandleMask()
//    {
//        // Declare variables
//        secondsToFade = DelayHelper.GetDelayTime(DelayType);
//        float initialWidth = Mask.gameObject.GetComponent<RectTransform>().sizeDelta.x;
//        float minimumPadding = 0f;
//        Mask.padding = new Vector4(initialWidth, minimumPadding);
//        bool isIncompleteFade = false;

//        do
//        {
//            isIncompleteFade = false;

//            float increment = (initialWidth / secondsToFade) * Time.unscaledDeltaTime;
//            float newPadding = Mathf.Clamp(Mask.padding.x - increment, minimumPadding, initialWidth);

//            Mask.padding = new Vector4(newPadding, minimumPadding);

//            if (newPadding > minimumPadding)
//            {
//                isIncompleteFade = true;
//            }

//            yield return null;

//        } while (isIncompleteFade);

//        StopCoroutine(HandleMask());
//    }
//}
