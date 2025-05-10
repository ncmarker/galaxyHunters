using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public enum WeaponType
{
    FOAM,
    BLADE,
    LASER,
    BOMB,
    SIGNAL, 
    ROLLINGBALL,
    SWORD,
    NONE
}
// also add to start() when we add more weapons
// and add to player controller methods to check for the new type
// and to WeaponInfo.cs
// and to GameLogger.cs

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;
    private List<WeaponType> allWeapons = new List<WeaponType>();

    public Image[] player1Equipped;
    public Image[] player2Equipped;

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
        allWeapons = new List<WeaponType>
        {
            WeaponType.LASER,
            WeaponType.BLADE,
            WeaponType.FOAM, 
            WeaponType.BOMB, 
            WeaponType.SIGNAL,
            WeaponType.ROLLINGBALL,
            WeaponType.SWORD
        };
    }

    // return a random weapon that the player doesn't already have
    public WeaponType GetRandomWeapon(PlayerController player, HashSet<WeaponType> additionalConstraints = null)
    {
        HashSet<WeaponType> currentWeaponTypes = new HashSet<WeaponType>(player.GetPlayerWeapons());

        if (additionalConstraints != null)
        {
            currentWeaponTypes.UnionWith(additionalConstraints);
        }

        // filter out the weapons that the player already has
        List<WeaponType> availableWeaponTypes = new List<WeaponType>();

        foreach (WeaponType weaponType in allWeapons)
        {
            if (!currentWeaponTypes.Contains(weaponType))
            {
                availableWeaponTypes.Add(weaponType);
            }
        }

        // if there are available weapon types to choose from, return a random one
        if (availableWeaponTypes.Count > 0)
        {
            int randomIndex = Random.Range(0, availableWeaponTypes.Count);
            return availableWeaponTypes[randomIndex];
        }

        Debug.Log("NO WEAPON FOUND in GetRandomWeapon()");
        return WeaponType.NONE;
    }

    // updates the UI for a player's weapons
    public void UpdatePlayerWeaponIcons(PlayerController player)
    {
        HashSet<WeaponType> pWeapons = player.GetPlayerWeapons();
        
        int currEquip = 0;

        if (!player.GetIsPlayer2()) 
        {
            foreach (WeaponType wt in pWeapons) 
            {
                player1Equipped[currEquip].sprite = Resources.Load<Sprite>($"Sprites/{wt}");
                player1Equipped[currEquip].color = Color.white;
                currEquip++;
            }
        }
        else 
        {
            foreach (WeaponType wt in pWeapons) 
            {
                player2Equipped[currEquip].sprite = Resources.Load<Sprite>($"Sprites/{wt}");
                player2Equipped[currEquip].color = Color.white;
                currEquip++;
            }
        }
    }
}
