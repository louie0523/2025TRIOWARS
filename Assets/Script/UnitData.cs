using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Team
{
    Player,
    Enemy,
    Object,
}

public enum PlayerType
{
    Mele,
    Ranger,
    Magic
}

public enum EnemyType
{

    Mele1,
    Mele2,
    Mele3,
    Range1,
    Range2,
    Range3,
    Boos1,
    Boss2,
    Boss3
}


[CreateAssetMenu(fileName = "new Unit", menuName = "Game/Unit")]
public class UnitData : ScriptableObject
{
    public Team team;
    public PlayerType playerType;
    public EnemyType enemyType;
    public float MaxHp;
    public float MaxMp;
    public float DefulatAttack;
    public float Range;
    public float AttackSpeed;
    public float Critical_Rate;
    public float Critical_Value = 2;
    public float Speed;
    public int InventoryMax = 4;
    [Header("성장치")]
    public float HpUp;
    public float AtkUp;
    [Header("상호작용")]
    public float DectecRadius = 8f;
    public float FriendRadius = 1.5f;
}
