using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]

public class BattleState : MonoBehaviour
{
    public enum PerformAction
    {
        WAIT,
        TAKE,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE,
        PAUSE
    }
    public PerformAction battleState;

    public enum AlliesGUI
    {
        ACTIVATE,
        WAITING,
        InputAttack,
        InputEnemySelect,
        DONE
    }
    public AlliesGUI AlliesInput;

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> AlliesInBattle = new List<GameObject>();
    public List<GameObject> AxisInBattle = new List<GameObject>();
    public List<GameObject> AlliesToManage = new List<GameObject>();

    public HandleTurn AlliesChoice;

    public GameObject enemyButton;
    public Transform EnemySpacer;
    public GameObject EnemySelectPanel;

    public GameObject ActionPanel;
    public GameObject actionButton;
    public Transform actionSpacer;

    public GameObject InformationScreen;
    public bool MaskActive;
    public float countdown;
    public bool MaskChoiceTaken;
    private bool Buttonpressed;


    public AudioClip BattleMusic;
    public AudioSource BattleMusicSource;
    public AudioClip ChoiceMade;
    public AudioSource ChoiceMadeSource;
    public AudioClip BuildUp;
    public AudioSource BuildUpSource;

    private List<GameObject> actnbtns = new List<GameObject>();
    private List<GameObject> enemybtns = new List<GameObject>();

    bool reinforce;

    public int AlliedUnits;
    public bool RandomBattle;
    private int NextLevelToLoad;
    private int RetryLevelToLoad;

    // Use this for initialization
    void Start()
    {
        battleState = PerformAction.WAIT;

        AlliesInput = AlliesGUI.ACTIVATE;

        AxisInBattle.AddRange(GameObject.FindGameObjectsWithTag("Axis"));
        AlliesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Allies"));

        InformationScreen.SetActive(true);
        ActionPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);

        EnemyButtons();

        countdown = 0;

        NextLevelToLoad = SceneManager.GetActiveScene().buildIndex + 1;
        RetryLevelToLoad = SceneManager.GetActiveScene().buildIndex + 2;

        Buttonpressed = false;

