using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Objects : MonoBehaviour
{
    public Unit unit;
    public List<GameObject> DropItems = new List<GameObject>();
    public bool OpenTheKill = true;
    public Vector3 Rotate;
    public bool isOepn = false;
    public int DropRange = 100;
    public bool trap;



    private void Start()
    {
        if(OpenTheKill)
            unit = GetComponent<Unit>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit") && !OpenTheKill)
        {
            Unit unit = other.gameObject.GetComponent<Unit>();

            if (unit.unitData.team == Team.Player)
            {
                Debug.Log("붙어서 상자 오픈");

                if (trap)
                {
                    List<RangeUnitList> units = unit.ResearchTrapTarget(transform.position, 5f, false);
                    Debug.Log(units.Count);
                    for (int i = 0; i < units.Count; i++)
                    {
                        units[i].unit.Damage(DropRange, null);
                    }
                    ParticleSystem particleSystem = GameObject.Find("TrapEffect").GetComponent<ParticleSystem>();
                    particleSystem.transform.position = transform.position;
                    particleSystem.Play();
                }

                OnOpen();
            }
        }
    }



    public void OnOpen()
    {

        if (isOepn)
            return;


        isOepn = true;
        if(DropItems.Count > 0)
        {
            int dropRan = Random.Range(1, 101);

            if(dropRan <= DropRange)
            {
                int rand = Random.Range(0, DropItems.Count);

                float x = Random.Range(-1.5f, 1.5f);
                float z = Random.Range(-1.5f, 1.5f);

                if(rand <= 5)
                {
                    Quaternion quaternion = Quaternion.Euler(Rotate);
                    Instantiate(DropItems[rand], transform.position, quaternion);
                } else if(rand == 8)
                {
                    Instantiate(DropItems[rand], transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                } else
                {
                    Instantiate(DropItems[rand], transform.position, Quaternion.identity);
                }
            }

        }


        gameObject.SetActive(false);

    }


    


}
