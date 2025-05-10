using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class WeaponInfo : MonoBehaviour
{
    // UI Elements
    public Image selectedWeapon;
    private int selectedWeaponIdx = 0;
    public Image[] weaponChoices;
    public Sprite[] weapons;
    public Text selectedWeaponDescription;
    public Text selectedWeaponName;
    public Text selectedWeaponDamage;
    public Text selectedWeaponUpgrade;
    public Text weaponNotes;

    private HomeManager homeManager;
    
    // data -- update when new weapon added
    private const int AVAILABLE_WEAPONS = 7;
    private String[] weaponNames = {"Blades", "Foam", "Laser", "Bombs", "Signal", "Ball", "Sword"};
    private String[] weaponDamages = {"5", "1", "5", "20", "3", "20", "3"};
    private String[] weaponUpgrades = {
        "Blade number increased", 
        "Foam radius increased 1.5x", 
        "Lasers shot in additional direction", 
        "+2 bombs thrown", 
        "Signal radius increased 1.5x",
        "Ball size increased 50%",
        "Additional slash direction"
    };
    private String[] weaponDescriptions = {
        "Blades rotating around player", 
        "Damage Radius around player", 
        "Beams shoot outward from player", 
        "Bombs thrown randomly around player", 
        "Deployable building with damage radius",
        "Rolling ball damaging anything in its path",
        "Slash enemies close by in a restricted direction"
    };
    private String signalNote = "Place building with L/R shift, cooldown displayed above XP bar";


    // called each time the WeaponUpgradeUI is set to active
    void OnEnable()
    {
        SetupWeaponPanel();
        homeManager = GameObject.Find("HomeManager").GetComponent<HomeManager>();

        // start with weapon 0
        selectedWeaponIdx = 0;
        HighlightChoice(selectedWeaponIdx, weaponChoices);
    }

    // waits for both players to decide before unpausing game 
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            homeManager.CloseWeaponInfo();
        }

        HandleIput();
        
    }     

    // adds current weapons / new weapon to player 2 upgrade panel 
    private void SetupWeaponPanel()
    {
        int currWeapon = 0;
        foreach (Image img in weaponChoices) 
        {
            if (currWeapon >= weapons.Length)
            {
                img.sprite = Resources.Load<Sprite>("Sprites/coming_soon");
            }
            else
            {
                img.sprite = weapons[currWeapon];
                currWeapon++;
            }
        }
    }

    private void HandleIput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SoundManager.Instance.PlaySound("toggle");
            selectedWeaponIdx = Mathf.Max(0, selectedWeaponIdx - 1);
            HighlightChoice(selectedWeaponIdx, weaponChoices);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            SoundManager.Instance.PlaySound("toggle");
            selectedWeaponIdx = Mathf.Min(weapons.Length - 1, selectedWeaponIdx + 1);
            HighlightChoice(selectedWeaponIdx, weaponChoices);
        }
    }

    private void HighlightChoice(int choiceIndex, Image[] choices)
    {
        UpdateSelectionOpacity(choices, choiceIndex);

        selectedWeapon.sprite = weapons[choiceIndex];
        selectedWeaponDescription.text = weaponDescriptions[choiceIndex];
        selectedWeaponName.text = weaponNames[choiceIndex];
        selectedWeaponDamage.text = weaponDamages[choiceIndex] + "/hit";
        selectedWeaponUpgrade.text = weaponUpgrades[choiceIndex];

        weaponNotes.text = weaponNames[choiceIndex] == "Signal" ? signalNote : "";
    }

    void UpdateSelectionOpacity(Image[] options, int selectedIndex)
    {
        for (int i = 0; i < options.Length; i++)
        {
            Color currColor = options[i].color;
            currColor.a = (i == selectedIndex) ? 1.0f : 0.5f;
            options[i].color = currColor;
        }
    }
}
