using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public static UImanager instance;

    public GameObject SkillUI;
    public GameObject PrivateSkillParent;
    public GameObject PublicSkillParent;
    public GameObject CurrentPlayerParent;
    public Sprite DownSkillLv;
    public Sprite SkillLv;
    public Text MessageText;
    public float FadeTIme = 2.5f;

    [Header("플레이어 UI")]
    public GameObject PlayerUi;
    public GameObject CharacterParent;
    public Slider HpSlider;
    public Slider MpSlider;
    public Slider ExpSlider;
    public GameObject SkillGrid;
    public Text LvText;


    private Coroutine MessageCorutine;

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

    private void Update()
    {
        if(PlayerUi.activeSelf)
        {
            HpSlider.value = LeaderManager.instance.currentLeaderUnit.CurrentHp / LeaderManager.instance.currentLeaderUnit.MaxHp;
            MpSlider.value = LeaderManager.instance.currentLeaderUnit.CurrentMp / LeaderManager.instance.currentLeaderUnit.MaxMp;
            ExpSlider.value = LeaderManager.instance.currentLeaderUnit.Exp / LeaderManager.instance.currentLeaderUnit.MaxExp;

            Text HpText = HpSlider.transform.GetChild(2).GetComponent<Text>();
            Text MpText = MpSlider.transform.GetChild(2).GetComponent<Text>();
            Text ExpText = ExpSlider.transform.GetChild(2).GetComponent<Text>();

            HpText.text = $"{(int)LeaderManager.instance.currentLeaderUnit.CurrentHp} / {(int)LeaderManager.instance.currentLeaderUnit.MaxHp}";
            MpText.text = $"{(int)LeaderManager.instance.currentLeaderUnit.CurrentMp} / {(int)LeaderManager.instance.currentLeaderUnit.MaxMp}";
            ExpText.text = $"{(int)LeaderManager.instance.currentLeaderUnit.Exp} / {(int)LeaderManager.instance.currentLeaderUnit.MaxExp}";

            LvText.text = $"Lv.{LeaderManager.instance.currentLeaderUnit.Lv}";

            SkillUISet();
        }
    }

    public void OpenSkillUI(Unit unit)
    {
        SkillUI.SetActive(true);
        PlayerUi.SetActive(false);

        for(int i = 0; i < CurrentPlayerParent.transform.childCount; i++)
        {
            if(i == (int)unit.unitData.playerType)
            {
                CurrentPlayerParent.transform.GetChild(i).gameObject.SetActive(true);
            } else
            {
                CurrentPlayerParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        SetPrivateSkillUI();
        SetPublicSkillUI();



        Time.timeScale = 0;
    }

    public void SetPrivateSkillUI()
    {

        Unit unit = GameManager.instance.SkillChooser;

        for (int i = 0; i < PrivateSkillParent.transform.childCount; i++)
        {
            Image icon = PrivateSkillParent.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
            icon.sprite = unit.unitData.HaveSkill[i].icon;
            icon.color = unit.unitData.HaveSkill[i].IconColor;
            Text skillName = PrivateSkillParent.transform.GetChild(i).transform.GetChild(1).GetComponent<Text>();
            skillName.text = unit.unitData.HaveSkill[i].SkillName;
            GameObject Slv = PrivateSkillParent.transform.GetChild(i).transform.GetChild(2).gameObject;
            for (int j = 0; j < Slv.transform.childCount; j++)
            {
                Image lvImage = Slv.transform.GetChild(j).GetComponent<Image>();
                if (unit.SkillLv[i] > j)
                    lvImage.sprite = SkillLv;
                else
                    lvImage.sprite = DownSkillLv;
            }
        }
    }

    public void SetPublicSkillUI()
    {
        for (int i = 0; i < PublicSkillParent.transform.childCount; i++)
        {
            GameObject Slv = PublicSkillParent.transform.GetChild(i).transform.GetChild(2).gameObject;
            for (int j = 0; j < Slv.transform.childCount; j++)
            {
                Image lvImage = Slv.transform.GetChild(j).GetComponent<Image>();
                if (GameManager.instance.PublicSkillLv[i] > j)
                    lvImage.sprite = SkillLv;
                else
                    lvImage.sprite = DownSkillLv;
            }
        }
    }
    
    public void Message(string message)
    {
        if (MessageCorutine != null)
            StopCoroutine(MessageCorutine);

        MessageText.text = message;
        MessageText.color = new Color(MessageText.color.r, MessageText.color.g, MessageText.color.b, 1f);
        MessageCorutine = StartCoroutine(MessageFade());
    }

    IEnumerator MessageFade()
    {
        float timer = FadeTIme;
        while(timer > 0 )
        {
            timer -= Time.deltaTime;
            MessageText.color = new Color(MessageText.color.r, MessageText.color.g, MessageText.color.b, timer / FadeTIme);

            yield return null;
        }

        MessageText.text = "";
    }


    public void CurrentPlayerIconSet()
    {
        for(int i = 0; i < 3; i++)
        {
            if(i == LeaderManager.instance.currentLeaderIndex)
            {
                CharacterParent.transform.GetChild(i).gameObject.SetActive(true);
            } else
            {
                CharacterParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

    }

    public void SkillUISet()
    {
        Unit unit = LeaderManager.instance.currentLeaderUnit;

        for (int i = 0; i < 4; i++)
        {
            GameObject skill = SkillGrid.transform.GetChild(i).gameObject;
            Image icon = skill.transform.GetChild(0).GetComponent<Image>();
            Text cost = skill.transform.GetChild(1).GetComponent<Text>();
            Image cool = skill.transform.GetChild(2).GetComponent<Image>();
            icon.color = unit.unitData.HaveSkill[i].IconColor;
            if (unit.SkillLv[i] >= 1)
            {
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1f);
                icon.sprite = unit.unitData.HaveSkill[i].icon;
                cost.text = unit.unitData.HaveSkill[i].UseMp[unit.SkillLv[i]-1].ToString();
                cool.fillAmount = unit.skillCooltime[i] / unit.unitData.HaveSkill[i].CoolTime[unit.SkillLv[i]-1];
            } else
            {
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
                cost.text = "";
                cool.fillAmount = 0;
            }

        }
    }


    


}
