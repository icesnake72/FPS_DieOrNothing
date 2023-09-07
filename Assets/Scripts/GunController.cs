using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using TMPro;

public enum GunAutoStyles
{
    nonautomatic,
    automatic
}

public class GunController : MonoBehaviour
{
    //[SerializeField] private GameObject firingPoint;

    //[SerializeField] private float fireRate = 0.3f;

    //[SerializeField] private ParticleSystem muzzle;

    //[SerializeField] private AudioSource shootSound;

    //[SerializeField] private GameObject sparkEffect;

    [Tooltip("Selects type of waepon to shoot rapidly or one bullet per click.")]
    [SerializeField] private GunAutoStyles currentStyle;

    [SerializeField] private Animator handsAnimator;

    private Camera cameraComponent;
    private Camera secondCamera;    
    private Transform playerTransform;
    private Transform cameraTransform;

    private float curFireRate;
    private RaycastHit hit;

    
    private GameObject bulletSpawnPlace;

    [Tooltip("Bullet prefab that this waepon will shoot.")]
    [SerializeField] private GameObject bullet;

    [Tooltip("Rounds per second if weapon is set to automatic rafal.")]
    [SerializeField] private float roundsPerSecond;


    [Tooltip("Audios for shootingSound, and reloading.")]
    [SerializeField] private AudioSource shoot_sound_source, reloadSound_source;
    [Tooltip("Sound that plays after successful attack bullet hit.")]
    [SerializeField] private static AudioSource hitMarker;


    [Header("보통 모드에서 해당 무기의 마우스 민감도")]
    [Tooltip("Sensitvity of this gun while not aiming.")]
    [SerializeField] private float mouseSensitvity_notAiming = 10;
    
    [Tooltip("Sensitvity of this gun while aiming.")]
    [SerializeField] private float mouseSensitvity_aiming = 5;
    
    [Tooltip("Sensitvity of this gun while running.")]
    [SerializeField] private float mouseSensitvity_running = 4;

    [Header("근접 공격 모드에서 해당 무기의 마우스 민감도")]
    [SerializeField] private float meeleNotAimLook = 2f;
    [SerializeField] private float meeleAimLook = 1.6f;
    [SerializeField] private float meeleRun = 1f;

    private float startLook, startAim, startRun;
        
    private bool reloading;     // 총알 리로딩중인지 체크
    private bool meeleAttack;   // 근거리 격투중인지 체크      
    private bool aiming;        // 조준중인지 체크 


    [Header("Animation names")]
    public string reloadAnimationName = "Player_Reload";
    public string aimingAnimationName = "Player_AImpose";
    public string meeleAnimationName = "Character_Malee";



    [Header("Bullet properties")]
    [Tooltip("무기가 얼마나 많은 총알을 생성할지 알려주는 사전 설정 값입니다.")]
    [SerializeField] private float bulletsIHave = 20;

    [Tooltip("소총 내부에 무기가 생성되는 총알의 양을 알려주는 사전 설정 값입니다.")]
    [SerializeField] private float bulletsInTheGun = 5;

    [Tooltip("탄창 하나에 얼마나 많은 총알을 실을 수 있는지 알려주는 사전 설정 값입니다.")]
    [SerializeField] private float amountOfBulletsPerLoad = 5;


    [Header("Player movement properties")]
    [Tooltip("속도는 총을 통해 결정됩니다. 모든 총이 동일한 속성이나 무게를 갖고 있는 것은 아니므로 여기에서 속도를 설정해야 합니다.")]
    [SerializeField] private int walkingSpeed = 3;
    [SerializeField] private int runningSpeed = 5;

        
    [Header("reload time after anima")]
    [Tooltip("Time that passes after reloading. Depends on your reload animation length, because reloading can be interrupted via meele attack or running. So any action before this finishes will interrupt reloading.")]
    [SerializeField] private float reloadChangeBulletsTime;

        
    private Vector3 currentGunPosition;

