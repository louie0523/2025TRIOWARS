using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public bool Leader;


    private void Start()
    {
        SetData();
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
}
