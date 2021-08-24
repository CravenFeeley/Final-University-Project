using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    public HowToPlay H2P;
    public AudioClip ChoiceMade;
    public AudioSource GunSound;
    public GameObject MainMenuPanel;
    public GameObject RandomBattlePanel;
    public GameObject HowToPlayPanel;


    // Start is called before the first frame update
    void Start()
    {
        H2P = GameObject.Find("How To Play Menu").GetComponent<HowToPlay>();
        GunSound.clip = ChoiceMade;
        MainMenuPanel.SetActive(true);
        RandomBattlePanel.SetActive(false);
        HowToPlayPanel.SetActive(false);
    }

    public void SelectCampaign()
    {
        SceneManager.LoadScene("BeachLanding");
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

    public void OpenRandomPanel()
    {
        MainMenuPanel.SetActive(false);
        RandomBattlePanel.SetActive(true);
        GunSound.Play();
    }

    public void OpenH2PPanel()
    {
        MainMenuPanel.SetActive(false);
        HowToPlayPanel.SetActive(true);
        GunSound.Play();
    }

    public void BackToMainMenu()
    {

        H2P.Tutorial1.SetActive(true);
        H2P.Tutorial2.SetActive(false);
        H2P.Tutorial3.SetActive(false);
        H2P.Tutorial4.SetActive(false);
        H2P.Tutorial5.SetActive(false);
        H2P.Tutorial6.SetActive(false);
        H2P.Button1.SetActive(true);
        H2P.Button2.SetActive(false);
        H2P.Button3.SetActive(false);
        H2P.Button4.SetActive(false);
        H2P.Button5.SetActive(false);
        RandomBattlePanel.SetActive(false);
        HowToPlayPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
        GunSound.Play();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
