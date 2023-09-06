using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walk_speed = 2f;

    [SerializeField]
    private float run_speed = 4f;

    [SerializeField]
    private float max_speed = 2f;

    [SerializeField]
    private float Jumpforce = 250f;

    [SerializeField]
    private float gravity_weight = 1.1f;
    
    [SerializeField]
    private LayerMask GroundLayer;

    [SerializeField]
    float smoothTime = 1F;

    [SerializeField]
    private float crouchHeight = 0.75f;

    [SerializeField]
    private float speedReduction = 0.5f;

    [SerializeField]
    private AudioSource freakZombieSound;

    
    private Vector3 originalScale;
    private float crouchSpeed;
    private float originalWalkSpeed;

    // private float currentSpeed;
    private Rigidbody rigid;
    private CapsuleCollider colli;
    private ShowDebugInfo logInfo;
    private List<string> infos;


    private static PlayerController playerController;

    // 싱글톤 인스턴스를 가져오는 프로퍼티
    public static PlayerController Instance
    {
        get
        {
            if (playerController == null)
            {
                // 씬에서 싱글톤 인스턴스를 찾거나 생성
                playerController = FindObjectOfType<PlayerController>();

                if (playerController == null)
                {
                    // 씬에 없다면 새로 생성
                    GameObject obj = new GameObject("PlayerController");
                    playerController = obj.AddComponent<PlayerController>();
                }
            }

            return playerController;
        }
    }

    public AudioSource FreakZombieSound
    {
        get
        {
            return freakZombieSound;
        }
    }


    private void Awake()
    {
        infos = new List<string>();
        rigid = GetComponent<Rigidbody>();
        colli = GetComponent<CapsuleCollider>();
        logInfo = GetComponent<ShowDebugInfo>();
        crouchSpeed = walk_speed * speedReduction;
        originalWalkSpeed = walk_speed;
        originalScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        infos.Clear();
        Move();                
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

        Crouch();
        max_speed = (Input.GetButton("Run")) ? ((dirZ > 0f) ? run_speed : walk_speed) : walk_speed;     // 후진인경우에는 달리기를 허용하지 않음

        Vector3 force = new Vector3(dirX, 0f, dirZ);
        rigid.AddRelativeForce(force, ForceMode.Impulse);
        
        logInfo.SetLogItem("dirX", dirX.ToString());
        logInfo.SetLogItem("Rigidboyd.velocity.magnitude", Mathf.RoundToInt(rigid.velocity.magnitude).ToString());
        logInfo.SetLogItem("IsGround()", IsGround().ToString());
        logInfo.SetLogItem("HorizontalMovement", horizontalMovement.ToString());
    }

    private void Crouch()
    {
        // Stands player up to full height
        // Brings walkSpeed back up to original speed
        if (Input.GetKey(KeyCode.C))
        {            
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(originalScale.x, originalScale.y*crouchHeight, originalScale.z), Time.deltaTime * 15);
            walk_speed = crouchSpeed;
            
            Debug.DrawRay(transform.position, Vector3.up * 2f, Color.blue);
        }        
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 15);
            walk_speed = originalWalkSpeed;
        }
    }

    public float MoveSpeed
    {
        get
        {
            return rigid.velocity.magnitude;
        }
    }


    public float MaxSpeed
    {
        get
        {
            return max_speed;
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
        float dist = colli.bounds.extents.y + 0.1f;
        bool ret = Physics.Raycast(transform.position, Vector3.down, out hit, dist, GroundLayer);

        Debug.DrawRay(transform.position, Vector3.down * dist, (ret) ? Color.green : Color.red);

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
