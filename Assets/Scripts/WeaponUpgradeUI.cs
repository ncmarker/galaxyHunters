using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class WeaponUpgradeUI : MonoBehaviour
{
    // UI Elements
    public GameObject player1WeaponPanel;
    public GameObject player2WeaponPanel;

    // player 1 specific 
    private PlayerController player1; 
    public Button[] player1Choices;
    public Text[] player1Descriptions;
    private bool p1Decided;
    private int currP1Choice;

    // player 2 specific
    private PlayerController player2; 
    public Button[] player2Choices;
    public Text[] player2Descriptions;
    private bool p2Decided;
    private int currP2Choice;

    // called each time the WeaponUpgradeUI is set to active
    void OnEnable()
    {
        GameObject p1 = GameObject.Find("Player1");
        player1 = p1 ? p1.GetComponent<PlayerController>() : null;

        GameObject p2 = GameObject.Find("Player2");
        player2 = p2 ? p2.GetComponent<PlayerController>() : null;
        
        // Initially hide both panels
        player1WeaponPanel.SetActive(false);
        player2WeaponPanel.SetActive(false);
        // p1Decided = false;
        // p2Decided = false;
        if (!player1) p1Decided = true;
        else p1Decided = player1.AreAllWeaponsMaxed();
        
        if (!player2) p2Decided = true;
        else p2Decided = player2.AreAllWeaponsMaxed();

        // both choices start at first choice 
        currP1Choice = 0;
        currP2Choice = 0;

        // logic on what panel to show 
        ShowUpgradeOptions();

        // start with first weapon highlighted
        currP1Choice = 0;
        HighlightChoice(currP1Choice, player1Choices);
        currP2Choice = 0;
        HighlightChoice(currP2Choice, player2Choices);
    }

    // waits for both players to decide before unpausing game 
    void Update()
    {
        if (p1Decided && p2Decided)
        {
            GameManager.Instance.UnpauseGame();
            if (player1) WeaponManager.Instance.UpdatePlayerWeaponIcons(player1);
            if (player2) WeaponManager.Instance.UpdatePlayerWeaponIcons(player2);
        }
        else
        {
            HandleIput();  
        }
        
    } 

    // shows each player's panel if they are ready for an upgrade
    public void ShowUpgradeOptions()
    {
        // FOR INDIVIDUAL XP BARS

        // if (player1 && player1.gameObject.GetComponent<PlayerXPController>().GetReadyForUpgrade())
        // {
        //     player1WeaponPanel.SetActive(true); 
        //     SetupPlayer1Panel();
        // }
        // else p1Decided = true;

        // if (player2 && player2.gameObject.GetComponent<PlayerXPController>().GetReadyForUpgrade())
        // {
        //     player2WeaponPanel.SetActive(true); 
        //     SetupPlayer2Panel();
        // }
        // else p2Decided = true;

        if (XPManager.Instance.GetReadyForUpgrade())
        {
            if (player1)
            {
                player1WeaponPanel.SetActive(true); 
                SetupPlayer1Panel();
            }
            else p1Decided = true;

            if (player2)
            {
                player2WeaponPanel.SetActive(true); 
                SetupPlayer2Panel();
            }
            else p2Decided = true;
        }
    }

    // adds current weapons / new weapon to player 1 upgrade panel 
    private void SetupPlayer1Panel()
    {
        if (player1 && player1.GetComponent<PlayerHealthController>().GetIsAlive()) 
        {
            HashSet<WeaponType> p1Weapons = player1.GetPlayerWeapons();

            int currChoice = 0;
            foreach (WeaponType wt in p1Weapons) 
            {
                // if already max level, deactivate button
                if (player1.IsWeaponMaxLevel(wt))
                {
                    player1Choices[currChoice].interactable = false;
                    player1Descriptions[currChoice].text = wt.ToString() + " is at max level";
                }
                // otherwise offer an upgrade
                else 
                {
                    player1Choices[currChoice].onClick.RemoveAllListeners();

                    player1Choices[currChoice].interactable = true;
                    player1Choices[currChoice].onClick.AddListener(() => {
                        player1.UpgradeWeapon(wt);
                        player1WeaponPanel.SetActive(false);
                        p1Decided = true;
                        });
                    player1Descriptions[currChoice].text = "Level Up " + wt.ToString();
                }

                player1Choices[currChoice].image.sprite = Resources.Load<Sprite>($"Sprites/{wt}");
                currChoice++;
            }

            HashSet<WeaponType> constraints = new HashSet<WeaponType>();
            while (currChoice < 3)
            {
                // shouldn't return none, but could
                WeaponType newWeaponType = WeaponManager.Instance.GetRandomWeapon(player1, constraints);
                constraints.Add(newWeaponType);

                player1Choices[currChoice].onClick.RemoveAllListeners();
                player1Choices[currChoice].onClick.AddListener(() => {
                    player1.AddWeapon(newWeaponType);
                    player1WeaponPanel.SetActive(false);
                    p1Decided = true;
                    });
                player1Descriptions[currChoice].text = "Add " + newWeaponType.ToString();

                player1Choices[currChoice].image.sprite = Resources.Load<Sprite>($"Sprites/{newWeaponType}");
                currChoice++;
            }
        }
    }

    // adds current weapons / new weapon to player 2 upgrade panel 
    private void SetupPlayer2Panel()
    {
        if (player2 && player2.GetComponent<PlayerHealthController>().GetIsAlive()) 
        {
            HashSet<WeaponType> p2Weapons = player2.GetPlayerWeapons();

            int currChoice = 0;
            foreach (WeaponType wt in p2Weapons) 
            {
                // if already max level, deactivate button
                if (player2.IsWeaponMaxLevel(wt))
                {
                    player2Choices[currChoice].interactable = false;
                    player2Descriptions[currChoice].text = wt.ToString() + " is at max level";
                }
                // otherwise offer an upgrade
                else 
                {
                    player2Choices[currChoice].onClick.RemoveAllListeners();

                    player2Choices[currChoice].interactable = true;
                    player2Choices[currChoice].onClick.AddListener(() => {
                        player2.UpgradeWeapon(wt);
                        player2WeaponPanel.SetActive(false);
                        p2Decided = true;
                        });
                    player2Descriptions[currChoice].text = "Level Up " + wt.ToString();
                }

                player2Choices[currChoice].image.sprite = Resources.Load<Sprite>($"Sprites/{wt}");
                currChoice++;
            }

            HashSet<WeaponType> constraints = new HashSet<WeaponType>();
            while (currChoice < 3)
            {
                // shouldn't return none, but could
                WeaponType newWeaponType = WeaponManager.Instance.GetRandomWeapon(player2, constraints);
                constraints.Add(newWeaponType);

                player2Choices[currChoice].onClick.RemoveAllListeners();
                player2Choices[currChoice].onClick.AddListener(() => {
                    player2.AddWeapon(newWeaponType);
                    player2WeaponPanel.SetActive(false);
                    p2Decided = true;
                    });
                player2Descriptions[currChoice].text = "Add " + newWeaponType.ToString();
                
                player2Choices[currChoice].image.sprite = Resources.Load<Sprite>($"Sprites/{newWeaponType}");
                currChoice++;
            }
        }
    }

    private void HandleIput()
    {
        if (!p1Decided)
        {
            // player 1 choices with WASD, left shift to select
            if (Input.GetKeyDown(KeyCode.A))
            {
                currP1Choice = Mathf.Max(0, currP1Choice - 1);
                HighlightChoice(currP1Choice, player1Choices);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                currP1Choice = Mathf.Min(player1Choices.Length - 1, currP1Choice + 1);
                HighlightChoice(currP1Choice, player1Choices);
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (player1Choices[currP1Choice].interactable)
                {
                    player1Choices[currP1Choice].onClick.Invoke();
                }
            }
        }  
        if (!p2Decided)
        {
            // player 2 choices with arrow keys, right shift to select
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {   
                currP2Choice = Mathf.Max(0, currP2Choice - 1);
                HighlightChoice(currP2Choice, player2Choices);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currP2Choice = Mathf.Min(player2Choices.Length - 1, currP2Choice + 1);
                HighlightChoice(currP2Choice, player2Choices);
            }
            else if (Input.GetKeyDown(KeyCode.RightShift))
            {
                if (player2Choices[currP2Choice].interactable)
                {
                    player2Choices[currP2Choice].onClick.Invoke();
                }
            }
        } 
    }

    // changes sprite icons to outline the currently selected choice
    private void HighlightChoice(int choiceIndex, Button[] playerChoices)
    {
        for (int i = 0; i < playerChoices.Length; i++)
        {
            Image buttonImage = playerChoices[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                string originalName = buttonImage.sprite.name;
                
                // swap current image to regular image or outlined one depending on current choice
                string baseName = originalName.Replace("_outline", "");
                string newSpriteName = (i == choiceIndex) ? baseName + "_outline" : baseName;
                Sprite newSprite = Resources.Load<Sprite>("Sprites/" + newSpriteName);
                
                if (newSprite != null)
                {
                    buttonImage.sprite = newSprite;
                }
                else
                {
                    Debug.LogWarning("Sprite not found: " + newSpriteName);
                }
            }
        }
    }
}
