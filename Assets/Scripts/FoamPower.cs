using System.Collections;
using UnityEngine;

public class FoamPower : Weapon
{
    private const float SCALE_FACTOR = 3.67f * 1.5f; // 3.67 is per 1 radius unit
    private float radius = 1.5f;
    private float damageInterval = 0.65f;
    private GameObject foamPrefab;
    private PlayerHealthController player;
    private GameObject activeFoamEffect;
    
    private void Start()
    {
        type = WeaponType.FOAM;
        isMaxLevel = false;
        maxLevel = 3;
        foamPrefab = Resources.Load<GameObject>("Prefabs/foam"); 
        player = gameObject.GetComponent<PlayerHealthController>(); 
        Activate();
    }

    private void Update()
    {
        // check if max level 
        if (currLevel == maxLevel)
        {
            isMaxLevel = true;
        }
    }

    public override void Activate()
    {
        if (isActive) return;

        // gameObject.GetComponent<PlayerController>().AddWeapon(this);
        isActive = true;
        damage = 1;

        if (foamPrefab != null && activeFoamEffect == null)
        {
            activeFoamEffect = Instantiate(foamPrefab, player.transform.position + new Vector3(-0.05f, 0, 0), Quaternion.identity);
            activeFoamEffect.transform.SetParent(player.transform); // attach to player
            // scale sprite to match radius 
            float scaleFactor = radius * SCALE_FACTOR; 
            activeFoamEffect.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }

        StartCoroutine(ApplyRadiusEffect());
    }

    public override void Deactivate()
    {
        isActive = false;
        StopAllCoroutines();

        if (activeFoamEffect != null)
        {
            Destroy(activeFoamEffect);
            activeFoamEffect = null;
        }
    }

    public override void DoDamage()
    {
    }

    public override void LevelUp()
    {
        currLevel++;
        damage += 1;
        radius++;
        float scaleFactor = radius * SCALE_FACTOR; 
        activeFoamEffect.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    private IEnumerator ApplyRadiusEffect()
    {
        while (isActive)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out Enemy enemy))
                {
                    enemy.TakeDamage(damage);
                }
            }
            SoundManager.Instance.PlaySound("foam_sound");
            yield return new WaitForSeconds(damageInterval);
        }
    }

    public override bool GetIsMaxLevel()
    {
        return isMaxLevel;
    }
}
