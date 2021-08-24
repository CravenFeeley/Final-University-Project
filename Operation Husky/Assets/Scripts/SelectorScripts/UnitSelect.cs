using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class UnitSelect : MonoBehaviour
{
    private BattleState BSM;
    //private SelectorScript SS;
    private GameObject selector;
    public GameObject Light;
    public GameObject Heavy;
    public GameObject Artillery;
    public GameObject SelectButton;
    public Transform unitSpacer;
    public GameObject SelectPanel;
    



    private List<GameObject> slctbtns = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Artillery.SetActive(false);
        Light.SetActive(false);
        Heavy.SetActive(false);

        SelectPanel.SetActive(true);
        CreateUnitSelectButtons();
        //SS = GameObject.Find("Selections").GetComponent<SelectorScript>();
        BSM = GameObject.Find("BattleManager").GetComponent<BattleState>();
    }

    public void InputSelectLight()
    {
        Light.SetActive(true);
        //SS.SelectionsMade.Add(selector);
        BSM.AlliesInBattle.Add(Light);
        SelectPanel.SetActive(false);
    }
    public void InputSelectHeavy()
    {
        Heavy.SetActive(true);
        //SS.SelectionsMade.Add(selector);
        BSM.AlliesInBattle.Add(Heavy);
        SelectPanel.SetActive(false);
    }
    public void InputSelectArtillery()
    {
        Artillery.SetActive(true);
        //SS.SelectionsMade.Add(selector);
        BSM.AlliesInBattle.Add(Artillery);
        SelectPanel.SetActive(false);
        
    }

    public void CreateUnitSelectButtons()
    {
        GameObject LightButton = Instantiate(SelectButton) as GameObject;
        TextMeshProUGUI InfantryButtonTextPro = LightButton.transform.Find("SelectButtonTextPro").gameObject.GetComponent<TextMeshProUGUI>();
        InfantryButtonTextPro.text = "Infantry Division";
        //Text LightButtonText = LightButton.transform.Find("SelectButtonText").gameObject.GetComponent<Text>();
        //LightButtonText.text = "Light";
        LightButton.GetComponent<Button>().onClick.AddListener(() => InputSelectLight());
        LightButton.transform.SetParent(unitSpacer, false);
        slctbtns.Add(LightButton);

        GameObject HeavyButton = Instantiate(SelectButton) as GameObject;
        TextMeshProUGUI TankButtonTextPro = HeavyButton.transform.Find("SelectButtonTextPro").gameObject.GetComponent<TextMeshProUGUI>();
        TankButtonTextPro.text = "Tank Division";
        //Text HeavyButtonText = HeavyButton.transform.Find("SelectButtonText").gameObject.GetComponent<Text>();
        //HeavyButtonText.text = "Heavy";
        HeavyButton.GetComponent<Button>().onClick.AddListener(() => InputSelectHeavy());
        HeavyButton.transform.SetParent(unitSpacer, false);
        slctbtns.Add(HeavyButton);

        GameObject ArtilleryButton = Instantiate(SelectButton) as GameObject;
        TextMeshProUGUI ArtilleryButtonTextPro = ArtilleryButton.transform.Find("SelectButtonTextPro").gameObject.GetComponent<TextMeshProUGUI>();
        ArtilleryButtonTextPro.text = "Artillery Division";
        //Text ArtilleryButtonText = ArtilleryButton.transform.Find("SelectButtonText").gameObject.GetComponent<Text>();
        //ArtilleryButtonText.text = "Artillery";
        ArtilleryButton.GetComponent<Button>().onClick.AddListener(() => InputSelectArtillery());
        ArtilleryButton.transform.SetParent(unitSpacer, false);
        slctbtns.Add(ArtilleryButton);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
