using System.Collections;
using UnityEngine;

public class SwordPower : Weapon
{
    private GameObject swordPrefab; 
    private PlayerController playerController; 
    private float slashCooldown = 1.5f;
    private float slashOffset = 1f;

    private Coroutine slashRoutine;

    void Start()
    {
        damage = 3;
        maxLevel = 3;
        isMaxLevel = false;
        swordPrefab = Resources.Load<GameObject>("Prefabs/sword"); 

        Animator animator = swordPrefab.GetComponent<Animator>();
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        if (controller != null && controller.animationClips.Length > 0)
        {
            duration = controller.animationClips[0].length;
        }
        else 
        {
            duration = 1f;
        }

        playerController = gameObject.GetComponent<PlayerController>();
        Activate();
    }

    public override void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            slashRoutine = StartCoroutine(SlashLoop());
        }
    }

    public override void Deactivate()
    {
        if (slashRoutine != null)
        {
            StopCoroutine(slashRoutine);
        }
        isActive = false;
    }

    private IEnumerator SlashLoop()
    {
        while (isActive)
        {
            DoDamage();
            yield return new WaitForSeconds(slashCooldown);
        }
    }

    public override void DoDamage()
    {
        Vector3 forwardDir = playerController.GetRecentForward();

        // sword in the facing direction
        SpawnSlash(forwardDir);

        if (currLevel >= 2)
        {
            // sword behind as well for level 2
            SpawnSlash(-forwardDir);
        }
    }

    private void SpawnSlash(Vector3 direction)
    {
        Vector3 spawnPos = transform.position + direction.normalized * slashOffset;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0,0,angle);
        spawnPos.z = -1.0f;

        GameObject slash = Instantiate(swordPrefab, spawnPos, rotation);
        
        if (currLevel >= 3)
        {
            slash.transform.localScale = new Vector3(3f, 3f, 3f); // 2 * 1.5 = 3
        }
        else
        {
            slash.transform.localScale = new Vector3(2f, 2f, 2f);
        }

        Vector3 pos = slash.transform.position;
        pos.z = -1;
        slash.transform.position = pos;

        // attach to player to follow
        FollowPlayer follow = slash.GetComponent<FollowPlayer>();
        if (follow != null)
        {
            follow.Initialize(transform, direction.normalized * slashOffset);
        }

        Sword sword = slash.GetComponent<Sword>();
        if (sword != null)
        {
            sword.SetDamage(damage);
        }

        SoundManager.Instance.PlaySound("swordSlash");

        Destroy(slash, duration);
    }

    public override void LevelUp()
    {
        if (currLevel < maxLevel)
        {
            currLevel++;
            if (currLevel == maxLevel) isMaxLevel = true;
        }
    }

    public override bool GetIsMaxLevel()
    {
        return isMaxLevel;
    }
}
