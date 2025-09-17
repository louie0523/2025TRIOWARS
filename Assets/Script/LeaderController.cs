using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LeaderController : MonoBehaviour
{
    private CharacterController characterController;
    public Unit unit;



    private void Awake()
    {
        unit = GetComponent<Unit>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
        Rotation();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            unit.UseSkill(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            unit.UseSkill(1);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            unit.UseSkill(2);

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            unit.UseSkill(3);

        }
    }

    void Move()
    {
        if(characterController == null)
        {
            characterController = GetComponent<CharacterController>();
            return;
        }

        if (unit.ArrowRaing)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v).normalized;

        characterController.Move(move * unit.moveSpeed * Time.deltaTime);

        bool Walking = move.magnitude > 0f;
        unit.animator.SetBool("Walk", Walking);
    }

    void Rotation()
    {
        if(Time.timeScale <= 0)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;
        if(plane.Raycast(ray, out rayLength))
        {
            Vector3 point = ray.GetPoint(rayLength);

            this.transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
        }
    }

}
