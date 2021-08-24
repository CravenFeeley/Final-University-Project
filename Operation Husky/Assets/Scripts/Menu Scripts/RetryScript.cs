using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryScript : MonoBehaviour
{
    private int RetryLevelToLoad;
    public AudioClip ChoiceMade;
    public AudioSource GunSound;

    private void Start()
    {
        RetryLevelToLoad = SceneManager.GetActiveScene().buildIndex - 2;
    }

    public void RetryLevel()
    {
        GunSound.Play();
        SceneManager.LoadScene(RetryLevelToLoad);
    }
    public void MainMenu()
    {
        GunSound.Play();
        SceneManager.LoadScene("MainMenu");
    }
}
