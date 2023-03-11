using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ToolTip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public RectTransform rectTransform;
    public int characterWrapLimit;

    private void Update()
    {
        SetPosition();
    }

    public void SetText(string content, string header = "")
    {
        // Set Header
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        // Set Content
        contentField.text = content;

        // Update ToolTip Size
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;

        SetPosition();
    }
    private void SetPosition()
    {
        Vector2 position = Input.mousePosition;

        // Offset
        float rectWidth = rectTransform.sizeDelta.x;
        float percentage = 0.375f;
        float x = Input.mousePosition.x - (Screen.width / 2f);

        if (position.x >= Screen.width / 2f)
        {
            float multiplier = Mathf.InverseLerp(Screen.width / 2f, 0, x);
            multiplier = Mathf.Lerp(0f, percentage, multiplier);

            position = new Vector2(position.x - (rectWidth * multiplier), position.y);
        }
        else
        {
            float multiplier = Mathf.InverseLerp(-Screen.width / 2f, 0, x);
            multiplier = Mathf.Lerp(0f, percentage, multiplier);

            position = new Vector2(position.x + (rectWidth * multiplier), position.y);
        }

        // Set ToolTip at position
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}
