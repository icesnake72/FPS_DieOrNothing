using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walk_speed = 2f;

    [SerializeField]
    private float run_speed = 4f;

    [SerializeField]
    private float max_speed = 2f;

    // [SerializeField]
    // private TextMeshProUGUI debugText;

    [SerializeField]
    private float Jumpforce = 250f;

    [SerializeField]
    private float gravity_weight = 1.1f;
    
    [SerializeField]
    private LayerMask GroundLayer;

    [SerializeField]
    float smoothTime = 1F;


    // private float currentSpeed;
    private Rigidbody rigid;
    private ShowDebugInfo logInfo;
    private List<string> infos;

    

    private void Awake()
    {
        infos = new List<string>();
        rigid = GetComponent<Rigidbody>();
        logInfo = GetComponent<ShowDebugInfo>();
    }

    private void FixedUpdate()
    {
        infos.Clear();
        Move();        
        //ShowInfo();
    }

    // Update is called once per frame
    void Update()
    {        
        Jump();        
    }

    private bool OverMaxSpeed
    {
        get
        {
            return rigid.velocity.magnitude > max_speed;
        }
    }

    private void Move()
    {
        // currentSpeed = rigid.velocity.magnitude;

        Vector2 horizontalMovement = new Vector2(rigid.velocity.x, rigid.velocity.z);
        // if (horizontalMovement.magnitude > max_speed)
        if ( OverMaxSpeed )
        {
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= max_speed;
        }

        float gravity = rigid.velocity.y;
        if (rigid.velocity.y < 0)
            gravity *= gravity_weight;            
        

        rigid.velocity = new Vector3( horizontalMovement.x,
        //    // rigid.velocity.y,
            gravity,
            horizontalMovement.y );

        if (IsGround())
        {
            Vector3 slowDown = Vector3.zero;            
            rigid.velocity = Vector3.SmoothDamp(rigid.velocity,
                    new Vector3(0, gravity, 0),
                    ref slowDown,
                    smoothTime);
        }

        float dirX = Input.GetAxis("Horizontal");
        float dirZ = Input.GetAxis("Vertical");
        max_speed = (Input.GetButton("Run")) ? ((dirZ > 0f) ? run_speed : walk_speed) : walk_speed;     // 후진인경우에는 달리기를 허용하지 않음

        Vector3 force = new Vector3(dirX, 0f, dirZ);
        rigid.AddRelativeForce(force, ForceMode.Impulse);
        
        logInfo.SetLogItem("dirX", dirX.ToString());
        logInfo.SetLogItem("Rigidboyd.velocity.magnitude", Mathf.RoundToInt(rigid.velocity.magnitude).ToString());
        logInfo.SetLogItem("IsGround()", IsGround().ToString());
        logInfo.SetLogItem("HorizontalMovement", horizontalMovement.ToString());
    }

    public float MoveSpeed
    {
        get
        {
            return rigid.velocity.magnitude;
        }
    }

    //private void AddInfo(string info)
    //{
    //    infos.Add(info);
    //}

    //private void ShowInfo()
    //{
    //    string info = "";
    //    foreach(string line in infos)
    //    {
    //        info += line;
    //        info += "\n";
    //    }

    //    debugText.SetText(info);
    //}

    private bool IsGround()
    {
        RaycastHit hit;        
        bool ret = Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, GroundLayer);
        Debug.DrawRay(transform.position, Vector3.down*hit.distance, Color.red);        

        return ret;
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGround())
        {            
            rigid.AddRelativeForce(Vector3.up * Jumpforce);
        }        
     }
}
