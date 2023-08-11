using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float max_speed = 3f;

    [SerializeField]
    private TextMeshProUGUI debugText;

    [SerializeField]
    private float Jumpforce = 260f;

    [SerializeField]
    private float gravity_weight = 20f;
    
    [SerializeField]
    private LayerMask GroundLayer;


    private float currentSpeed;
    private Rigidbody rigid;

    private List<string> infos;

    

    private void Awake()
    {
        infos = new List<string>();
        rigid = GetComponent<Rigidbody>();        
    }

    private void FixedUpdate()
    {
        infos.Clear();
        Move();        
        ShowInfo();
    }

    // Update is called once per frame
    void Update()
    {        
        Jump();        
    }

    private void Move()
    {
        currentSpeed = rigid.velocity.magnitude;

        Vector2 horizontalMovement = new Vector2(rigid.velocity.x, rigid.velocity.z);
        if (horizontalMovement.magnitude > max_speed)
        {
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= max_speed;
        }

        float gravity = rigid.velocity.y;
        if (rigid.velocity.y < 0)
            gravity *= gravity_weight;

        rigid.velocity = new Vector3( horizontalMovement.x,
            // rigid.velocity.y,
            gravity,
            horizontalMovement.y );

        if (IsGround())
        {
            Vector3 slowDown = Vector3.zero;
            float smoothTime = 1F;
            rigid.velocity = Vector3.SmoothDamp(rigid.velocity,
                    new Vector3(0, rigid.velocity.y, 0),
                    ref slowDown,
                    smoothTime);
        }

        float dirX = Input.GetAxis("Horizontal");
        float dirZ = Input.GetAxis("Vertical");

        Vector3 force = new Vector3(dirX, 0f, dirZ);
        rigid.AddRelativeForce(force, ForceMode.Impulse);        
                
        

        AddInfo($"dirX : {dirX}, dirZ : {dirZ}");                
        AddInfo($"Rigidboyd.velocity = {rigid.velocity.ToString()}");        
        AddInfo($"IsGround() : {IsGround()}");
    }

    private void AddInfo(string info)
    {
        infos.Add(info);
    }

    private void ShowInfo()
    {
        string info = "";
        foreach(string line in infos)
        {
            info += line;
            info += "\n";
        }

        debugText.SetText(info);
    }

    private bool IsGround()
    {
        RaycastHit hit;        
        bool ret = Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, GroundLayer);
        Debug.DrawRay(transform.position, Vector3.down*hit.distance, Color.red);
        AddInfo($"hit : {hit.ToString()}");
        AddInfo($"ret : {ret}");       

        return ret;
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGround())
        {
            // Jumpforce
            // rigid.AddForce(new Vector3(0, Jumpforce, 0));
            rigid.AddRelativeForce(Vector3.up * Jumpforce);            
        }        
     }
}
