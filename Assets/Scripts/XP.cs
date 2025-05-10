using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Xml.XPath;
using Unity.VisualScripting;

public enum XPType { 
    Blue, 
    Purple, 
    Orange 
}


public class XP : MonoBehaviour
{
    public Sprite blueXP;
    public Sprite purpleXP;
    public Sprite orangeXP;

    private XPType xpType;
    private int xpAmount;
    private SpriteRenderer sr;
    public GameObject damageNumberPrefab;

    private Dictionary<XPType, int> values = new Dictionary<XPType, int>
    {
        { XPType.Blue, 10 },
        { XPType.Purple, 20 },
        { XPType.Orange, 50 }
    };

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Initialize(XPType type)
    {
        xpType = type;
        xpAmount = values[type];

        switch (xpType)
        {
            case XPType.Blue:
                sr.sprite = blueXP;
                break;
            case XPType.Purple:
                sr.sprite = purpleXP;
                break;
            case XPType.Orange:
                sr.sprite = orangeXP;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!GameManager.Instance.GetIsPaused())
            {
                SoundManager.Instance.PlaySound("xp_pickup");
            }
            
            // add XP to combined xp amount
            bool playerHasBonus = other.GetComponent<PlayerController>().GetCharacterType() == CharacterType.XP; 
            XPManager.Instance.AddXP(playerHasBonus ? xpAmount * 2 : xpAmount);

            // "+XP" damage number 
            GameObject damageText = Instantiate(damageNumberPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            damageText.GetComponent<DamageNumber>().Initialize("+" + xpAmount, Color.yellow);

            Destroy(gameObject); 
        }
    }

    
}
