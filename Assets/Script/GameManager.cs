using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


[System.Serializable]
public class Mission
{
    public List<int> NeedMission = new List<int>()
    {
        0, 0, 0, 0
    };



    public Mission(int e1, int e2, int e3, int ob)
    {
        NeedMission[0] = e1;
        NeedMission[1] = e2;
        NeedMission[2] = e3;
        NeedMission[3] = ob;
    }
}

[System.Serializable]
public class BossSpawn
{
    public GameObject Boss;
    public List<GameObject> subEnemys = new List<GameObject>();
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    public Mission mission;
    public bool MissonClear = false;
    public BossSpawn bossSpawn;
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
                SkillChooser.SkillLv[num]++;
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
        UImanager.instance.PlayerUi.SetActive(true);
        Time.timeScale = 1f;
    }

    public void MissionCheck()
    {
        if (MissonClear)
            return;

        int result = 0;
        for(int i = 0; i < mission.NeedMission.Count; i++)
        {
            result += mission.NeedMission[i];
        }

        if(result == 0)
        {
            MissonClear = true;
            BossTime();
        }
    }

    public void BossTime()
    {
        Instantiate(bossSpawn.Boss, new Vector3(0, 0, 0), Quaternion.identity);

        foreach(GameObject obj in bossSpawn.subEnemys)
        {
            float randX = Random.Range(-3, 4);
            float randZ = Random.Range(-2, 3);
            Instantiate(obj, Vector3.zero + new Vector3(randX, 0, randZ), Quaternion.identity);
        }
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
                    u.Critical_Rate += list[PublicSkillLv[num]];
                    break;
                case 1:
                    u.AttackSpped += list[PublicSkillLv[num]];
                    break;
                case 2:
                    u.MaxHp += list[PublicSkillLv[num]];
                    u.CurrentHp += list[PublicSkillLv[num]];
                    break;
                case 3:
                    u.moveSpeed += u.unitData.Speed * list[PublicSkillLv[num]];
                    break;
                case 4:
                    u.Attack += list[PublicSkillLv[num]];
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
