using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    private int NextLevelToLoad;

    public AudioClip ChoiceMade;
    public AudioSource GunSound;

    private void Start()
    {
        NextLevelToLoad = SceneManager.GetActiveScene().buildIndex + 2;
        GunSound.clip = ChoiceMade;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(NextLevelToLoad);
        GunSound.Play();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        GunSound.Play();
    }

}
