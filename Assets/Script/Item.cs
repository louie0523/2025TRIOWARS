using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemInfo item;


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            Unit unit = other.GetComponent<Unit>();
            if(unit == LeaderManager.instance.currentLeaderUnit)
            {
                if(unit.inventory.Count < unit.inventoryMax)
                {
                    unit.AddItem(item);
                    Debug.Log("¾ÆÀÌÅÛ È¹µæ");
                    Destroy(gameObject);
                } else {
                    UImanager.instance.Message("¾ÆÀÌÅÛ ½½·ÏÀÌ °¡µæ Ã¡½À´Ï´Ù.");
                    Debug.Log("¾ÆÀÌÅÛ ½½·ÔÀÌ °¡µæ Ã¡½À´Ï´Ù.");
                }
            }
        }
    }
}
