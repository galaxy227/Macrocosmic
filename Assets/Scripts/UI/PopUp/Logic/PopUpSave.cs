using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;

// PopUp for MiniMenu Canvas, MiniMenu.cs

public class PopUpSave : MonoBehaviour
{
    public TMP_InputField SaveInput;

    private void OnEnable()
    {
        SaveInput.text = TimeController.Instance.Days + "." + TimeController.Instance.Years;
    }

    // Buttons
    public void SaveGalaxy()
    {
        FileGalaxy.SaveGalaxy(ValidateSaveFileName(SaveInput.text));

        UIManager.Instance.BackToLastActiveCanvas();
    }

    // Utility
    private string ValidateSaveFileName(string fileName)
    {
        char[] charArray = fileName.ToCharArray();
        List<char> charList = charArray.ToList();

        char[] invalidArray = Path.GetInvalidFileNameChars();

        for (int i = charList.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < invalidArray.Length; j++)
            {
                if (charList[i] == invalidArray[j])
                {
                    charList.Remove(charList[i]);
                    break;
                }
            }
        }

        return new string(charList.ToArray());
    }
}
