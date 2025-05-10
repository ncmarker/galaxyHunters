using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    int max_health = 100;
    const float INVULNERABLE_PHASE = 1.0f;
    public int currHealth;
    public float invulnerablePeriod = 0.0f;
    private bool isAlive = true;

    public Slider healthSlider;
    public GameObject playerSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        currHealth = max_health;
        healthSlider.maxValue = currHealth;
        healthSlider.value = currHealth;
    }

    void Update()
    {
        if (currHealth <= 0)
        {
            KillPlayer();
        }
        healthSlider.value = currHealth;
    }

    void FixedUpdate()
    {
        invulnerablePeriod += Time.deltaTime;
    }

    void OnTriggerStay2D(Collider2D other){
        if (other.CompareTag("Enemy"))
        {
            if (invulnerablePeriod > INVULNERABLE_PHASE)
            {
                SoundManager.Instance.PlaySound("hurt");
                StartCoroutine(FlashRed());
                currHealth -= other.gameObject.GetComponent<Enemy>().GetDamage();
                EnemyTypeID enemyType = other.gameObject.GetComponent<Enemy>().GetEnemyTypeID();
                if (enemyType != EnemyTypeID.MINIBOSS_ENEMY && enemyType != EnemyTypeID.FINAL_BOSS_ENEMY) 
                {
                    other.gameObject.GetComponent<Enemy>().Die(); // kill enemy on collision with player
                }

                healthSlider.value = currHealth;
                invulnerablePeriod = 0.0f;
                if (currHealth <= 0){
                    KillPlayer();
                }
            }
        }
    }
    
    // not actually used
    public void TakeDamage(int amount) {
        StartCoroutine(FlashRed());
        currHealth -= amount;
        healthSlider.value = currHealth;
    }

    void KillPlayer() 
    {
        this.gameObject.SetActive(false);
        isAlive = false;
        gameObject.GetComponent<PlayerController>().DeactivateAllWeapons();
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    public void SetIsAlive(bool alive)
    {
        isAlive = alive;
    }

    public void MaxOutHealth()
    {
        currHealth = max_health;
    }

    public void SetMaxHealth(int newMax)
    {
        max_health = newMax;
        currHealth = max_health;
        healthSlider.maxValue = currHealth;
        healthSlider.value = currHealth;
    }

    public int GetMaxHealth()
    {
        return max_health;
    }

    private IEnumerator FlashRed()
    {
        SpriteRenderer spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            spriteRenderer.color = new Color(0.984f, 0.588f, 0.588f); // #FB9696
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
}
