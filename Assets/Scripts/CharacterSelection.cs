using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CharacterType
{
    HEALER,
    SPEED,
    XP, 
    NONE
}

public class CharacterSelection : MonoBehaviour
{
    // UI elements
    public Button startGameButton;
    public Image player1Icon, player2Icon;
    public Image[] player1Options, player2Options;
    public Text player1Description, player2Description;

    // Currently selected character index for each player
    private int player1Selection = 0;
    private int player2Selection = 0;

    // Track whether the players have confirmed their selection
    private bool player1Confirmed = false;
    private bool player2Confirmed = false;

    private const int ACTIVE_OPTIONS = 2;
    private Sprite comingSoonSprite;

    // List of character sprites
    public Sprite[] characterSprites; 

    // update all below lists every time an update is made for a character
    private String[] characterDescriptions = {
        "Runs at 1.25x speed\nStarting Weapon: laser",
        "1.5x max HP\nStarting Weapon: foam",
        "1.5x max HP\nStarting Weapon: sword",
        "2x pickup XP value\nStarting Weapon: bombs"
        };
    private WeaponType[] characterStartingWeapons = {WeaponType.LASER, WeaponType.FOAM, WeaponType.SWORD, WeaponType.BOMB};
    // update player controller AddBonus() when altering bonus names
    private CharacterType[] characterBonuses = {CharacterType.SPEED, CharacterType.HEALER, CharacterType.HEALER, CharacterType.XP};

    void Start()
    {
        startGameButton.interactable = false;

        // player1Icon.sprite = characterSprites[player1Selection];
        // player2Icon.sprite = characterSprites[player2Selection];

        player1Icon.sprite = Resources.Load<Sprite>("Sprites/" + characterSprites[player1Selection].name + "Icon");
        player1Description.text = characterDescriptions[player1Selection];
        player2Icon.sprite = Resources.Load<Sprite>("Sprites/" + characterSprites[player2Selection].name + "Icon");
        player2Description.text = characterDescriptions[player2Selection];

        comingSoonSprite = Resources.Load<Sprite>("Sprites/coming_soon");

        // fill player choices with sprites
        for (int i = 0; i < player1Options.Length; i++)
        {
            if (i < ACTIVE_OPTIONS)
            {
                // player1Options[i].sprite = characterSprites[i];
                // player2Options[i].sprite = characterSprites[i];
                player1Options[i].sprite = Resources.Load<Sprite>("Sprites/" + characterSprites[i].name + "Icon");
                player2Options[i].sprite = Resources.Load<Sprite>("Sprites/" + characterSprites[i].name + "Icon");
            }
            else 
            {
                player1Options[i].sprite = comingSoonSprite;
                player2Options[i].sprite = comingSoonSprite;
            }
        }
        SoundManager.Instance.PlayBackgroundMusic("pregameBackground");
    }

    void Update()
    {
        HandlePlayer1Input();
        HandlePlayer2Input();

        // enable the start button if both players have confirmed
        if (player1Confirmed && player2Confirmed)
        {
            startGameButton.interactable = true;
            startGameButton.onClick.AddListener(StartGame);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SoundManager.Instance.PlaySound("toggle");
                StartGame();
            }
        }
    }

    void HandlePlayer1Input()
    {
        if (!player1Confirmed)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                SoundManager.Instance.PlaySound("toggle");
                player1Selection = (player1Selection - 1 + ACTIVE_OPTIONS) % ACTIVE_OPTIONS;
                UpdatePlayer1Icon();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                SoundManager.Instance.PlaySound("toggle");
                player1Selection = (player1Selection + 1) % ACTIVE_OPTIONS;
                UpdatePlayer1Icon();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SoundManager.Instance.PlaySound("selection");
                player1Confirmed = true;
            }
        }
        else 
        {
            HideSelection(player1Options);
        }
    }

    void HandlePlayer2Input()
    {
        if (!player2Confirmed)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SoundManager.Instance.PlaySound("toggle");
                player2Selection = (player2Selection - 1 + ACTIVE_OPTIONS) % ACTIVE_OPTIONS;
                UpdatePlayer2Icon();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SoundManager.Instance.PlaySound("toggle");
                player2Selection = (player2Selection + 1) % ACTIVE_OPTIONS;
                UpdatePlayer2Icon();
            }

            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                SoundManager.Instance.PlaySound("selection");
                player2Confirmed = true;
            }
        }
        else 
        {
            HideSelection(player2Options);
        }
    }

    // hides all options once player confirms their character choice
    void HideSelection(Image[] options)
    {
        if (options[0].IsActive())
        {
            foreach (var option in options)
            {
                option.gameObject.SetActive(false);
            }
        }
    }

    void UpdatePlayer1Icon()
    {
        // player1Icon.sprite = characterSprites[player1Selection];
        player1Icon.sprite = Resources.Load<Sprite>("Sprites/" + characterSprites[player1Selection].name + "Icon");
        
        player1Description.text = characterDescriptions[player1Selection];

        UpdateSelectionOpacity(player1Options, player1Selection);
    }

    void UpdatePlayer2Icon()
    {
        // player2Icon.sprite = characterSprites[player2Selection];
        player2Icon.sprite = Resources.Load<Sprite>("Sprites/" + characterSprites[player2Selection].name + "Icon");

        player2Description.text = characterDescriptions[player2Selection];

        UpdateSelectionOpacity(player2Options, player2Selection);
    }

    // makes all other character options less opaque while character toggles through selection
    void UpdateSelectionOpacity(Image[] options, int selectedIndex)
    {
        for (int i = 0; i < options.Length; i++)
        {
            Color currColor = options[i].color;
            currColor.a = (i == selectedIndex) ? 1.0f : 0.5f;
            options[i].color = currColor;
        }
    }

    // Function to be called when Start Game button is clicked
    private void StartGame()
    {
        Debug.Log("Game Started with Player 1: " + characterSprites[player1Selection].name + " and Player 2: " + characterSprites[player2Selection].name);
        // setup player 1 data
        PlayerData.player1Sprite = "Sprites/" + characterSprites[player1Selection].name;
        PlayerData.player1CharIdx = player1Selection;
        PlayerData.p1StartingWeapon = characterStartingWeapons[player1Selection];
        PlayerData.p1Bonus = characterBonuses[player1Selection];
        // setup player 2 data
        PlayerData.player2Sprite = "Sprites/" + characterSprites[player2Selection].name;
        PlayerData.player2CharIdx = player2Selection;
        PlayerData.p2StartingWeapon = characterStartingWeapons[player2Selection];
        PlayerData.p2Bonus = characterBonuses[player2Selection];

        SceneManager.LoadScene("Loading");
    }
}
