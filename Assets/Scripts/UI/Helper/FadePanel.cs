using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

// Handles animation to fade Image components with OnEnable

public class FadePanel : MonoBehaviour
{
    public DelayType DelayType;
    public bool FadeAllChildren;
    public bool IgnoreAfterFirst; // Do not fade if UIManager.ActiveCanvas is in UIManager.activeCanvasList

    private Image image;
    private float alpha; // original alpha
    private Image[] imageArray;
    private float[] alphaArray; // original alpha
    private float secondsToFade;

    private bool isCoroutineRunning;

    private void Awake()
    {
        if (FadeAllChildren)
        {
            // Get array of children with Image Component
            Image[] array = GetComponentsInChildren<Image>();
            List<Image> imageList = array.ToList();

            // Remove children with FadePanel Component
            for (int i = imageList.Count - 1; i >= 0; i--)
            {
                if (imageList[i].TryGetComponent(out FadePanel fadePanel))
                {
                    imageList.RemoveAt(i);
                }
            }

            // Set ImageArray
            imageArray = imageList.ToArray();

            // Set imageArrayAlpha
            alphaArray = new float[imageArray.Length];

            for (int i = 0; i < imageArray.Length; i++)
            {
                alphaArray[i] = imageArray[i].color.a;
            }
        }
        else if (TryGetComponent(out Image outImage))
        {
            image = outImage;
            alpha = image.color.a;
        }
    }
    private void OnEnable()
    {
        if (IgnoreAfterFirst)
        {
            if (UIManager.Instance.IsActiveCanvasFirstOpen)
            {
                HandleCoroutine();
            }
        }
        else
        {
            HandleCoroutine();
        }
    }

    // Panel
    private void HandleCoroutine()
    {
        if (image != null || imageArray != null)
        {
            if (isCoroutineRunning)
            {
                StopCoroutine("FadeAlpha");
            }

            StartCoroutine("FadeAlpha");
            isCoroutineRunning = true;
        }
    }
    private IEnumerator FadeAlpha()
    {
        ResetAlpha();

        secondsToFade = DelayHelper.GetDelayTime(DelayType);
        bool isIncompleteFade = false;

        if (FadeAllChildren)
        {
            do
            {
                isIncompleteFade = false;

                for (int i = 0; i < imageArray.Length; i++)
                {
                    if (imageArray[i].color.a < alphaArray[i])
                    {
                        float increment = alphaArray[i] * (1f / secondsToFade) * Time.unscaledDeltaTime;
                        float incrementValue = Mathf.InverseLerp(0, alphaArray[i], increment);

                        imageArray[i].color = new Color(imageArray[i].color.r, imageArray[i].color.g, imageArray[i].color.b, Mathf.Clamp(imageArray[i].color.a + incrementValue, 0, alphaArray[i]));

                        if (imageArray[i].color.a < alphaArray[i])
                        {
                            isIncompleteFade = true;
                        }
                    }
                }

                yield return null;

            } while (isIncompleteFade);
        }
        else if (image != null)
        {
            if (image.color.a < alpha)
            {
                do
                {
                    isIncompleteFade = false;

                    float increment = alpha * (1f / secondsToFade) * Time.unscaledDeltaTime;
                    float incrementValue = Mathf.InverseLerp(0, alpha, increment);

                    image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Clamp(image.color.a + incrementValue, 0, alpha));

                    if (image.color.a < alpha)
                    {
                        isIncompleteFade = true;
                    }

                    yield return null;

                } while (isIncompleteFade);
            }
        }

        StopCoroutine("FadeAlpha");
        isCoroutineRunning = false;
    }
    private void ResetAlpha()
    {
        if (FadeAllChildren)
        {
            foreach (Image childImage in imageArray)
            {
                childImage.color = new Color(childImage.color.r, childImage.color.g, childImage.color.b, 0);
            }
        }
        else if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
    }
}
