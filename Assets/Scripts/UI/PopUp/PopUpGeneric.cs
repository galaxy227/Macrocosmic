using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PopUpGeneric helper to provide logic for PopUp prefabs

public class PopUpGeneric : MonoBehaviour
{
    // Buttons
    public void Back()
    {
        UIManager.Instance.BackToLastActiveCanvas();
    }
}
