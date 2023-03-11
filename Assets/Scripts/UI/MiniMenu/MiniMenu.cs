using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

public class MiniMenu : MonoBehaviour
{
    public void BackToMainMenu()
    {
        GameController.SetGameState(GameState.MainMenu);
    }
}
