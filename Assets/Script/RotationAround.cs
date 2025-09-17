using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAround : MonoBehaviour
{
    public float timer = 0f;
    public float duration = 3f;
    public Vector3 offset = Vector3.up; // ȸ�� ��

    private void Update()
    {
        timer += Time.deltaTime; // ��� ����
        float t = (timer % duration) / duration; // 0~1 �ݺ�

        float angle = Mathf.Lerp(0, 360, t);

        transform.localRotation = Quaternion.AngleAxis(angle, offset);
    }
}
