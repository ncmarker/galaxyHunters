using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance;
    private int currentXP = 0;
    private int nextXPLevelUp = 400;
    public Text textXP;
    private bool readyForUpgrade;

    public Slider xpSlider;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // preserves between scenes (if needed)
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        xpSlider.maxValue = nextXPLevelUp;
        xpSlider.value = currentXP;
        readyForUpgrade = false;
    }

    void Update()
    {
        xpSlider.value = currentXP;
        textXP.text = "XP: " + currentXP.ToString() + "/" + nextXPLevelUp.ToString();

        // check for upgrades
        if (currentXP >= nextXPLevelUp)
        {
            readyForUpgrade = true;
            GameManager.Instance.PauseGame();
            nextXPLevelUp *= 2;
            currentXP = 0;
            xpSlider.maxValue = nextXPLevelUp;
            readyForUpgrade = false;
        }
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
    }

    public bool GetReadyForUpgrade() 
    {
        return readyForUpgrade;
    }

    public void MaxOutXP()
    {
        currentXP = nextXPLevelUp;
    }
}
