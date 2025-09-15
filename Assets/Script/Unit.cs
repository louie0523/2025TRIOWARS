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
    public float MaxHp;
    public float CurrentHp;
    public float MaxMp;
    public float CurrentMp;
    public float DefaultAttack;
    public float Attack;
    public float AttackRange;
    public float Critical_Rate;
    public float Critical_Value;
    public float moveSpeed;
    public int inventoryMax;
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
    


    private void Start()
    {
        SetData();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Update()
    {
        if(debuff == Debuff.Stun)
        {
            stunTimer -= Time.deltaTime;
            if(stunTimer > 0)
            {
                return;
            } else
            {
                debuff = Debuff.None;
            }
        } 

        if(Status == Status.Dead)
        {
            return;
        }

        if(currentTarget == null || isVaildTarget(currentTarget))
        {
            currentTarget = ResearchTarget();
        }

        if(currentTarget != null)
        {
            float dis = Vector3.Distance(transform.position, currentTarget.position);
            if(dis < AttackRange)
            {
                Status = Status.Attack;
            } else
            {
                Status = Status.Chase;
            }
        } else
        {
            if(Leader != null)
            {
                Status = Status.Follow;
            } else
            {
                Status = Status.Idle;
            }
        }

        switch(Status)
        {
            case Status.Attack:
                desiredVelocity = Vector3.zero;
                Debug.Log("공격중");
                break;
            case Status.Chase:
                Vector3 dir = transform.position - currentTarget.position;
                dir.y = 0;
                if(dir.sqrMagnitude < 0.1f)
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
        }
    }

    private void FixedUpdate()
    {
        if(Leader == null)
        {
            return;
        }

        if(navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            return;
        }

        if(desiredVelocity.sqrMagnitude > 0.01f)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(desiredVelocity);
        } else
        {
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
    }

    bool isVaildTarget(Transform tr)
    {
        if(tr == null)
        {
            return false;
        }
        Unit unit = tr.GetComponent<Unit>();
        if (unit == null || unit.CurrentHp < 0 || unit.unitData.team == unitData.team)
        {
            return false;
        }

        return true;
    }

    Transform ResearchTarget()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, unitData.DectecRadius, LayerMask.GetMask("Unit"));

        Transform target = null;

        float max = float.MaxValue;
        for(int i = 0; i < col.Length; i++)
        {
            Unit unit = col[i].GetComponent<Unit>();
            if(unit == null || unit.CurrentHp <= 0 || unit.unitData.team == unitData.team)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, unit.transform.position);
            if(max > distance)
            {
                max = distance;
                target = unit.transform;
            }
        }

        return target;
    }


}
