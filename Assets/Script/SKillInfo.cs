using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Skill", menuName = "Game/SKill")]
public class SKillInfo : ScriptableObject
{
    public string SkillName;
    public Sprite icon;
    [Min(1)]
    public int[] MaxTarget = new int[5] { 1, 1, 1, 1, 1 };
    public bool NeedTarget = true;
    public float[] Attack_Value = new float[5];
    public float[] Plus_Skill_Value = new float[5];
    public float[] Plus_Skill2_Value = new float[5];
    public int[] UseMp = new int[5];
    public int[] CoolTime = new int[5];
    public Color IconColor = new Color(255f, 255f, 255f, 1f);
   
}
