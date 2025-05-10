using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class SignalPower : Weapon
{
    private float damageInterval = 1.0f; 
    private GameObject signalPrefab;
    private PlayerHealthController player;
    private GameObject activeSignal;
    private float radius = 1.5f;

    // for cooldown bar
    private Slider cooldownBar;
    private float cooldownTime = 20f; 
    private float cooldownTimer = 20f;
    private bool onCooldown = false;
    private float signalLifetime = 30f;
    
    private void Start()
    {
        type = WeaponType.SIGNAL;
        isMaxLevel = false;
        maxLevel = 2;
        signalPrefab = Resources.Load<GameObject>("Prefabs/signal1"); 
        player = gameObject.GetComponent<PlayerHealthController>(); 

        cooldownBar = player.gameObject.name == "Player1" ? GameManager.Instance.GetPlayer1SignalBar() : GameManager.Instance.GetPlayer2SignalBar(); 
        cooldownBar.gameObject.SetActive(true);
        cooldownBar.maxValue = cooldownTime;
        cooldownBar.value = cooldownTimer;
    }

    private void Update()
    {
        // check if max level 
        if (currLevel == maxLevel)
        {
            isMaxLevel = true;
        }

        HandleCooldown();
        HandleInput();
    }

    private void HandleInput()
    {
        if (player.gameObject.name == "Player1" && Input.GetKeyDown(KeyCode.LeftShift))
        {
            DropSignal();
        }
        else if (player.gameObject.name == "Player2" && Input.GetKeyDown(KeyCode.RightShift))
        {
            DropSignal();
        }
    }

    private void DropSignal()
    {
        if (onCooldown || isActive) return;

        Activate();
        StartCooldown();
    }

    public override void Activate()
    {
        if (isActive || onCooldown) return;

        isActive = true;
        damage = 3;

        if (signalPrefab != null && activeSignal == null)
        {
            activeSignal = Instantiate(signalPrefab, player.transform.position, Quaternion.identity);
 
            // UpdateDamageInterval(activeSignal);

            StartCoroutine(DestroySignalAfterTime());
        }

        StartCoroutine(ApplySignalEffect());
    }

    private IEnumerator DestroySignalAfterTime()
    {
        yield return new WaitForSeconds(signalLifetime);
        Deactivate();
    }

    public override void Deactivate()
    {
        isActive = false;
        StopAllCoroutines();

        if (activeSignal != null)
        {
            Destroy(activeSignal);
            activeSignal = null;
        }
    }

    public override void DoDamage()
    {
    }

    public override void LevelUp()
    {
        if (currLevel < maxLevel)
        {
            currLevel++;
            if (currLevel == 2)
            {
                signalPrefab = Resources.Load<GameObject>("Prefabs/signal2"); // Load new prefab
                radius = 2f; 

                // swap old prefab with new one
                if (activeSignal != null)
                {
                    Destroy(activeSignal);
                }
                activeSignal = Instantiate(signalPrefab, player.transform.position, Quaternion.identity);
                // UpdateDamageInterval(activeSignal);
            }
        }
    }

    private IEnumerator ApplySignalEffect()
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
            // SoundManager.Instance.PlayBackgroundMusic("foam_sound");
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void StartCooldown()
    {
        onCooldown = true;
        cooldownTimer = 0f;
        cooldownBar.value = 0f; // Reset UI bar
    }

    private void HandleCooldown()
    {
        if (!onCooldown) return;

        cooldownTimer += Time.deltaTime;
        cooldownBar.value = cooldownTimer;

        // cooldown finished
        if (cooldownTimer >= cooldownTime)
        {
            onCooldown = false;
            cooldownBar.value = 1f;
        }
    }

    public override bool GetIsMaxLevel()
    {
        return isMaxLevel;
    }

    // updates the damage interval to be the exact animation length
    // private void UpdateDamageInterval(GameObject activeSignal)
    // {
    //     Animator animator = activeSignal.GetComponent<Animator>();
    //     if (animator != null && animator.runtimeAnimatorController != null)
    //     {
    //         AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
    //         if (clips.Length > 0)
    //         {
    //             damageInterval = clips[0].length;
    //         }
    //     }
    // }
}
