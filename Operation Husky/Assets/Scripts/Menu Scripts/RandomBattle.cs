using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RandomBattle : MonoBehaviour
{
    public AudioClip ChoiceMade;
    public AudioSource GunSound;
    public GameObject WinScreen;
    public GameObject RandomBattlePanel;
    // Start is called before the first frame update
    void Start()
    {
        WinScreen.SetActive(true);
        RandomBattlePanel.SetActive(false);
    }

    public void PlayAgain()
    {
        WinScreen.SetActive(false);
        RandomBattlePanel.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Select1V1()
    {
        SceneManager.LoadScene("1v1RandomBattle");
    }

    public void Select2V2()
    {
        SceneManager.LoadScene("2v2RandomBattle");
    }

    public void Select3V3()
    {
        SceneManager.LoadScene("3v3RandomBattle");
    }

    public void Back()
    {
        RandomBattlePanel.SetActive(false);
        WinScreen.SetActive(true);
    }

}
