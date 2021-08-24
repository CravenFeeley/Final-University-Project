using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]

public class AxisState : MonoBehaviour
{

    public BaseAxis Axis;
    private BattleState BSM;


    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }
    public TurnState currentState;

    public Animator AttackAnimation;
    public Animator TwoThirdAnim;
    public Animator OneThirdAnim;
    public float animSpeed = 15f;
    private float cur_cooldown = 0f;
    private float max_cooldown;
    public int reinforcesleft;
    public bool Defending;
    private bool alive = true;
    public int AlliedUnits;
    public bool MaskActive = true;

    public GameObject twoThirds;
    public GameObject oneThird;
    public GameObject Mask;
    public GameObject Damage;
    //public Text damageRecieved;
    public TextMeshProUGUI damageNumber;
    public bool DamageTaken;
    public float damageTimer;
    public GameObject Arrow;
    public GameObject MaskArrow;
    private Vector2 startposition;
    public GameObject AlliesToAttack;
    HandleTurn myAttack = new HandleTurn();

    public AudioClip AttackSound;
    public AudioSource AttackSource;

    // time for action
    private bool actionStarted = false;

    // Use this for initialization
    void Start()
    {

        damageTimer = 0f;
        //AttackSource.clip = AttackSound;

        Damage.SetActive(false);
        Arrow.SetActive(false);
        MaskArrow.SetActive(false);

        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleState>();
        startposition = transform.position;

        AlliedUnits = BSM.AlliedUnits;

        DamageTaken = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (DamageTaken == true)
        {
            TimerCountdown();
        }


        if (BSM.MaskChoiceTaken == false)
        {
            MaskActive = BSM.MaskActive;
            CheckMask();
        }
        else
        {
            MaskActive = MaskActive;
            CheckMask();
        }
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                ProgressBar();

                break;

            case (TurnState.CHOOSEACTION):
                if (BSM.AlliesInBattle.Count > 0)
                {
                    ChooseAction();
                    AlliedUnits = 0;
                }
                currentState = TurnState.WAITING;
                break;

            case (TurnState.WAITING):

                break;


            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;

            case (TurnState.DEAD):
                if (!alive)
                {
                    return;
                }
                else
                {
                    this.gameObject.tag = "DeadAxis";
                    BSM.AxisInBattle.Remove(this.gameObject);
                    Arrow.SetActive(false);

                    for (int i = 0; i < BSM.PerformList.Count; i++)
                    {
                        if (BSM.PerformList[i].AttackersGameObject == this.gameObject)
                        {
                            BSM.PerformList.Remove(BSM.PerformList[i]);
                        }
                    }
                    this.gameObject.GetComponent<SpriteRenderer>().material.color = new Color32(105, 105, 105, 255);
                    alive = false;
                    BSM.EnemyButtons();
                    BSM.battleState = BattleState.PerformAction.CHECKALIVE;


                }
                break;
        }
    }



    void ChooseAction()
    {

        myAttack.Attacker = Axis.name;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.AlliesInBattle[Random.Range(0, BSM.AlliesInBattle.Count)];

        //int num = Random.Range(0, Axis.actions.Count);

        if (Axis.cur_manpower >= Axis.max_manpower / 2)
        {
            myAttack.chosenAction = Axis.actions[0];
        }
        if (Axis.cur_manpower <= Axis.max_manpower / 2)
            if (Defending == false)
            {
                myAttack.chosenAction = Axis.actions[2];
            }
            else if (Defending == true)
            {
                if (reinforcesleft > 0)
                {
                    myAttack.chosenAction = Axis.actions[3];
                }
                else if (reinforcesleft <= 0)
                {
                    myAttack.chosenAction = Axis.actions[1];
                }
            }

        if (Axis.cur_manpower <= Axis.max_manpower / 3)
        {
            if (reinforcesleft > 0)
            {
                myAttack.chosenAction = Axis.actions[3];
            }
            else if (reinforcesleft <= 0)
            {
                myAttack.chosenAction = Axis.actions[1];
            }
        }
        if (Axis.cur_manpower <= Axis.max_manpower / 5)
        {
            myAttack.chosenAction = Axis.actions[1];
        }
        BSM.CollectActions(myAttack);
    }

    private IEnumerator TimeForAction()
    {

        if (BSM.PerformList[0].chosenAction.attackDamage > 1 && BSM.PerformList[0].chosenAction.isDefending == false)
        {
            if (actionStarted)
            {
                yield break;
            }



            actionStarted = true;

            Vector2 AlliesPosition = new Vector2(AlliesToAttack.transform.position.x + 1.5f, AlliesToAttack.transform.position.y);
            while (MoveTowardsEnemy(AlliesPosition))
            {
                yield return null;
            }

            AttackAnimation.SetBool("isAttacking", true);
            TwoThirdAnim.SetBool("isAttacking", true);
            OneThirdAnim.SetBool("isAttacking", true);

            BSM.BattleMusicSource.Pause();
            AttackSource.Play();

            yield return new WaitForSeconds(3f);

            AttackAnimation.SetBool("isAttacking", false);
            TwoThirdAnim.SetBool("isAttacking", false);
            OneThirdAnim.SetBool("isAttacking", false);

            AttackSource.Stop();
            BSM.BattleMusicSource.Play();

            DoDamage();
            ManPowerGain();
            Defending = false;
            MaskActive = false;
            CheckMask();

            Vector2 firstPosition = startposition;
            while (MoveTowardsStart(firstPosition))
            {
                yield return null;
            }

            BSM.PerformList.RemoveAt(0);

            if (BSM.battleState != BattleState.PerformAction.WIN && BSM.battleState != BattleState.PerformAction.LOSE)
            {


                BSM.battleState = BattleState.PerformAction.WAIT;
                cur_cooldown = 0f;
                currentState = TurnState.PROCESSING;
            }
            else
            {
                currentState = TurnState.WAITING;
            }

            actionStarted = false;

        }


        else if (BSM.PerformList[0].chosenAction.attackDamage < 1 && BSM.PerformList[0].chosenAction.isDefending == false)
        {
            if (actionStarted)
            {
                yield break;
            }

            actionStarted = true;

            Vector2 AxisPosition = new Vector2(this.gameObject.transform.position.x + 1f, this.gameObject.transform.position.y);
            while (MoveTowardsReinforce(AxisPosition))
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            ManPowerGain();
            Reinforces();
            Defending = false;


            Vector2 firstposition = startposition;
            while (MoveTowardsStart(firstposition))
            {
                yield return null;
            }

            BSM.PerformList.RemoveAt(0);

            if (BSM.battleState != BattleState.PerformAction.WIN && BSM.battleState != BattleState.PerformAction.LOSE)
            {


                BSM.battleState = BattleState.PerformAction.WAIT;
                cur_cooldown = 0f;
                currentState = TurnState.PROCESSING;
            }
            else
            {
                currentState = TurnState.WAITING;
            }

            actionStarted = false;

        }
        else if (BSM.PerformList[0].chosenAction.isDefending == true)
        {
            if (actionStarted)
            {
                yield break;
            }

            actionStarted = true;

            if (Defending == false)
            {
                Vector2 AxisPosition = new Vector2(this.gameObject.transform.position.x + 3f, this.gameObject.transform.position.y);
                while (MoveTowardsReinforce(AxisPosition))
                {
                    yield return null;
                }
            }
            yield return new WaitForSeconds(2f);

            Defending = true;



            BSM.PerformList.RemoveAt(0);

            if (BSM.battleState != BattleState.PerformAction.WIN && BSM.battleState != BattleState.PerformAction.LOSE)
            {


                BSM.battleState = BattleState.PerformAction.WAIT;
                cur_cooldown = 0f;
                currentState = TurnState.PROCESSING;
            }
            else
            {
                currentState = TurnState.WAITING;
            }

            actionStarted = false;

        }

    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsReinforce(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }


    void ProgressBar()
    {
        switch (Axis.EnemyType)
        {
            case UnitType.LIGHT:
                max_cooldown = 7.5f;
                break;

            case UnitType.ARTILLERY:
                max_cooldown = 10f;
                break;

            case UnitType.HEAVY:
                max_cooldown = 12.5f;
                break;

            default:
                break;
        }
        if (BSM.AlliesInBattle.Count >= AlliedUnits)
        {
            cur_cooldown = cur_cooldown + Time.deltaTime;
            AlliedUnits = 0;
        }

        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnState.CHOOSEACTION;
        }
    }

    void DoDamage()
    {
        float multiplier = Effectiveness(Axis.EnemyType, AlliesToAttack.GetComponent<BaseAllies>().FriendlyType);
        float calc_damage = ((Axis.attack + BSM.PerformList[0].chosenAction.attackDamage) - (AlliesToAttack.GetComponent<BaseAllies>().defence / 2)) * multiplier;

        AlliesToAttack.GetComponent<AlliesState>().TakeDamage(calc_damage);
    }

    void ManPowerGain()
    {
        float calc_reinforce = Axis.cur_manpower + BSM.PerformList[0].chosenAction.manpowerGain;
        Axis.cur_manpower = calc_reinforce;

        if (Axis.cur_manpower > Axis.max_manpower)
        {
            Axis.cur_manpower = Axis.max_manpower;
        }

    }

    public void TakeDamage(float GetDamageAmount)
    {
        if (Defending == true)
        {
            Axis.cur_manpower -= GetDamageAmount / 2;
            Damage.SetActive(true);
            damageNumber.text = "" + GetDamageAmount / 2;
            DamageTaken = true;
        }
        else
        {
            Axis.cur_manpower -= GetDamageAmount;
            Damage.SetActive(true);
            damageNumber.text = "" + GetDamageAmount;
            DamageTaken = true;
        }
        MaskActive = false;
        CheckMask();
        CheckHealth();


    }
    public void Reinforces()
    {
        reinforcesleft -= 1;
        if (reinforcesleft <= 0)
        {
            GetComponent<BaseAxis>().actions.RemoveAt(3);
        }
        oneThird.SetActive(true);
        twoThirds.SetActive(true);
    }
    public void CheckHealth()
    {
        if (Axis.cur_manpower <= 0)
        {
            Axis.cur_manpower = 0;
            currentState = TurnState.DEAD;
        }
        if (Axis.cur_manpower <= (Axis.max_manpower / 3) * 2)
        {
            twoThirds.SetActive(false);
        }
        else if (Axis.cur_manpower >= (Axis.max_manpower / 3) * 2)

        {
            twoThirds.SetActive(true);
        }
        if (Axis.cur_manpower <= (Axis.max_manpower / 3))
        {
            oneThird.SetActive(false);
        }
        else if (Axis.cur_manpower >= (Axis.max_manpower / 3))

        {
            oneThird.SetActive(true);
        }
    }
    public float Effectiveness(UnitType enemyType, UnitType friendlyType)
    {
        float multiplier = 1f;
        switch (enemyType)
        {
            case UnitType.LIGHT:
                switch (friendlyType)
                {
                    case UnitType.LIGHT:
                        break;
                    case UnitType.HEAVY:
                        break;
                    case UnitType.ARTILLERY:
                        multiplier = 2f;
                        break;
                    default:
                        break;
                }
                break;
            case UnitType.ARTILLERY:
                switch (friendlyType)
                {
                    case UnitType.LIGHT:
                        break;
                    case UnitType.HEAVY:
                        multiplier = 2f;
                        break;
                    case UnitType.ARTILLERY:
                        break;
                    default:
                        break;
                }
                break;
            case UnitType.HEAVY:
                switch (friendlyType)
                {
                    case UnitType.LIGHT:
                        multiplier = 2f;
                        break;
                    case UnitType.HEAVY:
                        break;
                    case UnitType.ARTILLERY:
                        break;
                    default:
                        break;
                }
                break;
        }
        return multiplier;
    }
    public void CheckMask()
    {
        if (MaskActive == true)
        {
            Mask.SetActive(true);
            oneThird.SetActive(false);
            twoThirds.SetActive(false);
        }

        else if (MaskActive == false)
        {
            Mask.SetActive(false);
            if (Axis.cur_manpower <= (Axis.max_manpower / 3) * 2)
            {
                twoThirds.SetActive(false);
            }
            else
            {
                twoThirds.SetActive(true);
            }
            if (Axis.cur_manpower <= (Axis.max_manpower / 3))
            {
                oneThird.SetActive(false);
            }
            else
            {
                oneThird.SetActive(true);
            }
        }
    }

    public void TimerCountdown()
    {
        damageTimer = damageTimer + Time.deltaTime;

        if (damageTimer >= 2.5f)
        {
            Damage.gameObject.SetActive(false);
            damageTimer = 0f;
            DamageTaken = false;
        }
    }
}

