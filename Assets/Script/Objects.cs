using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Objects : MonoBehaviour
{
    public Unit unit;
    public List<GameObject> DropItems = new List<GameObject>();
    public bool OpenTheKill = true;
    public Vector3 Rotate;
    public bool isOepn = false;
    public int DropRange = 100;



    private void Start()
    {
        unit = GetComponent<Unit>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Unit") && !OpenTheKill)
        {
            Unit unit = collision.gameObject.GetComponent<Unit>();
            if (unit == LeaderManager.instance.currentLeaderUnit)
            {
                Debug.Log("붙어서 상자 오픈");
                OnOpen();
            }
        }
    }


    public void OnOpen()
    {

        if (isOepn)
            return;


        isOepn = true;

        int rand = Random.Range(0, DropItems.Count);

        float x = Random.Range(-1.5f, 1.5f);
        float z = Random.Range(-1.5f, 1.5f);

        Quaternion quaternion = Quaternion.Euler(Rotate)
            ;
        Instantiate(DropItems[rand], transform.position, quaternion);

        gameObject.SetActive(false);

    }


}
