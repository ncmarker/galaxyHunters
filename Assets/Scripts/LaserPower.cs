using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPower : Weapon
{
    private GameObject laserPrefab;
    private float shootInterval = 0.5f;
    private float laserSpeed = 10f;
    private List<Vector2> shootingDirections = new List<Vector2>();

    void Start()
    {
        type = WeaponType.LASER;
        isMaxLevel = false;
        maxLevel = 3;
        damage = 5;
        laserPrefab = Resources.Load<GameObject>("Prefabs/laser"); 
        Activate();
    }

    void Update()
    {
        if (currLevel == maxLevel)
        {
            isMaxLevel = true;
        }
    }

    public override void Activate()
    {
        isActive = true;
        UpdateShootingDirections();
        StartCoroutine(ShootBullets());
    }

    public override void Deactivate()
    {
        isActive = false;
        StopAllCoroutines();
    }

    public override void DoDamage() 
    {
    }

    public override void LevelUp()
    {
        if (currLevel < maxLevel)
        {
            currLevel++;
            UpdateShootingDirections();
            StopAllCoroutines();
            StartCoroutine(ShootBullets());
        }
    }

    private void UpdateShootingDirections()
    {
        shootingDirections.Clear();
        
        if (currLevel >= 1)
        {
            shootingDirections.Add(Vector2.left);
            shootingDirections.Add(Vector2.right);
        }
        if (currLevel >= 2)
        {
            shootingDirections.Add(Vector2.up);
            shootingDirections.Add(Vector2.down);
        }
        if (currLevel >= 3)
        {
            shootingDirections.Add(new Vector2(1, 1).normalized);
            shootingDirections.Add(new Vector2(1, -1).normalized);
            shootingDirections.Add(new Vector2(-1, 1).normalized);
            shootingDirections.Add(new Vector2(-1, -1).normalized);
        }
    }

    private IEnumerator ShootBullets()
    {
        while (isActive)
        {
            foreach (Vector2 direction in shootingDirections)
            {
                ShootLaser(direction);
            }
            SoundManager.Instance.PlaySound("laser_shoot"); // play once per interval

            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void ShootLaser(Vector2 direction)
    {
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);

        RotateLaserSprite(laser, direction);

        // pass damage to laser instance
        Laser laserScript = laser.GetComponent<Laser>();
        if (laserScript != null)
        {
            laserScript.SetDamage(damage);
        }

        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * laserSpeed;
        }

        Destroy(laser, 5f);
    }

    // helper function to rotate the laser sprite based on the direction
    private void RotateLaserSprite(GameObject laser, Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        laser.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public override bool GetIsMaxLevel()
    {
        return isMaxLevel;
    }
}

