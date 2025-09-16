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

    public void OpenSkillUI(Unit unit)
    {
        SkillUI.SetActive(true);

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

    


}
