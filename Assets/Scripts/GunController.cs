using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private GameObject firingPoint;

    [SerializeField]
    private float fireRate = 0.5f;

    [SerializeField]
    private ParticleSystem muzzle;

    [SerializeField]
    private AudioSource shootSound;

    [SerializeField]
    private GameObject sparkEffect;


    private float curFireRate;
    private RaycastHit hit;

    // Start is called before the first frame update
    private void Awake()
    {
        curFireRate = fireRate;
    }

    // Update is called once per frame
    private void Update()
    {
        Targeting();
        SetFireRate();
        TryShoot();
        
    }

    private void Targeting()
    {
        if ( firingPoint!=null )
        {
            
            bool isTargeted = Physics.Raycast(firingPoint.transform.position,
                                         firingPoint.transform.TransformDirection(Vector3.forward),
                                         out hit,
                                         Mathf.Infinity);
            if (isTargeted)
                Debug.DrawRay(firingPoint.transform.position,
                              firingPoint.transform.TransformDirection(Vector3.forward) * hit.distance,
                              Color.blue);
        }
    }

    private void SetFireRate()
    {        
        if (curFireRate > 0)
            curFireRate -= Time.deltaTime;
    }

    private void TryShoot()
    {        
        if (Input.GetButton("Fire1") && curFireRate <= 0)
            ReadyToShoot();
    }

    private void ReadyToShoot()
    {        
        Shoot();
        curFireRate = fireRate;
    }

    private void Shoot()
    {
        shootSound.Play();
        muzzle.Play();

        SparkOnTarget();
    }

    private void SparkOnTarget()
    {
        if ( hit.collider!=null )
        {
            Debug.Log(hit.collider.name);

            var spark = Instantiate(sparkEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(spark, 2f);
        }
    }
}
