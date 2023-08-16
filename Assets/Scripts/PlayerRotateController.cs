using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotateController : MonoBehaviour
{
    [SerializeField]
    private Transform mainCamTransform;

    [SerializeField]
    private GameObject[] weapons;
    
    [Tooltip("카메라 앵글 머리위쪽 한계 각도")]
    [SerializeField]
    private float topAngleView = 60;

    [Tooltip("카메라 앵글 발 아래쪽 한계 각도")]
    [SerializeField]
    private float bottomAngleView = -45;
    
    [SerializeField]
    private float mouseSensitvity_notAiming = 300;

    [SerializeField]
    private float mouseSensitvity_aiming = 50;

    [SerializeField]
    private float xCameraSpeed = 0.1f;

    [SerializeField]
    private float yRotationSpeed = 0.1f;

    [SerializeField]
    private float mouseSensitvity = 0;

    [SerializeField]
    private float headMovementSpeed = 1f;

    [SerializeField]
    private AudioSource LeftfootStep;

    [SerializeField]
    private AudioSource RightfootStep;


    private ShowDebugInfo logInfo;


    private int currentWeaponIndex;

    private float wantedYRotation;
    private float currentYRotation;
    private float rotationYVelocity;
    

    private float wantedCameraXRotation;
    private float currentCameraXRotation;
    private float cameraXVelocity;
    

    private float timer;
    private int int_timer;
    private float zRotation;
    private float wantedZ;
    private float upDown;
    private float wantedY;
    
    private float timerToRotateZ = 1f;
    private float playerSpeed;
    private float sign;

    float deltaTime = 0.0f;

    private void Awake()
    {        
        currentWeaponIndex = 0;
        Cursor.lockState = CursorLockMode.Locked;
        logInfo = GetComponent<ShowDebugInfo>();
        sign = Mathf.Sign(zRotation);
}

    private void Start()
    {
        upDown = mainCamTransform.localPosition.y;
    }

    private void FixedUpdate()
    {
        /*
	    * Reduxing mouse sensitvity if we are aiming.
	    */
        mouseSensitvity = 4; //(Input.GetAxis("Fire2") != 0) ? mouseSensitvity_aiming : mouseSensitvity_notAiming;

        ApplyingStuff();
    }

    void Update()
    {
        MouseInputMovement();

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        logInfo.SetLogItem("deltaTime", deltaTime.ToString());

        playerSpeed = GetComponent<PlayerController>().MoveSpeed;
        if ( playerSpeed > 1f )
            HeadMovement();
    }

    private void HeadMovement()
    {
        headMovementSpeed = playerSpeed;
        timer += headMovementSpeed * Time.deltaTime;
        int_timer = Mathf.RoundToInt(timer);
        logInfo.SetLogItem("PlayerSpeed", playerSpeed.ToString());
        logInfo.SetLogItem("timer", timer.ToString());
        logInfo.SetLogItem("int_timer", int_timer.ToString());
        if (int_timer % 2 == 0)
        {
            wantedY = wantedZ = -2;            
        }
        else
        {
            wantedY = wantedZ = 2;
        }

        zRotation = Mathf.Lerp(zRotation, wantedZ, Time.deltaTime * timerToRotateZ);
        // upDown = mainCamTransform.localPosition.y + Mathf.Lerp(mainCamTransform.localPosition.y, wantedY, Time.deltaTime * timerToRotateZ);
    }


    private void MouseInputMovement()
    {
        wantedYRotation += Input.GetAxis("Mouse X") * mouseSensitvity;
        wantedCameraXRotation -= Input.GetAxis("Mouse Y") * mouseSensitvity;
        wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);
    }

    /*
    * Smoothing the wanted movement.
    * Calling the waeponRotation form here, we are rotating the waepon from this script.
    * Applying the camera wanted rotation to its transform.
    */
    void ApplyingStuff()
    {

        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
        currentCameraXRotation = Mathf.SmoothDampAngle(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);

         //WeaponRotation();

        transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
        mainCamTransform.localRotation = Quaternion.Euler(currentCameraXRotation, 0, zRotation);
        PlayFootStep();

        mainCamTransform.localPosition = new Vector3(mainCamTransform.localPosition.x, upDown, mainCamTransform.localPosition.z);
        logInfo.SetLogItem("mainCamTransform.localPosition", mainCamTransform.localPosition.ToString());
        Debug.Log(upDown);
        Debug.Log(mainCamTransform.localPosition);
    }

    
    private void PlayFootStep()
    {
        float temp = Mathf.Sign(zRotation);
        if (sign == temp)
            return;

        if (sign > 0)
            LeftfootStep.Play();
        else
            RightfootStep.Play();

        sign = temp;
    }
}
