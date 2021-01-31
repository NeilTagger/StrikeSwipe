using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameController gameController;
    private AudioSource sound;
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }
    public void PlayGame()
    {
        sound.Play();
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
