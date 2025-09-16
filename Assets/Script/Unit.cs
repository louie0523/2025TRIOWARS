using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public enum Debuff
{
    None,
    Stun,
    Postion,
}

public enum Status
{
    Idle,
    Follow,
    Chase,
    Attack,
    Dead
}

public class Unit : MonoBehaviour
{
    public UnitData unitData;

    [Header("스테이터스")]
    public int Lv;
    public float Exp;
    public float MaxExp = 100;
    public float PlusMaxExp = 200;
    public float MaxHp;
    public float CurrentHp;
    public float MaxMp;
    public float CurrentMp;
    public float DefaultAttack;
    public float Attack;
    public float AttackRange;
    public float attackTimer;
    public float AttackSpped = 1f;
    public float attackRateTime;
    public float Critical_Rate;
    public float Critical_Value;
    public float moveSpeed;
    public int inventoryMax;
    public List<int> SkillLv = new List<int>()
    {
        0,0,0,0
    };
    [Header("유닛 정보")]
    public Transform Leader;
    public Transform currentTarget;
    public Vector3 desiredVelocity;
    public Status Status;
    public Debuff debuff;
    public float stunTimer;
    public NavMeshAgent navMeshAgent;
    public Vector3 LeaderOffset;
    public Animator animator;

    public bool Load = false;
    public Material UnitMaterial;
    private Coroutine MaterialCoroutine;
    public Transform StartPos;
    public LineRenderer lineRenderer;





    private void Awake()
    {
        if (unitData.team == Team.Player)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        if (!Load)
        {
            SetData();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = transform.GetChild(0).GetComponent<Animator>();
            lineRenderer = transform.GetChild(2).GetComponent<LineRenderer>();
            Load = true;
        }
        if (unitData.team == Team.Player)
            transform.position = new Vector3(0, 0, 0);

    }

