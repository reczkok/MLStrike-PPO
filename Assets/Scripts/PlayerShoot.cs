using System.Collections;
using System.Collections.Generic;
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

    private float timer;

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime / fireRate;


        if (isAuto)
        {
            if (Input.GetMouseButton(0) && timer <= 0)
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && timer <= 0)
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("BulletHolder").transform);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawnTransform.right * bulletSpeed, ForceMode.Impulse);
        bullet.GetComponent<Bullet>().SetShooter(GetComponent<CoinChaser>());
        //bullet.GetComponent<Bullet>().damage = bulletDamage;

        timer = 1;
    }
}
