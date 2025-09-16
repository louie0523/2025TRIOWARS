using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    public List<Unit> AllUnits = new List<Unit>();
    public List<int> PublicSkillLv = new List<int>()
    {
        0,0,0,0,0
    };
    public Unit SkillChooser;
    public bool Choosing = false;
    public List<float> Critical_Up = new List<float>()
    {
        0.1f, 0.1f, 0.1f, 0.1f, 0.1f
    };
    public List<float> Attack_Speed_Up = new List<float>()
    {
        0.5f, 0.5f, 0.5f, 0.5f, 1f
    };
    public List<float> MaxHP_Up= new List<float>()
    {
        200,400,600,800,1000
    };
    public List<float> Speed_Up = new List<float>()
    {
        0.5f, 0.5f, 0.2f, 0.3f, 0.5f,
    };
    public List<float> Attck_Up = new List<float>()
    {
        0.2f,0.2f,0.2f,0.2f,0.2f
    };



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator ChooseSkill(Unit unit)
    {
        if (Choosing)
            yield return new WaitWhile(() => Choosing);

        SkillChooser = unit;
        Choosing = true;
        UImanager.instance.OpenSkillUI(unit);
    }

    public void SKillBtn(int num)
    {

        if (!Choosing)
            return;


        if(num <= 3)
        {

            if (SkillChooser.SkillLv[num] < 5)
            {

            } else
            {
                UImanager.instance.Message("해당 유닛 스킬은 이미 최고 레벨 입니다.");
                return;
            }

        } else
        {
            num -= 4;
            if (PublicSkillLv[num] < 5)
            {
                PublicSkillStatusUp(num);
            } else
            {
                UImanager.instance.Message("해당 공통 스킬은 이미 최고 레벨 입니다.");
                return;
            }
        }

        Choosing = false;
        UImanager.instance.SkillUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PublicSkillStatusUp(int num)
    {
        List<float> list = new List<float>();
        switch(num)
        {
            case 0:
                list = Critical_Up;
                break;
            case 1:
                list = Attack_Speed_Up;
                break;
            case 2:
                list = MaxHP_Up;
                break;
            case 3:
                list = Speed_Up;
                break;
            case 4:
                list = Attck_Up;
                break;
        }


        foreach(Unit u in UnitsGet(0))
        {
            switch (num)
            {
                case 0:
                    u.Critical_Rate += list[SkillChooser.SkillLv[num]];
                    break;
                case 1:
                    u.AttackSpped += list[SkillChooser.SkillLv[num]];
                    break;
                case 2:
                    u.MaxHp += list[SkillChooser.SkillLv[num]];
                    u.CurrentHp += list[SkillChooser.SkillLv[num]];
                    break;
                case 3:
                    u.moveSpeed += u.unitData.Speed * list[SkillChooser.SkillLv[num]];
                    break;
                case 4:
                    u.Attack += list[SkillChooser.SkillLv[num]];
                    break;
            }

        }
        PublicSkillLv[num]++;



    }

    public List<Unit> UnitsGet(int num)
    {
        Unit[] units = GameObject.FindObjectsOfType<Unit>();


        List<Unit> unit = new List<Unit>();
        if(num == 0)
        {
            foreach (Unit u in units)
            {
                if (u.unitData.team == Team.Player)
                    unit.Add(u);
            }
        } else if(num == 1)
        {
            foreach (Unit u in units)
            {
                if (u.unitData.team == Team.Enemy)
                    unit.Add(u);
            }
        } else if(num == 2)
        {
            foreach (Unit u in units)
            {
                unit.Add(u);
            }
        }

        return unit;

    }
}
