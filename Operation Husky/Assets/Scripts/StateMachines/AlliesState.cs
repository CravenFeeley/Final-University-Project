using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]

public class AlliesState : MonoBehaviour
{

    public BaseAllies Allies;
    private BattleState BSM;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }
    public TurnState currentState;

    //public UnitType unitType;
    public float animSpeed = 15f;
    private float cur_cooldown = 0f;
    private float max_cooldown;
    public float reinforcesleft;
    private float reinforcesstart;
    public bool canReinforce;
    public bool Defending;
    private bool alive = true;
    public int AlliedUnits;

    public Animator AttackAnimation;
    public Animator TwoThirdAnim;
    public Animator OneThirdAnim;
    public GameObject twoThirds;
    public GameObject oneThird;
    //private float damageNotifier = 0f;
    public GameObject Damage;
    public Text damageRecieved;
    public bool DamageTaken;
    public float damageTimer;
    public TextMeshProUGUI damageNumber;
    public GameObject Arrow;
    private Vector2 startPosition;
    public GameObject EnemyToAttack;

    public AudioClip AttackSound;
    public AudioSource AttackSource;

    public Image progressBar;
    //public Image healthBar;
    public Image reinforceBar;
    private bool actionStarted = false;

    // Use this for initialization
    void Start()
    {
        //AttackSource.clip = AttackSound;

        Damage.SetActive(false);

        Arrow.SetActive(false);

        currentState = TurnState.PROCESSING;

        BSM = GameObject.Find("BattleManager").GetComponent<BattleState>();

        AlliedUnits = BSM.AlliedUnits;

        reinforcesstart = reinforcesleft;
        canReinforce = true;
        cur_cooldown = 0f;

        startPosition = transform.position;

        if (Allies.cur_manpower <= (Allies.max_manpower / 3) * 2)
        {
            twoThirds.SetActive(false);
        }
        else
        {
            twoThirds.SetActive(true);
        }
        if (Allies.cur_manpower <= (Allies.max_manpower / 3))
        {
            oneThird.SetActive(false);
        }
        else
        {
            oneThird.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DamageTaken == true)
        {
            TimerCountdown();
        }

        switch (currentState)
        {
            case (TurnState.PROCESSING):
                ProgressBar();
                break;

            case (TurnState.ADDTOLIST):
                BSM.AlliesToManage.Add(this.gameObject);
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
                    this.gameObject.tag = "DeadAllies";

                    BSM.AlliesInBattle.Remove(this.gameObject);
                    BSM.AlliesToManage.Remove(this.gameObject);
                    Arrow.SetActive(false);
                    BSM.ActionPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);

                    for (int i = 0; i < BSM.PerformList.Count; i++)
                        if (BSM.PerformList[i].AttackersGameObject == this.gameObject)
                        {
                            BSM.PerformList.Remove(BSM.PerformList[i]);
                        }

                    this.gameObject.GetComponent<SpriteRenderer>().material.color = new Color32(105, 105, 105, 255);

                    BSM.battleState = BattleState.PerformAction.CHECKALIVE;
                    alive = false;
                }
                break;
        }
    }

    void ProgressBar()
    {

        switch (Allies.FriendlyType)
        {
            case UnitType.LIGHT:
                max_cooldown = 5f;
                break;

            case UnitType.ARTILLERY:
                max_cooldown = 7.5f;
                break;

            case UnitType.HEAVY:
                max_cooldown = 10f;
                break;

            default:
                break;
        }

        if (BSM.AlliesInBattle.Count >= AlliedUnits)
        {
            cur_cooldown = cur_cooldown + Time.deltaTime;
            float calc_cooldown = cur_cooldown / max_cooldown;
            progressBar.fillAmount = Mathf.Clamp(calc_cooldown, 0, 1);
            AlliedUnits = 0;
        }

        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
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

            Vector2 AxisPosition = new Vector2(EnemyToAttack.transform.position.x - 1.5f, EnemyToAttack.transform.position.y);
            while (MoveTowardsEnemy(AxisPosition))
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

            Vector2 firstPosition = startPosition;
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

            Vector2 AlliesPosition = new Vector2(this.gameObject.transform.position.x - 1f, this.gameObject.transform.position.y);
            while (MoveTowardsReinforce(AlliesPosition))
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            ManPowerGain();
            Reinforces();
            Defending = false;

            Vector2 firstposition = startPosition;
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
                Vector2 AlliesPosition = new Vector2(this.gameObject.transform.position.x - 3f, this.gameObject.transform.position.y);
                while (MoveTowardsReinforce(AlliesPosition))
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

    public void TakeDamage(float GetDamageAmount)
    {
        if (Defending == true)
        {
            Allies.cur_manpower -= GetDamageAmount / 2;
            //healthBar.fillAmount = Allies.cur_manpower / Allies.max_manpower;
            Damage.SetActive(true);
            damageNumber.text = "" + GetDamageAmount/2;
            DamageTaken = true;
        }
        else
        {
            Allies.cur_manpower -= GetDamageAmount;
            //healthBar.fillAmount = Allies.cur_manpower / Allies.max_manpower;
            Damage.SetActive(true);
            damageNumber.text = "" + GetDamageAmount;
            DamageTaken = true;
        }
        CheckHealth();
    }

    void DoDamage()
    {
        float multiplier = Effectiveness(Allies.FriendlyType, EnemyToAttack.GetComponent<BaseAxis>().EnemyType);
        float calc_damage = ((Allies.attack + BSM.PerformList[0].chosenAction.attackDamage) - (EnemyToAttack.GetComponent<BaseAxis>().defence / 2)) * multiplier;

        EnemyToAttack.GetComponent<AxisState>().TakeDamage(calc_damage);
    }

    void ManPowerGain()
    {
        float calc_reinforce = Allies.cur_manpower + BSM.PerformList[0].chosenAction.manpowerGain;
        Allies.cur_manpower = calc_reinforce;

        if (Allies.cur_manpower > Allies.max_manpower)
        {
            Allies.cur_manpower = Allies.max_manpower;
        }

        //healthBar.fillAmount = Allies.cur_manpower / Allies.max_manpower;
    }

    public void Reinforces()
    {
        reinforcesleft -= 1;

        reinforceBar.fillAmount = reinforcesleft / reinforcesstart;

        NoReinforces();

        oneThird.SetActive(true);
        twoThirds.SetActive(true);
    }

    public void NoReinforces()
    {
        if (reinforcesleft <= 0)
        {
            GetComponent<BaseAllies>().actions.RemoveAt(3);
            canReinforce = false;
        }
    }
    public void CheckHealth()
    {
        if (Allies.cur_manpower <= 0)
        {
            Allies.cur_manpower = 0;
            currentState = TurnState.DEAD;
        }
        if (Allies.cur_manpower <= (Allies.max_manpower / 3) * 2)
        {
            twoThirds.SetActive(false);
        }
        else if (Allies.cur_manpower <= (Allies.max_manpower / 3) * 2)

        {
            twoThirds.SetActive(true);
        }
        if (Allies.cur_manpower <= (Allies.max_manpower / 3))
        {
            oneThird.SetActive(false);
        }
        else if (Allies.cur_manpower <= (Allies.max_manpower / 3))

        {
            oneThird.SetActive(true);
        }
    }
        public float Effectiveness(UnitType friendlyType, UnitType enemyType)
    {
        float multiplier = 1f;
        switch (friendlyType)
        {
            case UnitType.LIGHT:
                switch (enemyType)
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
                switch (enemyType)
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
                switch (enemyType)
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

    public void TimerCountdown()
    {
        damageTimer = damageTimer + Time.deltaTime;

        if (damageTimer >= 2.5)
        {
            Damage.gameObject.SetActive(false);
            damageTimer = 0f;
            DamageTaken = false;
        }
    }
}
