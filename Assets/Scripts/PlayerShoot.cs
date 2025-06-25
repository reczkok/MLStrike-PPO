using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Bullet Variables")]
    public float bulletSpeed;
    public float fireRate, bulletDamage;
    public bool isAuto;

    [Header("Initial Setup")]
    public Transform bulletSpawnTransform;
    public GameObject bulletPrefab;

    public void Shoot(GameObject owner, float rotation)
    {
        var rotationQuaternion = Quaternion.Euler(rotation, 0, 0);
        var bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, bulletSpawnTransform.transform.rotation * (rotation != 0 ? rotationQuaternion : Quaternion.identity));
        // bullet.transform.localRotation = rotationQuaternion;
        
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletSpeed, ForceMode.Impulse);
        bullet.GetComponent<Bullet>().SetShooter(owner);
    }
}
