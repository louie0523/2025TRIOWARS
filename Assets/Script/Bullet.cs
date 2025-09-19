using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float distance;
    public Unit unit;

    private void Update()
    {
            Shot();
    }

    void Shot()
    {

        float dis = Vector3.Distance(transform.position, unit.transform.position);
        if (dis > distance)
            Destroy(gameObject);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            Unit vic = other.GetComponent<Unit>();
            if(vic.unitData.team != Team.Player)
            {
                vic.Damage((unit.CurrentAttack() * unit.unitData.HaveSkill[1].Attack_Value[unit.SkillLv[1]-1]), unit, true);
            }
        }
    }


}
