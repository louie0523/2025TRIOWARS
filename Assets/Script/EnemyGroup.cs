using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    public GameObject MainEnemy;
    public GameObject SubEnemy;
    public int EnemyMin, EnemyMax;
    public int SubEnemyCount = 3;

    //public List<Unit> Enemys = new List<Unit>();
    public bool AllDie = false;
    public float ResqueTimeMIn, ResqueTimeMax;

    private void Start()
    {
        EnemySetting();
    }

    private void Update()
    {
        if(transform.childCount == 0 && !AllDie)
        {
            AllDie = true;
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        float rnad = Random.Range(ResqueTimeMIn, ResqueTimeMax);
        yield return new WaitForSeconds(rnad);
        EnemySetting();
        AllDie = false;
    }

    void EnemySetting()
    {
        int rand = Random.Range(EnemyMin, EnemyMax + 1);
        
        for(int i = 0; i < rand; i++)
        {
            float X = Random.Range(-2f, 2f);
            float Z = Random.Range(-2f, 2f);
            Vector3 vec = transform.position + new Vector3(X, 0, Z);
            Unit unit = Instantiate(MainEnemy, vec, Quaternion.identity, transform).GetComponent<Unit>();
        }

        for(int i = 0; i < SubEnemyCount; i++)
        {
            float X = Random.Range(-2f, 2f);
            float Z = Random.Range(-2f, 2f);
            Vector3 vec = transform.position + new Vector3(X, 0, Z);
            Unit unit = Instantiate(SubEnemy, vec, Quaternion.identity, transform).GetComponent<Unit>();
        }
    }

    //void SetChildClear()
    //{
    //    if(transform.childCount >= 1)
    //    {
    //        foreach(Transform tr in transform)
    //        {
    //            Destroy(tr.gameObject);
    //        }
    //    }
    //}
}
