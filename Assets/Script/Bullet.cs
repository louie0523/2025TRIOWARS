using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform Target;
    public float lifeTime;
    public float SetTime;
    public float speed;

    private void Update()
    {
        if (gameObject.activeSelf)
            Shot();


    }

    void Shot()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            gameObject.SetActive(false);
        }

        if (Target != null)
        {
            Vector3 dir = (Target.position - transform.position).normalized;
            transform.position = dir * speed * Time.deltaTime;
            transform.forward = dir;
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

    }

}
