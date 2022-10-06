using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Serializable]
    public struct GunStats
    {
        public int damage;
        public float range;
        public float knockback;
        public float firerate;
    }

    private float readyToFire = 0f;

    [SerializeField]
    private GunStats stats;

    [SerializeField]
    private Camera camera;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    [SerializeField]
    private GameObject hitEffect;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= readyToFire)
        {
            readyToFire = Time.time + 1f / stats.firerate;
            Shoot();
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hitInfo;

       if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hitInfo, stats.range))
        {
            Debug.Log(hitInfo.transform.name);

            Targetable target = hitInfo.transform.GetComponent<Targetable>();
            if(target != null)
            {
                target.getHit(stats.damage);
            }

            if(hitInfo.rigidbody != null)
            {
                hitInfo.rigidbody.AddForce(-hitInfo.normal * stats.knockback);
            }

            Instantiate(hitEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        }
    }
}
