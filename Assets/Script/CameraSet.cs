using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSet : MonoBehaviour
{
    //public GameObject Target;

    public Vector3 offset;
    public float smoothSpeed;


    private void LateUpdate()
    {
        Vector3 targetPostion = LeaderManager.instance.currentLeaderUnit.transform.position + offset;
        Vector3 smoothPostion = Vector3.Lerp(transform.position, targetPostion, smoothSpeed * Time.deltaTime);
        transform.position = smoothPostion;

        transform.LookAt(LeaderManager.instance.currentLeaderUnit.transform);
    }
}