    private void Update()
    {
        if (debuff == Debuff.Stun)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer > 0)
            {
                return;
            } else
            {
                debuff = Debuff.None;
            }
        }

        if (Status == Status.Dead)
        {
            return;
        }

        if (currentTarget == null || isVaildTarget(currentTarget))
        {
            currentTarget = ResearchTarget();
        }

        if (currentTarget != null)
        {
            Unit target = currentTarget.GetComponent<Unit>();
            if (target.Status == Status.Dead)
            {
                currentTarget = ResearchTarget();
                return;
            }

            float dis = Vector3.Distance(transform.position, currentTarget.position);
            if (dis < AttackRange)
            {
                Status = Status.Attack;
            } else
            {
                Status = Status.Chase;
            }
        } else
        {
            if (Leader != null)
            {
                float dis = Vector3.Distance(transform.position, Leader.position);
                if (dis > unitData.FriendRadius && unitData.team == Team.Player)
                    Status = Status.Follow;
                else
                    Status = Status.Idle;

                //Status = Status.Follow;
            } else
            {
                Status = Status.Idle;
            }
        }

        switch (Status)
        {
            case Status.Attack:
                desiredVelocity = Vector3.zero;
                //if(Leader != null)
                AttackTarget(currentTarget);
                break;
            case Status.Chase:
                Vector3 dir = transform.position - currentTarget.position;
                dir.y = 0;
                if (dir.sqrMagnitude < 0.1f)
                {
                    desiredVelocity = Vector3.zero;
                } else
                {
                    desiredVelocity = currentTarget.position;
                }
                break;
            case Status.Follow:
                Vector3 pos = Leader.position + LeaderOffset;
                desiredVelocity = pos;
                break;
            case Status.Idle:
                desiredVelocity = Vector3.zero;
                break;
        }

        //if(Status == Status.Attack && Leader == null && attackTimer >= 0)
        //{
        //    attackTimer -= Time.deltaTime;
        //}

        //if(Status == Status.Attack && Input.GetKeyDown(KeyCode.Mouse0) && Leader == null)
        //{
        //    LeaderAttackTarget(currentTarget);
        //}
    }

    private void FixedUpdate()
    {
        if (Leader == null && unitData.team == Team.Player)
        {
            return;
        }

        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            return;
        }

        if (desiredVelocity.sqrMagnitude > 0.01f)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.isStopped = false;
            animator.SetBool("Walk", true);
            navMeshAgent.SetDestination(desiredVelocity);
        } else
        {
            animator.SetBool("Walk", false);
            navMeshAgent.isStopped = true;
        }
    }



    void SetData()
    {
        MaxHp = unitData.MaxHp;
        CurrentHp = MaxHp;
        MaxMp = unitData.MaxMp;
        CurrentMp = MaxMp;
        DefaultAttack = unitData.DefulatAttack;
        AttackRange = unitData.Range;
        Critical_Rate = unitData.Critical_Rate;
        Critical_Value = unitData.Critical_Value;
        moveSpeed = unitData.Speed;
        inventoryMax = unitData.InventoryMax;
        attackRateTime = unitData.AttackSpeed;

    }

    bool isVaildTarget(Transform tr)
    {
        if (tr == null)
        {
            return false;
        }
        Unit unit = tr.GetComponent<Unit>();
        if (unit == null || unit.CurrentHp <= 0 || unit.unitData.team == unitData.team)
        {
            return false;
        }

        return true;
    }

    Transform ResearchTarget(bool debug = true)
    {
        Collider[] col = Physics.OverlapSphere(transform.position, unitData.DectecRadius, LayerMask.GetMask("Unit"));


        Transform target = null;

        float max = float.MaxValue;
        for (int i = 0; i < col.Length; i++)
        {
            Unit unit = col[i].GetComponent<Unit>();
            if (unit == null || unit.CurrentHp <= 0 || unit.unitData.team == unitData.team || unit.Status == Status.Dead)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, unit.transform.position);
            if (max > distance)
            {
                max = distance;
                target = unit.transform;

            }
        }

        if (!debug)
        {
            Debug.Log("타겟 반환 " + target.gameObject.name);
        }

        return target;
    }

    void AttackTarget(Transform tr)
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer >= 0)
        {
            return;
        }


        if (tr == null)
        {
            return;
        }


        Unit unit = tr.GetComponent<Unit>();

        if (unit == null)
        {
            return;
        }


        unit.Damage(CurrentAttack(), this);


        animator.SetTrigger("Attack");

        attackTimer = attackRateTime / AttackSpped;
    }

    void LeaderAttackTarget(Transform tr)
    {
        if (attackTimer >= 0)
        {
            return;
        }

        if (tr == null)
        {
            return;
        }


        Unit unit = tr.GetComponent<Unit>();

        if (unit == null)
        {
            return;
        }


        unit.Damage(CurrentAttack(), this);

        animator.SetTrigger("Attack");

        attackTimer = attackRateTime / AttackSpped;
    }

    public float CurrentAttack()
    {
        return Attack + DefaultAttack;
    }


    public bool Damage(float damage, Unit Attacker)
    {
        if (Status == Status.Dead)
        {
            return true;
        }

        if (Attacker.unitData.team == Team.Player)
        {
            float giveExp = unitData.KillExp * (Attacker.CurrentAttack() / MaxHp);
            if (damage > CurrentHp)
                giveExp = unitData.KillExp * (CurrentHp / MaxHp);

            Debug.Log(giveExp);

            Attacker.Exp += giveExp;
            Attacker.ExpCheck();
        }

        CurrentHp -= damage;



        if (CurrentHp <= 0)
        {
            animator.SetTrigger("Death");
            Status = Status.Dead;
            if (Attacker.unitData.team == Team.Player)
            {
                foreach (Unit unit in LeaderManager.instance.units)
                {
                    if (unit.unitData.team == Team.Player)
                    {
                        unit.Exp += unitData.KillExp / 10;
                        unit.ExpCheck();
                    }
                }
            } else
            {
                if (LeaderManager.instance.currentLeaderUnit == this)
                {
                    if (LeaderManager.instance.units.Count <= 1)
                    {
                        Debug.Log("게임 오바");
                    } else
                    {
                        LeaderManager.instance.units.Remove(this);
                        LeaderManager.instance.currentLeaderIndex = 0;
                        LeaderManager.instance.ChangeLeader(LeaderManager.instance.currentLeaderIndex);
                    }
                } else
                {
                    LeaderManager.instance.units.Remove(this);

                }
            }


            //Attacker.currentTarget = ResearchTarget();
            if (unitData.team == Team.Enemy) {
                StartCoroutine(EnemyObjDestory(3f));
            }
            return true;
        }

        TookDamageColor();

        return false;


    }

    public void ExpCheck()
    {
        if (Lv >= 20)
            return;

        while (Exp >= MaxExp)
        {
            Exp -= MaxExp;
            Lv++;
            MaxExp += PlusMaxExp;
            Debug.Log(gameObject.name + "이가 레벨업 했습니다!");
            MaxHp += unitData.HpUp;
            CurrentHp += unitData.HpUp;
            Attack += unitData.AtkUp;

            if (Lv == 1 || Lv % 3 == 0)
            {
                StartCoroutine(GameManager.instance.ChooseSkill(this));
            }
        }
    }

    IEnumerator EnemyObjDestory(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    void TookDamageColor()
    {
        if (MaterialCoroutine != null)
            StopCoroutine(MaterialCoroutine);

        UnitMaterial.color = Color.white;
        MaterialCoroutine = StartCoroutine(SetMaterial());
    }

    IEnumerator SetMaterial()
    {
        UnitMaterial.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        UnitMaterial.color = Color.white;
    }


}
