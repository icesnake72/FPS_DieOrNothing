using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private float max_speed = 10f;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float dirX = Input.GetAxis("Horizontal");
        float dirZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(dirX*speed, Physics.gravity.y, dirZ*speed);
        //rigid.velocity = movement.normalized * Time.deltaTime;
        rigid.AddForce(movement, ForceMode.Impulse);
        if (Mathf.Abs(rigid.velocity.x) > max_speed || Mathf.Abs(rigid.velocity.z) > max_speed)
            rigid.velocity = new Vector3(max_speed, Physics.gravity.y, max_speed);
    }

    private void Jump()
    {

    }
}
