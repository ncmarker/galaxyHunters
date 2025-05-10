using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RollingBallPower : Weapon
{
    private GameObject ballPrefab;
    private GameObject activeRollingBall;

    void Start()
    {
        type = WeaponType.ROLLINGBALL;
        isMaxLevel = false;
        maxLevel = 3;
        damage = 20;
        ballPrefab = Resources.Load<GameObject>("Prefabs/rollingBall"); 

        Activate();
    }

    public override void Activate()
    {
        if (isActive) return;

        isActive = true;

        if (ballPrefab != null && !activeRollingBall)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * 0.5f;
            Vector2 spawnPosition = (Vector2)transform.position + randomOffset;

            activeRollingBall = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
            activeRollingBall.GetComponent<RollingBall>().SetDirection(randomOffset.normalized);
            activeRollingBall.GetComponent<RollingBall>().SetDamage(damage);
            float randomZRotation = Random.Range(0f, 360f);
            activeRollingBall.transform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);
        }
    }

    public override void Deactivate()
    {
        isActive = false;
        Destroy(activeRollingBall);
    }

    public override void LevelUp()
    {
        if (currLevel < maxLevel)
        {
            currLevel++;
            Vector3 currScale = activeRollingBall.transform.localScale;
            activeRollingBall.transform.localScale = currScale * 1.5f;
        }
    }

    public override bool GetIsMaxLevel()
    {
        return isMaxLevel;
    }

    public override void DoDamage()
    {
    }

    void Update()
    {
        // check max level 
        if (currLevel == maxLevel)
        {
            isMaxLevel = true;
        }
    }
}