        BattleMusicSource.clip = BattleMusic;
        ChoiceMadeSource.clip = ChoiceMade;
        BuildUpSource.clip  = BuildUp;
    }

    // Update is called once per frame
    void Update()
    {

        if (Buttonpressed)
        {
            Countdown();
        }
        switch (battleState)
        {
            case (PerformAction.WAIT):
                if (PerformList.Count > 0)
                {
                    battleState = PerformAction.TAKE;
                }
                break;

            case (PerformAction.TAKE):
                GameObject performer = GameObject.Find(PerformList[0].Attacker);

                if (PerformList[0].Type == "Enemy")
                {
                    AxisState ASM = performer.GetComponent<AxisState>();
                    for (int i = 0; i < AlliesInBattle.Count; i++)
                    {
                        if (PerformList[0].AttackersTarget == AlliesInBattle[i])
                        {
                            ASM.AlliesToAttack = PerformList[0].AttackersTarget;
                            ASM.currentState = AxisState.TurnState.ACTION;
                            break;
                        }
                        else
                        {
                            PerformList[0].AttackersTarget = AlliesInBattle[Random.Range(0, AlliesInBattle.Count)];
                            ASM.AlliesToAttack = PerformList[0].AttackersTarget;
                            ASM.currentState = AxisState.TurnState.ACTION;
                        }
                    }
                }

                if (PerformList[0].Type == "Hero")
                {
                    AlliesState HSM = performer.GetComponent<AlliesState>();
                    HSM.EnemyToAttack = PerformList[0].AttackersTarget;
                    HSM.currentState = AlliesState.TurnState.ACTION;
                }
                EnemyButtons();
                battleState = PerformAction.PERFORMACTION;
                break;

            case (PerformAction.PERFORMACTION):

                break;

            case (PerformAction.CHECKALIVE):
                if (AlliesInBattle.Count < 1)
                {
                    battleState = PerformAction.LOSE;
                }

                else if (AxisInBattle.Count < 1)
                {
                    battleState = PerformAction.WIN;
                }
                else
                {
                    clearAttackPanel();
                    AlliesInput = AlliesGUI.ACTIVATE;
                }
                break;

            case (PerformAction.WIN):
                {
                    if (RandomBattle == true)
                    {
                        SceneManager.LoadScene("RandomBattleWin");
                    }
                    else if (RandomBattle == false)
                    {
                        SceneManager.LoadScene(NextLevelToLoad);
                    }

                    for (int i = 0; i < AlliesInBattle.Count; i++)
                    {
                        AlliesInBattle[i].GetComponent<AlliesState>().currentState = AlliesState.TurnState.WAITING;
                    }
                }
                break;

            case (PerformAction.LOSE):
                {
                    if (RandomBattle == true)
                    {
                        SceneManager.LoadScene("RandomBattleLose");
                    }
                    else if (RandomBattle == false)
                    {
                        SceneManager.LoadScene(RetryLevelToLoad);
                    }

                }
                break;

            case (PerformAction.PAUSE):
                {

                }
                break;
        }

        switch (AlliesInput)
        {
            case (AlliesGUI.ACTIVATE):
                if (AlliesToManage.Count > 0)
                {
                    AlliesToManage[0].transform.Find("Arrow").gameObject.SetActive(true);
                    AlliesChoice = new HandleTurn();
                    ActionPanel.SetActive(true);
                    CreateAttackButtons();
                    AlliesInput = AlliesGUI.WAITING;
                }
                break;

            case (AlliesGUI.WAITING):

                break;

            case (AlliesGUI.DONE):
                AlliesInputDone();
                break;
        }

    }

    public void CollectActions(HandleTurn input)
    {
        PerformList.Add(input);
    }

    public void EnemyButtons()
    {
        foreach (GameObject enemyBtn in enemybtns)
        {
            Destroy(enemyBtn);
        }

        enemybtns.Clear();

        foreach (GameObject Axis in AxisInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            AxisState cur_enemy = Axis.GetComponent<AxisState>();

            TextMeshProUGUI buttonTextPro = newButton.transform.Find("SelectText").gameObject.GetComponent<TextMeshProUGUI>();
            if (cur_enemy.MaskActive == true)
            {
                buttonTextPro.text = "UnknownDivision";
            }
            else
            {
                buttonTextPro.text = cur_enemy.Axis.name;
            }
            //Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            //buttonText.text = cur_enemy.Axis.name;

            button.EnemyPrefab = Axis;

            newButton.transform.SetParent(EnemySpacer, false);

            enemybtns.Add(newButton);
        }
    }

    public void InputAttack()
    {
        AlliesChoice.Attacker = AlliesToManage[0].name;
        AlliesChoice.AttackersGameObject = AlliesToManage[0];
        AlliesChoice.Type = "Hero";
        AlliesChoice.chosenAction = AlliesToManage[0].GetComponent<AlliesState>().Allies.actions[0];
        ActionPanel.SetActive(false);
        //EnemyButtons();
        EnemySelectPanel.SetActive(true);
    }
    public void InputReinforce()
    {
        AlliesChoice.Attacker = AlliesToManage[0].name;
        AlliesChoice.AttackersGameObject = AlliesToManage[0];
        AlliesChoice.Type = "Hero";
        reinforce = AlliesChoice.AttackersGameObject.GetComponent<AlliesState>().canReinforce;
        if (reinforce == true)
        {
            AlliesChoice.chosenAction = AlliesToManage[0].GetComponent<AlliesState>().Allies.actions[3];
            ActionPanel.SetActive(false);
            AlliesInput = AlliesGUI.DONE;
        }
        else
        {
            AlliesInput = AlliesGUI.WAITING;
        }
    }
    public void InputAllOutAttack()
    {
        AlliesChoice.Attacker = AlliesToManage[0].name;
        AlliesChoice.AttackersGameObject = AlliesToManage[0];
        AlliesChoice.Type = "Hero";
        AlliesChoice.chosenAction = AlliesToManage[0].GetComponent<AlliesState>().Allies.actions[1];
        ActionPanel.SetActive(false);
        //EnemyButtons();
        EnemySelectPanel.SetActive(true);
    }
    public void InputDefence()
    {
        AlliesChoice.Attacker = AlliesToManage[0].name;
        AlliesChoice.AttackersGameObject = AlliesToManage[0];
        AlliesChoice.Type = "Hero";
        AlliesChoice.chosenAction = AlliesToManage[0].GetComponent<AlliesState>().Allies.actions[2];
        ActionPanel.SetActive(false);
        AlliesInput = AlliesGUI.DONE;
    }

    public void InputEnemySelect(GameObject ChosenEnemy)
    {
        AlliesChoice.AttackersTarget = ChosenEnemy;
        AlliesInput = AlliesGUI.DONE;
    }

    void AlliesInputDone()
    {
        PerformList.Add(AlliesChoice);
        clearAttackPanel();

        AlliesToManage[0].transform.Find("Arrow").gameObject.SetActive(false);
        AlliesToManage.RemoveAt(0);
        AlliesInput = AlliesGUI.ACTIVATE;
    }

    void clearAttackPanel()
    {
        EnemySelectPanel.SetActive(false);
        ActionPanel.SetActive(false);

        foreach (GameObject actnbtn in actnbtns)
        {
            Destroy(actnbtn);
        }
        actnbtns.Clear();
    }

    void CreateAttackButtons()
    {
        GameObject AttackButton = Instantiate(actionButton) as GameObject;
        TextMeshProUGUI AttackButtonTextPro = AttackButton.transform.Find("AttackTMP").gameObject.GetComponent<TextMeshProUGUI>();
        AttackButtonTextPro.text = "Attack";
        //Text AttackButtonText = AttackButton.transform.Find("AttackText").gameObject.GetComponent<Text>();
        //AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(() => InputAttack());
        AttackButton.transform.SetParent(actionSpacer, false);
        actnbtns.Add(AttackButton);

        GameObject ReinforceButton = Instantiate(actionButton) as GameObject;
        TextMeshProUGUI ReinforceButtonTextPro = ReinforceButton.transform.Find("AttackTMP").gameObject.GetComponent<TextMeshProUGUI>();
        ReinforceButtonTextPro.text = "Reinforce";
        //Text ReinforceButtonText = ReinforceButton.transform.Find("AttackText").gameObject.GetComponent<Text>();
        //ReinforceButtonText.text = "Reinforce";
        ReinforceButton.GetComponent<Button>().onClick.AddListener(() => InputReinforce());
        ReinforceButton.transform.SetParent(actionSpacer, false);
        actnbtns.Add(ReinforceButton);

        GameObject DefendButton = Instantiate(actionButton) as GameObject;
        TextMeshProUGUI DefendButtonTextPro = DefendButton.transform.Find("AttackTMP").gameObject.GetComponent<TextMeshProUGUI>();
        DefendButtonTextPro.text = "Defend";
        //Text DefendButtonText = DefendButton.transform.Find("AttackText").gameObject.GetComponent<Text>();
        //DefendButtonText.text = "Defend";
        DefendButton.GetComponent<Button>().onClick.AddListener(() => InputDefence());
        DefendButton.transform.SetParent(actionSpacer, false);
        actnbtns.Add(DefendButton);

        GameObject DesperateButton = Instantiate(actionButton) as GameObject;
        TextMeshProUGUI DesperateButtonTextPro = DesperateButton.transform.Find("AttackTMP").gameObject.GetComponent<TextMeshProUGUI>();
        DesperateButtonTextPro.text = "All Out Attack";
        //Text DesperateButtonText = DesperateButton.transform.Find("AttackText").gameObject.GetComponent<Text>();
        //DesperateButtonText.text = "All Out Attack";
        DesperateButton.GetComponent<Button>().onClick.AddListener(() => InputAllOutAttack());
        DesperateButton.transform.SetParent(actionSpacer, false);
        actnbtns.Add(DesperateButton);
    }

    public void InfoPanel()
    {
        InformationScreen.SetActive(false);
    }

    public void MaskSelectorYes()
    {
        BattleMusicSource.Play();
        countdown = 0;
        MaskActive = false;
        Buttonpressed = true;
        BuildUpSource.Stop();
        BattleMusicSource.Play();
    }
    public void MaskSelectorNo()
    {
        ChoiceMadeSource.Play();
        countdown = 0;
        MaskActive = true;
        Buttonpressed = true;
        BuildUpSource.Stop();
        BattleMusicSource.Play();
    }

    public void Countdown()
    {
        countdown = countdown + Time.deltaTime;
        if (countdown >= 1f)
        {
            InformationScreen.SetActive(false);
            MaskChoiceTaken = true;
            Buttonpressed = false;
            countdown = 0;
        }
    }
}

