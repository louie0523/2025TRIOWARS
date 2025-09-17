using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class RangeUnitList
{
    public Unit unit;
    public float Rnage;

    public RangeUnitList(Unit unit, float Rnage)
    {
        this.unit = unit;
        this.Rnage = Rnage;
    }
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
    public List<float> skillCooltime = new List<float>()
    {
        0,0,0,0
    };
    public List<GameObject> SkillEffect = new List<GameObject>();
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
    public bool Fight = false;
    public float FihgtTimer = 0;
    public float HpGetTimer = 0;
    public float ManaGetTime = 2.5f;
    public float ManaTimer = 0;
    public bool ArrowRaing;

    public bool Load = false;
    public GameObject Meshobj;
    public Material UnitMaterial;
    private Coroutine MaterialCoroutine;
    public Transform StartPos;
    public Transform Endpos;
    public LineRenderer lineRenderer;
    private Coroutine LineRenderCoroutine;
    public bool SkillUseing = false;
    public GameObject StunObj;






    private void Awake()
    {
        if (unitData.team == Team.Player)
        {
            DontDestroyOnLoad(gameObject);
        }else
        {
            if (UnitMaterial != null)
            {
                UnitMaterial = Instantiate(UnitMaterial);
                Meshobj.GetComponent<Renderer>().material = UnitMaterial;  // 렌더러에 인스턴스된 머티리얼 적용
            }

            StunObj = transform.GetChild(1).gameObject;
        }

    }
    private void Start()
    {
        if (!Load)
        {
            SetData();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = transform.GetChild(0).GetComponent<Animator>();
            if(unitData.team == Team.Player)
                lineRenderer = transform.GetChild(1).GetComponent<LineRenderer>();
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
                StunObj.SetActive(true);
                return;
            } else
            {
                StunObj.SetActive(false);
                debuff = Debuff.None;
            }
        }

        if (ArrowRaing)
            return;

        if (Status == Status.Dead)
        {
            return;
        }

        SkillCoolDown();

   
        FihgtTimer += Time.deltaTime;
        if(FihgtTimer >= 5 && unitData.team == Team.Player)
        {
            HpGetTimer += Time.deltaTime;
            if (HpGetTimer >= 1)
            {
                Heal(MaxHp * 0.05f);
                HpGetTimer = 0f;
            }
               
        }

        ManaTimer += Time.deltaTime;
        if(ManaTimer > ManaGetTime && unitData.team == Team.Player)
        {
            MpHeal(MaxMp * 0.01f);
            ManaTimer = 0f;
        }
        

        if (lineRenderer != null && lineRenderer.positionCount >= 2)
        {
            lineRenderer.SetPosition(0, StartPos.position);
            lineRenderer.SetPosition(1, Endpos.position);
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
            if(Status != Status.Dead)
                animator.SetBool("Walk", true);
            navMeshAgent.SetDestination(desiredVelocity);
        } else
        {
            if (Status != Status.Dead)
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

    void SkillCoolDown()
    {
        for(int i = 0; i < 4; i++)
        {
            if (SkillLv[i] >= 1 && skillCooltime[i] > 0)
            {
                skillCooltime[i] -= Time.deltaTime;
            }
        }
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

    Transform ResearchTarget(bool isEnemy = true, bool debug = true)
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

        if(AttackRange >= 4)
            LineRednerAttack(unit.transform);

        animator.SetTrigger("Attack");

        attackTimer = attackRateTime / AttackSpped;
    }


    public float CurrentAttack()
    {
        return Attack + DefaultAttack;
    }


    public bool Damage(float damage, Unit Attacker, bool skillDamage = false)
    {
        if (Status == Status.Dead)
        {
            return true;
        }


        if(Attacker.Critical_Rate > 0)
        {
            int rand = Random.Range(1, 101);
            if(((int)Attacker.Critical_Rate * 100) > rand)
            {
                Debug.Log("크리티컬!");
                damage *= Attacker.Critical_Value;
            }
        }

        if (Attacker.unitData.team == Team.Player)
        {
            float giveExp = unitData.KillExp * (Attacker.CurrentAttack() / MaxHp);
            if (damage > CurrentHp)
                giveExp = unitData.KillExp * (CurrentHp / MaxHp);

            Attacker.Exp += giveExp;
            Attacker.ExpCheck();
        }

        CurrentHp -= damage;

        if (skillDamage)
            Debug.Log($"{gameObject.name}가 {Attacker.gameObject.name}에게 {damage}의 피해를 입었습니다!");



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


        Attacker.FihgtTimer = 0f;
        FihgtTimer = 0f;
        TookDamageColor();

        return false;


    }

    public void MpHeal(float heal, Unit Healer = null)
    {
        if (CurrentHp <= 0)
        {
            return;
        }

        CurrentMp += heal;
        if (CurrentMp > MaxMp)
            CurrentMp = MaxMp;


    }

    public void Heal(float heal, Unit Healer = null)
    {
        if(CurrentHp <= 0)
        {
            return;
        }

        CurrentHp += heal;
        if (CurrentHp > MaxHp)
            CurrentHp = MaxHp;


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

            if (Lv == 1 || Lv % 2 != 0)
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

    void LineRednerAttack(Transform tr)
    {
        if (LineRenderCoroutine != null)
            StopCoroutine(LineRenderCoroutine);

        lineRenderer.positionCount = 2;
        Endpos = tr;

        LineRenderCoroutine = StartCoroutine(ActiveLineRender((attackRateTime / AttackSpped) - 0.05f));



    }

    IEnumerator ActiveLineRender(float time)
    {
        yield return new WaitForSeconds(time);

        lineRenderer.positionCount = 0;
    }

     List<RangeUnitList> ResearchSkillTarget(Vector3 pos,float range, bool debug = true)
    {
        Collider[] col = Physics.OverlapSphere(pos, range, LayerMask.GetMask("Unit"));


        List<RangeUnitList> targetList = new List<RangeUnitList>();
        for (int i = 0; i < col.Length; i++)
        {
            Unit unit = col[i].GetComponent<Unit>();
            if (unit == null || unit.CurrentHp <= 0 || unit.unitData.team == unitData.team )
            {
                continue;
            }

            
            float distance = Vector3.Distance(pos, unit.transform.position);
            targetList.Add(new RangeUnitList(unit, distance));
        }

        List<RangeUnitList> sortedList = targetList.OrderBy(x => x.Rnage).ToList();


        return sortedList;
        
    }


    public void UseSkill(int num)
    {
        if (SkillLv[num] <=  0)
        {
            Debug.Log(SkillLv[num]);
            UImanager.instance.Message("해당 스킬칸은 비어있습니다.");
            return;
        }

        if (skillCooltime[num] > 0)
        {
            UImanager.instance.Message("해당 스킬은 아직 재사용 대기시간이 남아있습니다.");
            return;
        }

        if(CurrentMp < unitData.HaveSkill[num].UseMp[SkillLv[num] - 1])
        {
            UImanager.instance.Message("마나가 부족합니다.");
            return;
        }

        if(unitData.HaveSkill[num].NeedTarget && currentTarget == null)
        {
            UImanager.instance.Message("현재 대상이 없습니다.");
            return;
        }

        switch(unitData.playerType)
        {
            case PlayerType.Mele:
                switch (num)
                {
                    case 0:
                        StartCoroutine(참격(num, unitData.HaveSkill[num]));
                        break;
                    case 1:
                        StartCoroutine(가르기(num, unitData.HaveSkill[num]));
                        break;
                    case 2:
                        StartCoroutine(파쇄격(num, unitData.HaveSkill[num]));
                        break;
                    case 3:
                        StartCoroutine(격파(num, unitData.HaveSkill[num]));
                        break;
                }
                break;
            case PlayerType.Ranger:
                switch (num)
                {
                    case 0:
                        StartCoroutine(퀵샷(num, unitData.HaveSkill[num]));
                        break;
                    case 1:
                        StartCoroutine(포착(num, unitData.HaveSkill[num]));
                        break;
                    case 2:
                        StartCoroutine(쐐기살(num, unitData.HaveSkill[num]));
                        break;
                    case 3:
                        StartCoroutine(화살비(num, unitData.HaveSkill[num]));
                        break;
                }
                break;
        }

    }

    public void SetStun(float timer)
    {
        stunTimer = timer;
        debuff = Debuff.Stun;
    }

    public IEnumerator 참격(int num, SKillInfo skill)
    {
        SkillUseing = true;
        skillCooltime[num] = skill.CoolTime[SkillLv[num] - 1];
        CurrentMp -= skill.UseMp[SkillLv[num] - 1];


        animator.SetTrigger("Skill" + (num + 1));
        List<RangeUnitList> unitLists =  ResearchSkillTarget(transform.position + new Vector3(0, 1, 1), 2f);
        
        if(unitLists.Count > 0)
        {
            unitLists[0].unit.Damage((CurrentAttack() * skill.Attack_Value[SkillLv[num] - 1]), this);
        } else
        {
            Debug.Log("타겟 못찾음");
        }
        yield return new WaitForSeconds(0.1f);
        SkillUseing = false;
    }

    public IEnumerator 가르기(int num, SKillInfo skill)
    {
        if (SkillUseing)
        {
            Debug.Log("스킬이 이미 사용중, 반환함");
            yield return null;
        }

        SkillUseing = true;
        skillCooltime[num] = skill.CoolTime[SkillLv[num]-1];
        CurrentMp -= skill.UseMp[SkillLv[num] - 1];


        animator.SetTrigger("Skill" + (num + 1));
        List<RangeUnitList> unitLists = ResearchSkillTarget(transform.position + new Vector3(0, 1, 1), 3f);

        Debug.Log(unitLists.Count);
        if (unitLists.Count > 0)
        {
            for(int i = 0; i < unitLists.Count; i++)
            {
                if(i <= skill.MaxTarget[SkillLv[num] - 1]){
                    unitLists[i].unit.Damage((CurrentAttack() * skill.Attack_Value[SkillLv[num]-1]), this);
                } else
                {
                    break;
                }
            }
        }
        else
        {
            Debug.Log("타겟 못찾음");
        }
        yield return new WaitForSeconds(0.1f);
        SkillUseing = false;
    }

    public IEnumerator 파쇄격(int num, SKillInfo skill)
    {
        SkillUseing = true;
        skillCooltime[num] = skill.CoolTime[SkillLv[num] - 1];
        CurrentMp -= skill.UseMp[SkillLv[num] - 1];


        animator.SetTrigger("Skill" + (num - 1));
        List<RangeUnitList> unitLists = ResearchSkillTarget(transform.position + new Vector3(0, 1, 1), 2f);

        if (unitLists.Count > 0)
        {
            unitLists[0].unit.Damage((CurrentAttack() * skill.Attack_Value[SkillLv[num]-1]), this);
            Debug.Log($"{unitLists[0].unit.gameObject.name}이가 첫번째로 타격당함");
            List<RangeUnitList> NextunitLists = ResearchSkillTarget(unitLists[0].unit.transform.position + new Vector3(0, 1, -1), 2f);
            for (int i = 0; i < NextunitLists.Count; i++)
            {
                if (NextunitLists[i].unit != unitLists[0].unit)
                {
                    Debug.Log($"{NextunitLists[i].unit.gameObject.name}이가 {i+2}번째로 타격당함");
                    NextunitLists[i].unit.Damage((CurrentAttack() * skill.Plus_Skill_Value[SkillLv[num]-1]), this);
                }
            }
        }
        else
        {
            Debug.Log("타겟 못찾음");
        }
        
        yield return new WaitForSeconds(0.1f);
        SkillUseing = false;
    }

    public IEnumerator 격파(int num, SKillInfo skill)
    {
        SkillUseing = true;
        skillCooltime[num] = skill.CoolTime[SkillLv[num] - 1];
        CurrentMp -= skill.UseMp[SkillLv[num] - 1];


        animator.SetTrigger("Skill" + (num + 1));
        List<RangeUnitList> unitLists = ResearchSkillTarget(transform.position + new Vector3(0, 1, 1), 2f);

        if (unitLists.Count > 0)
        {
            for (int i = 0; i < unitLists.Count; i++)
            {
                if (i <= skill.MaxTarget[SkillLv[num] - 1])
                {
                    unitLists[i].unit.Damage((CurrentAttack() * skill.Attack_Value[SkillLv[num]-1]), this);
                    int rand = Random.Range(1, 101);
                    if(rand <= (int)skill.Plus_Skill_Value[SkillLv[num] - 1])
                    {
                        unitLists[i].unit.SetStun(2f);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            Debug.Log("타겟 못찾음");
        }

        yield return new WaitForSeconds(0.1f);
        SkillUseing = false;
    }

    public IEnumerator 퀵샷(int num, SKillInfo skill)
    {
        SkillUseing = true;
        skillCooltime[num] = skill.CoolTime[SkillLv[num] - 1];
        CurrentMp -= skill.UseMp[SkillLv[num] - 1];


        animator.SetTrigger("Skill" + (num + 1));
        Unit unit = currentTarget.GetComponent<Unit>();

        for(int i = 0; i < skill.Plus_Skill_Value[SkillLv[num]-1]; i++)
        {
            if(unit.Status != Status.Dead)
            {
                animator.SetTrigger("Skill" + (num + 1));
                unit.Damage(CurrentAttack() * skill.Attack_Value[SkillLv[num] - 1], this);
                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(0.1f);
        SkillUseing = false;
    }

    public IEnumerator 포착(int num, SKillInfo skill)
    {
        if (SkillUseing)
        {
            Debug.Log("스킬이 이미 사용중, 반환함");
            yield return null;
        }

        SkillUseing = true;
        skillCooltime[num] = skill.CoolTime[SkillLv[num] - 1];
        CurrentMp -= skill.UseMp[SkillLv[num] - 1];


        animator.SetTrigger("Skill" + (num + 1));
        List<RangeUnitList> unitLists = ResearchSkillTarget(transform.position, AttackRange);

        Debug.Log(unitLists.Count);
        if (unitLists.Count > 0)
        {
            for (int i = 0; i < unitLists.Count; i++)
            {
                if (i <= skill.MaxTarget[SkillLv[num] - 1])
                {
                    unitLists[i].unit.Damage((CurrentAttack() * skill.Attack_Value[SkillLv[num] - 1]), this, true);
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            Debug.Log("타겟 못찾음");
        }
        yield return new WaitForSeconds(0.1f);
        SkillUseing = false;
    }

    public IEnumerator 쐐기살(int num, SKillInfo skill)
    {
        if (SkillUseing)
        {
            Debug.Log("스킬이 이미 사용중, 반환함");
            yield return null;
        }

        SkillUseing = true;
        skillCooltime[num] = skill.CoolTime[SkillLv[num] - 1];
        CurrentMp -= skill.UseMp[SkillLv[num] - 1];


        animator.SetTrigger("Skill" + (num + 1));

        float coneAngle = 45f;
        Vector3 firePos = transform.position + new Vector3(0, 1, 1);
        float startAnmge = -coneAngle / 2f;
        float angleStep = coneAngle / (skill.Plus_Skill_Value[SkillLv[num] - 1] - 1);

        for(int i = 0; i < skill.Plus_Skill_Value[SkillLv[num] - 1]; i++)
        {

            float currentAngle = startAnmge + angleStep * i;
            Quaternion rotate = Quaternion.Euler(0, currentAngle, 0);
            Vector3 dir = rotate * transform.forward;

            GameObject arrow = Instantiate(SkillEffect[0], firePos, Quaternion.LookRotation(dir));
            Bullet bullet = arrow.GetComponent<Bullet>();
            bullet.unit = this;
            bullet.distance = skill.Plus_Skill2_Value[SkillLv[num] - 1];
            Rigidbody rg = arrow.GetComponent<Rigidbody>();
            rg.velocity = dir * 7f;
        }



        yield return new WaitForSeconds(0.1f);
        SkillUseing = false;
    }

    public IEnumerator 화살비(int num, SKillInfo skill)
    {
        if (SkillUseing)
        {
            Debug.Log("스킬이 이미 사용중, 반환함");
            yield return null;
        }

        SkillUseing = true;
        skillCooltime[num] = skill.CoolTime[SkillLv[num] - 1];
        CurrentMp -= skill.UseMp[SkillLv[num] - 1];


        animator.SetTrigger("Skill2");


        Unit unit = currentTarget.GetComponent<Unit>();

        float wait = SkillLv[num] <= 2 ? 0.5f : 0.25f;
        ArrowRaing = SkillLv[num] <= 2 ? true : false;

        for (int i = 0; i < skill.Plus_Skill_Value[SkillLv[num]-1]; i++)
        {
            Vector3 pos = unit.transform.position + new Vector3(0, 5, 0);
            GameObject arrows = Instantiate(SkillEffect[1], pos, Quaternion.identity);
            float x = skill.Plus_Skill2_Value[SkillLv[num] - 1];
            float z = skill.Plus_Skill2_Value[SkillLv[num] - 1];
            foreach (Transform obj in arrows.transform)
            {
                float randX = Random.Range(-x, x);
                float randZ = Random.Range(-z, z);
                obj.transform.position += new Vector3(randX, 0, randZ);
            }
            SkillObj skobj = arrows.GetComponent<SkillObj>();
            skobj.lifetime = 1f;
            skobj.unit = this;

            Rigidbody rg = arrows.GetComponent<Rigidbody>();
            rg.useGravity = true;
            rg.velocity = Vector3.down * 7;

            if (SkillLv[num] >= 1)
            {
                yield return new WaitForSeconds(wait);
            }else
            {
                break;
            }

        }

        ArrowRaing = false;
        yield return new WaitForSeconds(0.1f);
        SkillUseing = false;
    }



}