    [Header("Gun Positioning")]
    [Tooltip("Vector 3 position from player SETUP for NON AIMING values")]
    [SerializeField] private Vector3 restPlacePosition;

    [Tooltip("Vector 3 position from player SETUP for AIMING values")]
    [SerializeField] private Vector3 aimPlacePosition;

    [Tooltip("Time that takes for gun to get into aiming stance.")]
    [SerializeField] private float gunAimTime = 0.1f;


    [Header("Gun Rotation")]
    private Vector2 velocityGunRotate;
    private float gunWeightX, gunWeightY;

    [Tooltip("The time waepon will lag behind the camera view best set to '0'.")]
    [SerializeField] private float rotationLagTime = 0f;

    private float rotationLastY;    
    private float rotationDeltaY;
    private float angularVelocityY;

    private float rotationLastX;
    private float rotationDeltaX;
    private float angularVelocityX;

    [Tooltip("Value of forward rotation multiplier.")]
    [SerializeField] private Vector2 forwardRotationAmount = Vector2.one;



    [HideInInspector][SerializeField] private float recoilAmount_z = 0.5f;
    [HideInInspector][SerializeField] private float recoilAmount_x = 0.5f;
    [HideInInspector][SerializeField] private float recoilAmount_y = 0.5f;

    [Header("Recoil Not Aiming")]
    [Tooltip("Recoil amount on that AXIS while NOT aiming")]
    [SerializeField] private float recoilAmount_z_non = 0.5f;

    [Tooltip("Recoil amount on that AXIS while NOT aiming")]
    [SerializeField] private float recoilAmount_x_non = 0.5f;

    [Tooltip("Recoil amount on that AXIS while NOT aiming")]
    [SerializeField] private float recoilAmount_y_non = 0.5f;

    [Header("Recoil Aiming")]
    [Tooltip("Recoil amount on that AXIS while aiming")]
    [SerializeField] private float recoilAmount_z_ = 0.5f;

    [Tooltip("Recoil amount on that AXIS while aiming")]
    [SerializeField] private float recoilAmount_x_ = 0.5f;

    [Tooltip("Recoil amount on that AXIS while aiming")]
    [SerializeField] private float recoilAmount_y_ = 0.5f;

    private float velocity_x_recoil;
    private float velocity_y_recoil;
    private float velocity_z_recoil;
        
    private float currentRecoilXPos;
    private float currentRecoilYPos;
    private float currentRecoilZPos;

    private Vector3 velV;

