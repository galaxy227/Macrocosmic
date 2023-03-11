using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public string header;
    public string content;
    public DelayType DelayType;

    private float timer;
    private float timerThreshold;

    private void OnDisable()
    {
        OnExit();
    }

    // Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine("ShowDelay");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit();
    }
    private void OnExit()
    {
        StopCoroutine("ShowDelay");
        ToolTipController.Hide();
    }

    // Delay Coroutine
    private IEnumerator ShowDelay()
    {
        timer = 0;
        timerThreshold = DelayHelper.GetDelayTime(DelayType);

        do
        {
            timer += Time.unscaledDeltaTime;
            yield return null;

        } while (timer <= timerThreshold);

        ToolTipController.Show(content, header);
    }
    
}

public enum DelayType
{
    None,
    Fast,
    Standard,
    Slow,
}

public static class DelayHelper
{
    public static float GetDelayTime(DelayType delayType)
    {
        float delayTime = 0;

        switch (delayType)
        {
            case DelayType.None:
                delayTime = 0f;
                break;
            case DelayType.Fast:
                delayTime = 0.125f;
                break;
            case DelayType.Standard:
                delayTime = 0.25f;
                break;
            case DelayType.Slow:
                delayTime = 0.5f;
                break;
        }

        return delayTime;
    }
}
