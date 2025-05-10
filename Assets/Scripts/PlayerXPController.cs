using UnityEngine;
using UnityEngine.UI;


// *********************************************************************
// NO LONGER BEING USED
// *********************************************************************




public class PlayerXPController : MonoBehaviour
{
    public int currentXP = 0;
    private int nextXPLevelUp = 2000;
    public Text playerXP;
    private bool readyForUpgrade;

    void Start()
    {
        playerXP.text = "XP: " + currentXP.ToString() + " / " + nextXPLevelUp.ToString();
        readyForUpgrade = false;
    }

    void Update()
    {
        if (currentXP >= nextXPLevelUp)
        {
            Debug.Log("Player Level Up");
            readyForUpgrade = true;
            GameManager.Instance.PauseGame();
            nextXPLevelUp *= 2;
            readyForUpgrade = false;
        }
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        playerXP.text = "XP: " + currentXP.ToString() + " / " + nextXPLevelUp.ToString();
    }

    public bool GetReadyForUpgrade() 
    {
        return readyForUpgrade;
    }

    public void MaxOutXP()
    {
        currentXP = nextXPLevelUp;
    }

    public int GetCurrentXP()
    {
        return currentXP;
    }

}
