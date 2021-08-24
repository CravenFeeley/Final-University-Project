using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameComplete : MonoBehaviour
{

    public GameObject InfoPanel;
    public GameObject MainMenuPanel;
    public AudioClip ChoiceMade;
    public AudioSource GunSound;

    private void Start()
    {
        InfoPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void MainMenu()
    {
        GunSound.Play();
        SceneManager.LoadScene("MainMenu");
    }
    public void InfoContinue()
    {
        GunSound.Play();
        InfoPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

}