    [Header("")]
    [Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
    [SerializeField] private float recoilOverTime_z = 0.5f;

    [Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
    [SerializeField] private float recoilOverTime_x = 0.5f;

    [Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
    [SerializeField] private float recoilOverTime_y = 0.5f;

    [Header("Gun Precision")]
    [Tooltip("Gun rate precision when player is not aiming. THis is calculated with recoil.")]
    [SerializeField] private float gunPrecision_notAiming = 200.0f;

    [Tooltip("Gun rate precision when player is aiming. THis is calculated with recoil.")]
    [SerializeField] private float gunPrecision_aiming = 100.0f;

    [Tooltip("FOV of first camera when NOT aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    [SerializeField] private float cameraZoomRatio_notAiming = 60;

    [Tooltip("FOV of first camera when aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    [SerializeField] private float cameraZoomRatio_aiming = 40;

    [Tooltip("FOV of second camera when NOT aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    [SerializeField] private float secondCameraZoomRatio_notAiming = 60;

    [Tooltip("FOV of second camera when aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    [SerializeField] private float secondCameraZoomRatio_aiming = 40;

    [HideInInspector]
    [SerializeField] private float gunPrecision;


    [Tooltip("Array of muzzel flashes, randmly one will appear after each bullet.")]
    [SerializeField] private GameObject[] muzzelFlash;

    [Tooltip("Place on the gun where muzzel flash will appear.")]
    [SerializeField] private GameObject muzzelSpawn;


    [Header("Crosshair properties")]
    [SerializeField] private Texture horizontal_crosshair, vertical_crosshair;
    [SerializeField] private Vector2 top_pos_crosshair, bottom_pos_crosshair, left_pos_crosshair, right_pos_crosshair;
    [SerializeField] private Vector2 size_crosshair_vertical = new Vector2(1, 1), size_crosshair_horizontal = new Vector2(1, 1);
        
    private Vector2 expandValues_crosshair;
    private float fadeout_value = 1;

    private GameObject holdFlash;
    private GameObject holdSmoke;

    private Vector3 gunPosVelocity;
    private float cameraZoomVelocity;
    private float secondCameraZoomVelocity;


    private float waitTillNextFire;


    public Animator HandsAnimator
    {
        get
        {
            return handsAnimator;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        // 단 한개만 존재하는 게임 오브젝트에서 다음과 같이 접근할 수 있다 
        // playerTransform = PlayerRotateController.PlayerTransform();
        // cameraTransform = PlayerRotateController.CameraTransform();

        // 위의 방법말고도 게임에서 단 한개만 존재하는 유일한 오브젝트가 확실하다면 해당 오브젝트를 싱글톤으로 생성해서 접근해도 된다.
        playerTransform = PlayerRotateController.Instance.transform; //GetComponent<Transform>();
        cameraTransform = PlayerRotateController.Instance.MainCameraTransform;
        cameraComponent = cameraTransform.GetComponent<Camera>();
        secondCamera = PlayerRotateController.Instance.SecondCamera;

        bulletSpawnPlace = PlayerRotateController.Instance.BulletSpawn;
        hitMarker = transform.Find("hitMarkerSound").GetComponent<AudioSource>();

        float a = PlayerController.Instance.MoveSpeed;

        startLook = mouseSensitvity_notAiming;
        startAim = mouseSensitvity_aiming;
        startRun = mouseSensitvity_running;

        rotationLastY = PlayerRotateController.Instance.CurrentYRotation;
        rotationLastX = PlayerRotateController.Instance.CurrentCameraXRotation;

        // curFireRate = fireRate;        
    }


    private void FixedUpdate()
    {
        RotationGun();

        MeeleAnimationsStates();

        Aiming();
    }

    // Update is called once per frame
    private void Update()
    {
        Animations();
        GunSensitivityToPlayerRotateScript();
        PositionGun();
        
        Shooting();
        MeeleAttack();
        LockCameraWhileMelee();

        Sprint(); //if we have the gun you sprint from here, if we are gunless then its called from movement script

        CrossHairExpansionWhenWalking();
    }


    /*
     * Changing some values if we are aiming, like sensitity, zoom racion and position of the waepon.
	 */
    private void Aiming()
    {
        
        //if aiming
        if (Input.GetAxis("Fire2") != 0 && !reloading && !meeleAttack)
        {
            gunPrecision = gunPrecision_aiming;
            recoilAmount_x = recoilAmount_x_;   // auto : 0.005
            recoilAmount_y = recoilAmount_y_;   // auto : 0.005
            recoilAmount_z = recoilAmount_z_;   // auto : 0.01
            currentGunPosition = Vector3.SmoothDamp(currentGunPosition, aimPlacePosition, ref gunPosVelocity, gunAimTime);
            cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_aiming, ref cameraZoomVelocity, gunAimTime);
            secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_aiming, ref secondCameraZoomVelocity, gunAimTime);
        }
        //if not aiming
        else
        {
            gunPrecision = gunPrecision_notAiming;
            recoilAmount_x = recoilAmount_x_non;    // auto : 0.01
            recoilAmount_y = recoilAmount_y_non;    // auto : 0.01
            recoilAmount_z = recoilAmount_z_non;    // auto : 0.02
            currentGunPosition = Vector3.SmoothDamp(currentGunPosition, restPlacePosition, ref gunPosVelocity, gunAimTime);
            cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_notAiming, ref cameraZoomVelocity, gunAimTime);
            secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_notAiming, ref secondCameraZoomVelocity, gunAimTime);
        }
    }


    /* 
	 * Changes the max speed that player is allowed to go.
	 * Also max speed is connected to the animator which will trigger the run animation.
	 */
    void Sprint()
    {
        // Running();  so i can find it with CTRL + F
        if (Input.GetAxis("Vertical") > 0 && Input.GetAxisRaw("Fire2") == 0 && meeleAttack == false && Input.GetAxisRaw("Fire1") == 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                PlayerController.Instance.MaxSpeed = (PlayerController.Instance.MaxSpeed == walkingSpeed) ? runningSpeed : walkingSpeed;
            }
        }
        else
        {
            PlayerController.Instance.MaxSpeed = walkingSpeed;
        }
    }


    private void RotationGun()
    {
        // 현재 플레이어의 Y축 회전 변동값과 플레이어 카메라의 X축 회전 변동값을 구한다 
        rotationDeltaY = PlayerRotateController.Instance.CurrentYRotation - rotationLastY;
        rotationDeltaX = PlayerRotateController.Instance.CurrentCameraXRotation - rotationLastX;

        // 다음 변동값을 구하기위해 현재 회전값을 저장해둠 
        rotationLastY = PlayerRotateController.Instance.CurrentYRotation;
        rotationLastX = PlayerRotateController.Instance.CurrentCameraXRotation;

        // 이번 FixedUpdate()에 회전(변경)해야할 값을 구함 
        angularVelocityY = Mathf.Lerp(angularVelocityY, rotationDeltaY, Time.deltaTime * 5);
        angularVelocityX = Mathf.Lerp(angularVelocityX, rotationDeltaX, Time.deltaTime * 5);

        gunWeightX = Mathf.SmoothDamp(gunWeightX, PlayerRotateController.Instance.CurrentCameraXRotation, ref velocityGunRotate.x, rotationLagTime);
        gunWeightY = Mathf.SmoothDamp(gunWeightY, PlayerRotateController.Instance.CurrentYRotation, ref velocityGunRotate.y, rotationLagTime);

        // 최종적으로 현재 총 오브젝트의 회전값을 변경함 (플레이어의 회전이 끝나고 나서 angularVelocityX, angularVelocityY 만큼 더 이동함)
        transform.rotation = Quaternion.Euler(gunWeightX + (angularVelocityX * forwardRotationAmount.x),
                                               gunWeightY + (angularVelocityY * forwardRotationAmount.y),
                                               0);

        // 플레이어가 회전한만큼 무기를 회전시킴 
        // transform.rotation = Quaternion.Euler(gunWeightX, gunWeightY, 0);
    }

    
    /*
	* Fetching if any current animation is running.
	* Setting the reload animation upon pressing R.
	*/
    private void Animations()
    {
        if (handsAnimator)
        {
            int playerMaxSpeed = (int)PlayerController.Instance.MaxSpeed;
            reloading = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(reloadAnimationName);

            handsAnimator.SetFloat("walkSpeed", PlayerController.Instance.MoveSpeed);
            handsAnimator.SetBool("aiming", Input.GetButton("Fire2"));
            handsAnimator.SetInteger("maxSpeed", playerMaxSpeed);
            if (Input.GetKeyDown(KeyCode.R) && playerMaxSpeed < 5 && !reloading && !meeleAttack/* && !aiming*/)
            {
                StartCoroutine("Reload_Animation");
            }
        }
    }

    /*
	* Reloading, setting the reloading to animator,
	* Waiting for 2 seconds and then seeting the reloaded clip.
	*/
    IEnumerator Reload_Animation()
    {
        if (bulletsIHave > 0 && bulletsInTheGun < amountOfBulletsPerLoad && !reloading/* && !aiming*/)
        {
            // [ START ] 리로드 사운드 재생 시작뒤 애니메이션 재생 
            if (reloadSound_source.isPlaying == false && reloadSound_source != null)
            {
                if (reloadSound_source)
                    reloadSound_source.Play();
                else
                    print("'Reload Sound Source' missing.");
            }

            handsAnimator.SetBool("reloading", true);
            yield return new WaitForSeconds(0.5f);
            handsAnimator.SetBool("reloading", false);
            // [ END ] 리로드 사운드 재생 시작뒤 애니메이션 재생 



            yield return new WaitForSeconds(reloadChangeBulletsTime - 0.5f);
            int playerMaxSpeed = (int)PlayerController.Instance.MaxSpeed;
            if (meeleAttack == false && playerMaxSpeed != runningSpeed)
            {                
                // if (playerTransform.GetComponent<PlayerMovementScript>()._freakingZombiesSound)
                if ( PlayerController.Instance.FreakZombieSound )
                    PlayerController.Instance.FreakZombieSound.Play();
                else
                    print("Missing Freaking Zombies Sound");

                if (bulletsIHave - amountOfBulletsPerLoad >= 0)
                {
                    bulletsIHave -= amountOfBulletsPerLoad - bulletsInTheGun;
                    bulletsInTheGun = amountOfBulletsPerLoad;
                }
                else if (bulletsIHave - amountOfBulletsPerLoad < 0)
                {
                    float valueForBoth = amountOfBulletsPerLoad - bulletsInTheGun;
                    if (bulletsIHave - valueForBoth < 0)
                    {
                        bulletsInTheGun += bulletsIHave;
                        bulletsIHave = 0;
                    }
                    else
                    {
                        bulletsIHave -= valueForBoth;
                        bulletsInTheGun += valueForBoth;
                    }
                }
            }
            else
            {
                reloadSound_source.Stop();

                print("Reload interrupted via meele attack");
            }
        }
    }


    //
    // PlayerRotateController Script의 마우스 민감도 항목에 해당 총(GunController Script)의 마우스 민감도를 설정한다 
    //
    private void GunSensitivityToPlayerRotateScript()  //GiveCameraScriptMySensitvity()
    {
        PlayerRotateController.Instance.MouseSensitivityNotAimimg = mouseSensitvity_notAiming;
        PlayerRotateController.Instance.MouseSensitivityAimimg = mouseSensitvity_aiming;
    }




    private void PositionGun()
    {
        transform.position = Vector3.SmoothDamp( transform.position,
            cameraTransform.transform.position -
            (cameraTransform.transform.right   * (currentGunPosition.x + currentRecoilXPos)) +
            (cameraTransform.transform.up      * (currentGunPosition.y + currentRecoilYPos)) +
            (cameraTransform.transform.forward * (currentGunPosition.z + currentRecoilZPos)),
            ref velV,
            0);

        // pmS.cameraPosition = new Vector3(currentRecoilXPos, currentRecoilYPos, 0);
        
        currentRecoilXPos = Mathf.SmoothDamp(currentRecoilXPos, 0, ref velocity_x_recoil, recoilOverTime_x);
        currentRecoilYPos = Mathf.SmoothDamp(currentRecoilYPos, 0, ref velocity_y_recoil, recoilOverTime_y);
        currentRecoilZPos = Mathf.SmoothDamp(currentRecoilZPos, 0, ref velocity_z_recoil, recoilOverTime_z);
    }



    /*
	 * Checking if meeleAttack is already running.
	 * If we are not reloading we can trigger the MeeleAttack animation from the IENumerator.
	 */
    private void MeeleAnimationsStates()
    {
        if (handsAnimator)
        {
            // 근접 공격 모드, Aiming 모드 체크 
            meeleAttack = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(meeleAnimationName);
            aiming = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(aimingAnimationName);
        }
    }

    /*
    * User inputs meele attack with Q in keyboard start the coroutine for animation and damage attack.
    */
    void MeeleAttack()
    {

        if (Input.GetKeyDown(KeyCode.Q) && !meeleAttack)
        {
            StartCoroutine("AnimationMeeleAttack");
        }
    }

    /*
	* Sets meele animation to play.
	*/
    IEnumerator AnimationMeeleAttack()
    {
        handsAnimator.SetBool("meeleAttack", true);
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        handsAnimator.SetBool("meeleAttack", false);
    }

    /*
    * Setting the mouse sensitvity lower when meele attack and waits till it ends.
    */
    void LockCameraWhileMelee()
    {
        if (meeleAttack)
        {
            mouseSensitvity_notAiming = meeleNotAimLook;
            mouseSensitvity_aiming = meeleAimLook;
            mouseSensitvity_running = meeleRun;
        }
        else
        {
            mouseSensitvity_notAiming = startLook;
            mouseSensitvity_aiming = startAim;
            mouseSensitvity_running = startRun;
        }
    }


    private void Shooting()
    {
        if (!meeleAttack)
        {
            if (currentStyle == GunAutoStyles.nonautomatic)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    ShootMethod();
                }
            }

            if (currentStyle == GunAutoStyles.automatic)
            {
                if (Input.GetButton("Fire1"))
                {
                    ShootMethod();
                }
            }
        }
        waitTillNextFire -= roundsPerSecond * Time.deltaTime;
    }

    private void ShootMethod()
    {
        if (waitTillNextFire <= 0 && !reloading && PlayerController.Instance.MaxSpeed < 5)
        {

            if (bulletsInTheGun > 0)
            {

                int randomNumberForMuzzelFlash = Random.Range(0, 5);
                if (bullet)
                    Instantiate(bullet, bulletSpawnPlace.transform.position, bulletSpawnPlace.transform.rotation);
                else
                    print("Missing the bullet prefab");
                holdFlash = Instantiate(muzzelFlash[randomNumberForMuzzelFlash], muzzelSpawn.transform.position /*- muzzelPosition*/, muzzelSpawn.transform.rotation * Quaternion.Euler(0, 0, 90)) as GameObject;
                holdFlash.transform.parent = muzzelSpawn.transform;
                if (shoot_sound_source)
                    shoot_sound_source.Play();
                else
                    print("Missing 'Shoot Sound Source'.");

                RecoilMath();

                waitTillNextFire = 1;
                bulletsInTheGun -= 1;
            }

            else
            {
                //if(!aiming)
                StartCoroutine("Reload_Animation");
                //if(emptyClip_sound_source)
                //	emptyClip_sound_source.Play();
            }

        }
    }

    /*
	 * Called from ShootMethod();, upon shooting the recoil amount will increase.
	 */
    public void RecoilMath()
    {        
        currentRecoilXPos -= (Random.value - 0.5f) * recoilAmount_x;
        currentRecoilYPos -= (Random.value - 0.5f) * recoilAmount_y;
        currentRecoilZPos -= recoilAmount_z;

        PlayerRotateController.Instance.WantedCameraXRotation -= Mathf.Abs(currentRecoilYPos * gunPrecision);
        PlayerRotateController.Instance.WantedYRotation -= (currentRecoilXPos * gunPrecision);

        expandValues_crosshair += new Vector2(6, 12);
    }

    /*
     * Setting the number of bullets to the hud UI gameobject if there is one.
     * And drawing CrossHair from here.
     */
    [Tooltip("HUD bullets to display bullet count on screen. Will be find under name 'HUD_bullets' in scene.")]
    private TextMeshProUGUI HUD_bullets;

    public TextMeshProUGUI BulletsInfo
    {
        set { HUD_bullets = value; }
    }

    void OnGUI()
    {       
        if ( HUD_bullets )
            HUD_bullets.text = bulletsIHave.ToString() + " - " + bulletsInTheGun.ToString();

        DrawCrosshair();
    }



    /*
	 * Drawing the crossHair.
	 */
    private void DrawCrosshair()
    {
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, fadeout_value);
        if (Input.GetAxis("Fire2") == 0)
        {
            //if not aiming draw
            GUI.DrawTexture(new Rect(PercentageToScreen.vec2(left_pos_crosshair).x + PercentageToScreen.position_x(-expandValues_crosshair.x) + Screen.width / 2,
                                     Screen.height / 2 + PercentageToScreen.vec2(left_pos_crosshair).y,
                                     PercentageToScreen.vec2(size_crosshair_horizontal).x,
                                     PercentageToScreen.vec2(size_crosshair_horizontal).y),
                                     vertical_crosshair);//left

            GUI.DrawTexture(new Rect(PercentageToScreen.vec2(right_pos_crosshair).x + PercentageToScreen.position_x(expandValues_crosshair.x) + Screen.width / 2,
                                     Screen.height / 2 + PercentageToScreen.vec2(right_pos_crosshair).y,
                                     PercentageToScreen.vec2(size_crosshair_horizontal).x,
                                     PercentageToScreen.vec2(size_crosshair_horizontal).y),
                                     vertical_crosshair);//right

            GUI.DrawTexture(new Rect(PercentageToScreen.vec2(top_pos_crosshair).x + Screen.width / 2,
                                     Screen.height / 2 + PercentageToScreen.vec2(top_pos_crosshair).y + PercentageToScreen.position_y(-expandValues_crosshair.y),
                                     PercentageToScreen.vec2(size_crosshair_vertical).x, PercentageToScreen.vec2(size_crosshair_vertical).y),
                                     horizontal_crosshair);//top

            GUI.DrawTexture(new Rect(PercentageToScreen.vec2(bottom_pos_crosshair).x + Screen.width / 2,
                                     Screen.height / 2 + PercentageToScreen.vec2(bottom_pos_crosshair).y + PercentageToScreen.position_y(expandValues_crosshair.y),
                                     PercentageToScreen.vec2(size_crosshair_vertical).x,
                                     PercentageToScreen.vec2(size_crosshair_vertical).y), horizontal_crosshair);//bottom
        }
    }


    /*
     * Used to expand position of the crosshair or make it dissapear when running
     */
    void CrossHairExpansionWhenWalking()
    {

        if (playerTransform.GetComponent<Rigidbody>().velocity.magnitude > 1 && Input.GetAxis("Fire1") == 0)
        {//ifnot shooting

            expandValues_crosshair += new Vector2(20, 40) * Time.deltaTime;
            if (PlayerController.Instance.MaxSpeed < runningSpeed)
            { //not running
                expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y, 0, 20));
                fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);
            }
            else
            {//running
                fadeout_value = Mathf.Lerp(fadeout_value, 0, Time.deltaTime * 10);
                expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 20), Mathf.Clamp(expandValues_crosshair.y, 0, 40));
            }
        }
        else
        {//if shooting
            expandValues_crosshair = Vector2.Lerp(expandValues_crosshair, Vector2.zero, Time.deltaTime * 5);
            expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y, 0, 20));
            fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);

        }
    }



    //private void Targeting()
    //{
    //    if ( firingPoint!=null )
    //    {

    //        bool isTargeted = Physics.Raycast(firingPoint.transform.position,
    //                                     firingPoint.transform.TransformDirection(Vector3.forward),
    //                                     out hit,
    //                                     Mathf.Infinity);
    //        if (isTargeted)
    //            Debug.DrawRay(firingPoint.transform.position,
    //                          firingPoint.transform.TransformDirection(Vector3.forward) * hit.distance,
    //                          Color.blue);
    //    }
    //}

    //private void SetFireRate()
    //{        
    //    if (curFireRate > 0)
    //        curFireRate -= Time.deltaTime;
    //}

    //private void TryShoot()
    //{        
    //    if (Input.GetButton("Fire1") && curFireRate <= 0)
    //        ReadyToShoot();
    //}

    //private void ReadyToShoot()
    //{        
    //    Shoot();
    //    curFireRate = fireRate;
    //}

    //private void Shoot()
    //{
    //    shootSound.Play();
    //    muzzle.Play();

    //    SparkOnTarget();
    //}

    //private void SparkOnTarget()
    //{
    //    if ( hit.collider!=null )
    //    {
    //        Debug.Log(hit.collider.name);

    //        var spark = Instantiate(sparkEffect, hit.point, Quaternion.LookRotation(hit.normal));
    //        Destroy(spark, 2f);
    //    }
    //}
}
