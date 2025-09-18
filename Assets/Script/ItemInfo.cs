using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Hp,
    Mp,
    Cool,
    Cost,
    Resque,
}

[CreateAssetMenu(fileName = "new item", menuName = "Game/item")]
public class ItemInfo : ScriptableObject
{
    public string ItemName;
    public Sprite ItemIcon;
    public ItemType itemType;
    public int MaxCpunt = 10;
    public float Value;
    public Color ItemColor;
}
