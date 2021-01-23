using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameController gameController;
   public void PlayGame()
    {
        Debug.Log("play");
        Invoke("startGame", 0.1f);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void startGame()
    {
        gameController.gameStarted = true;
        gameObject.SetActive(false);
    }

}
