using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObj : MonoBehaviour
{
    public float lifetime = 5f;
    public Unit unit;

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            Unit vic = other.GetComponent<Unit>();
            if (vic.unitData.team != Team.Player)
            {
                vic.Damage((unit.CurrentAttack()), unit, true);
            }
        }
    }
}
