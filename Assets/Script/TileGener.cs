using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGener : MonoBehaviour
{
    public GameObject obj;
    public int X;
    public int Z;
    public Vector3 offset;
    public Vector3 Rotate;

    private void Start()
    {
        for(int i = 0; i < X; i ++)
        {
            for(int j = 0; j < Z; j++)
            {
                Vector3 spawnPos = new Vector3(i * offset.x, offset.y, j * offset.z);
                Instantiate(obj, spawnPos + transform.position, Quaternion.Euler(Rotate), this.transform);
            }
        }
    }
}
