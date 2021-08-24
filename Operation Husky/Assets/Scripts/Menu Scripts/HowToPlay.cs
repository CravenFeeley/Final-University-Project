using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlay : MonoBehaviour
{

    public GameObject Tutorial1;
    public GameObject Tutorial2;
    public GameObject Tutorial3;
    public GameObject Tutorial4;
    public GameObject Tutorial5;
    public GameObject Tutorial6;
    public GameObject Button1;
    public GameObject Button2;
    public GameObject Button3;
    public GameObject Button4;
    public GameObject Button5;

    public AudioClip ChoiceMade;
    public AudioSource GunSound;



    // Start is called before the first frame update
    void Start()
    {
        Tutorial1.SetActive(true);
        Tutorial2.SetActive(false);
        Tutorial3.SetActive(false);
        Tutorial4.SetActive(false);
        Tutorial5.SetActive(false);
        Tutorial6.SetActive(false);

        Button1.SetActive(true);
        Button2.SetActive(false);
        Button3.SetActive(false);
        Button4.SetActive(false);
        Button5.SetActive(false);
        
    }

    public void NextButton1()
    {
            Tutorial1.SetActive(false);
            Tutorial2.SetActive(true);
            Button1.SetActive(false);
            Button2.SetActive(true);
            GunSound.Play();
    }
    public void NextButton2()
    {
        Tutorial2.SetActive(false);
        Tutorial3.SetActive(true);
        Button2.SetActive(false);
        Button3.SetActive(true);
        GunSound.Play();
    }
    public void NextButton3()
    {
        Tutorial3.SetActive(false);
        Tutorial4.SetActive(true);
        Button3.SetActive(false);
        Button4.SetActive(true);
        GunSound.Play();
    }
    public void NextButton4()
    {
        Tutorial4.SetActive(false);
        Tutorial5.SetActive(true);
        Button4.SetActive(false);
        Button5.SetActive(true);
        GunSound.Play();

    }
    public void NextButton5()
    {
        Tutorial5.SetActive(false);
        Tutorial6.SetActive(true);
        Button5.SetActive(false);
        GunSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
