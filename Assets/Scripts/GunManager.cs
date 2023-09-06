using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField]
    private List<string> weapons = new ();

    private List<Texture> icons = new();
        
    private GameObject currentGun;
    private Animator currentHandsAnimator;

    private int currentGunCounter = 0;
    private float switchWeaponCooldown;
    [SerializeField] float cooldownTime = 1.2f;

    [Tooltip("Spacing between icons.")]
    [SerializeField] private int spacing = 10;
    [Tooltip("Begin position in percetanges of screen.")]
    [SerializeField] private Vector2 beginPosition;
    [Tooltip("Size of icon in percetanges of screen.")]
    [SerializeField] private Vector2 size;

        
    [Header("Sounds")]
    [Tooltip("Sound of weapon changing.")]
    [SerializeField] private AudioSource weaponChanging;

    private void Awake()
    {
        StartCoroutine("UpdateIconsFromResources");
        StartCoroutine("SpawnWeaponUponStart");//to start with a gun
    }

    private void Update()
    {
        switchWeaponCooldown += Time.deltaTime;
        if (switchWeaponCooldown > cooldownTime && Input.GetKey(KeyCode.LeftShift) == false)
        {
            Create_Weapon();
        }
    }

    private IEnumerator SpawnWeaponUponStart()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("Spawn", 0);
    }

    void Create_Weapon()
    {
        /*
		 * Scrolling wheel waepons changing
		 */
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            switchWeaponCooldown = 0;

            currentGunCounter++;
            if (currentGunCounter > weapons.Count - 1)
            {
                currentGunCounter = 0;
            }
            StartCoroutine("Spawn", currentGunCounter);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            switchWeaponCooldown = 0;

            currentGunCounter--;
            if (currentGunCounter < 0)
            {
                currentGunCounter = weapons.Count - 1;
            }
            StartCoroutine("Spawn", currentGunCounter);
        }

        /*
		 * Keypad numbers
		 */
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentGunCounter != 0)
        {
            switchWeaponCooldown = 0;
            currentGunCounter = 0;
            StartCoroutine("Spawn", currentGunCounter);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && currentGunCounter != 1)
        {
            switchWeaponCooldown = 0;
            currentGunCounter = 1;
            StartCoroutine("Spawn", currentGunCounter);
        }

    }


    IEnumerator Spawn(int gunIndex)
    {
        if (weaponChanging)
            weaponChanging.Play();
        else
            print("Missing Weapon Changing music clip.");

        if (currentGun)
        {
            if (currentGun.name.Contains("Gun"))
            {
                currentHandsAnimator.SetBool("changingWeapon", true);

                yield return new WaitForSeconds(0.8f);//0.8 time to change waepon, but since there is no change weapon animation there is no need to wait fo weapon taken down
                Destroy(currentGun);

                GameObject resource = (GameObject)Resources.Load(weapons[gunIndex].ToString());
                currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);
            }
            else if (currentGun.name.Contains("Sword"))
            {
                currentHandsAnimator.SetBool("changingWeapon", true);
                yield return new WaitForSeconds(0.25f);//0.5f

                currentHandsAnimator.SetBool("changingWeapon", false);

                yield return new WaitForSeconds(0.6f);//1
                Destroy(currentGun);

                GameObject resource = (GameObject)Resources.Load(weapons[gunIndex].ToString());
                currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);
            }
        }
        else
        {
            GameObject resource = (GameObject)Resources.Load(weapons[gunIndex].ToString());
            currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);            
        }

        AssignHandsAnimator();
    }

    /*
    * Assigns Animator to the script so we can use it in other scripts of a current gun.
    */
    void AssignHandsAnimator()
    {
        if (currentGun.name.Contains("Gun"))
        {
            currentHandsAnimator = currentGun.GetComponent<GunController>().HandsAnimator;
        }
    }



    private IEnumerator UpdateIconsFromResources()
    {
        yield return new WaitForEndOfFrame();
                
        for (int i = 0; i < weapons.Count; i++)
        {
            icons.Add((Texture)Resources.Load("Weapon_Icons/" + weapons[i].ToString() + "_img"));
        }
    }

    private void OnGUI()
    {
        //if (curWeaponIndex)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                DrawCorrespondingImage(i);
            }
        }
    }

    private void DrawCorrespondingImage(int i)
    {
        if ( icons.Count == weapons.Count )
        {
            GUI.DrawTexture(new Rect(PercentageToScreen(beginPosition).x,
                                 PercentageToScreen(beginPosition).y, //position variables
                                 PercentageToScreen(size).x,
                                 PercentageToScreen(size).y),
                                 icons[0]);

            GUI.DrawTexture(new Rect(PercentageToScreen(beginPosition).x + (1 * Position_x(spacing) + 10),
                                     PercentageToScreen(beginPosition).y + 10,
                                     PercentageToScreen(size).x - 20,
                                     PercentageToScreen(size).y - 20),
                                     icons[1]);
        }
        


        // string deleteCloneFromName = currentGun.name.Substring(0, currentGun.name.Length - 7);

        //if (deleteCloneFromName == gunsIHave[_number])
        //{
        //    GUI.DrawTexture(new Rect( vec2(beginPosition).x + (_number * position_x(spacing)),
        //                              vec2(beginPosition).y,//position variables
        //                              vec2(size).x, vec2(size).y),//size
        //                              icons[_number] );
        //}
        //else
        //{
        //    GUI.DrawTexture(new Rect(vec2(beginPosition).x + (_number * position_x(spacing) + 10), vec2(beginPosition).y + 10,//position variables
        //    vec2(size).x - 20, vec2(size).y - 20),//size
        //        icons[_number]);
        //}
                
    }

    //#####		RETURN THE SIZE AND POSITION for GUI images
    //(we pass in the percentage and it returns some number to appear in that percentage on the sceen) ##################
    private float Position_x(float var)
    {
        return Screen.width * var / 100;
    }
    private float Position_y(float var)
    {
        return Screen.height * var / 100;
    }

    private float PercentageToWidth(float var)
    {
        return Screen.width * var / 100;
    }

    private float PercentageToHeight(float var)
    {
        return Screen.height * var / 100;
    }

    private Vector2 PercentageToScreen(Vector2 _vec2)
    {
        return new Vector2(Screen.width * _vec2.x / 100, Screen.height * _vec2.y / 100);
    }
}
