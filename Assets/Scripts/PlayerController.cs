using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public static class PlayerData
{
    public static String player1Sprite;
    public static int player1CharIdx;
    public static String player2Sprite;
    public static int player2CharIdx;

    // new
    public static WeaponType p1StartingWeapon;
    public static WeaponType p2StartingWeapon;
    public static CharacterType p1Bonus;
    public static CharacterType p2Bonus;
}

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 3.5f;
    public bool isPlayer2 = false;
    private float screenWidth;
    private float screenHeight;
    public AudioSource backgroundMusicSource;
    private HashSet<WeaponType> weapons;
    private Transform playerSpriteTransform;
    public Animator animator;
    public RuntimeAnimatorController[] characterAnimatorControllers;
    public GameObject player1Image;
    public GameObject player2Image;
    private CharacterType characterType = CharacterType.NONE;
    private Vector2 recentForward = Vector2.right;


    private void Awake()
    {

    }
    
    void Start()
    {
        // Getting the screen bounds -- may not need anymore
        screenWidth = Camera.main.orthographicSize * Camera.main.aspect;
        screenHeight = Camera.main.orthographicSize;  

        playerSpriteTransform = transform.Find("player sprite");

        weapons = new HashSet<WeaponType>();
        // DetectStartingWeapons(); // not needed due to character selection

        // handles setup from character selection
        if (!isPlayer2)
        {
            // updates animator controller
            animator.runtimeAnimatorController = characterAnimatorControllers[PlayerData.player1CharIdx];
            
            // update player icon in UI
            player1Image.GetComponent<Image>().sprite = Resources.Load<Sprite>(PlayerData.player1Sprite + "Icon");

            // add starting weapon 
            AddWeapon(PlayerData.p1StartingWeapon);

            // apply bonus
            ApplyBonus(PlayerData.p1Bonus);

        }
        else 
        {
            // updates animator controller
            animator.runtimeAnimatorController = characterAnimatorControllers[PlayerData.player2CharIdx];
            
            // update player icon in UI (and make it face opposite)
            player2Image.GetComponent<Image>().sprite = Resources.Load<Sprite>(PlayerData.player2Sprite + "Icon");
            Vector3 scale = player2Image.transform.localScale;
            scale.x *= -1;
            player2Image.transform.localScale = scale;

            // add starting weapon 
            AddWeapon(PlayerData.p2StartingWeapon);

            // apply bonus
            ApplyBonus(PlayerData.p2Bonus);
        }

        // sets up player UI
        WeaponManager.Instance.UpdatePlayerWeaponIcons(this);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement() 
    {
        Vector3 moveInput = new Vector3(0f, 0f, 0f);
        Vector3 newScale = playerSpriteTransform.localScale;

        if (!isPlayer2) 
        {
            // Player 2 WASD
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                moveInput.x = -1;
                newScale.x = -Mathf.Abs(newScale.x);
            }
            else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                moveInput.x = 1;
                newScale.x = Mathf.Abs(newScale.x);
            }
            else moveInput.x = 0;

            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                moveInput.y = 1;
            }
            else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            {
                moveInput.y = -1;
            }
            else moveInput.y = 0;
        }
        else 
        {
            // Player 1 Arrow Keys
            if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                moveInput.x = -1;
                newScale.x = -Mathf.Abs(newScale.x);
            }
            else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            {
                moveInput.x = 1;
                newScale.x = Mathf.Abs(newScale.x);
            }
            else moveInput.x = 0;

            if (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
            {
                moveInput.y = 1;
            }
            else if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
            {
                moveInput.y = -1;
            }
            else moveInput.y = 0;
        }

        playerSpriteTransform.localScale = newScale;
        Vector3 pos = transform.position + (moveInput.normalized * moveSpeed * Time.deltaTime);
        
        // update most recent 'forward' if user input
        if (moveInput != Vector3.zero)
        {
            recentForward = new Vector2(moveInput.x, moveInput.y).normalized;
        }
        
        if (moveInput == Vector3.zero){
            animator.SetBool("isMoving", false);
        }
        else{
            animator.SetBool("isMoving", true);
        }
        transform.position = pos;
    }

    // CURRENTLY NOT BEING USED, KEEP FOR NOW ******
    // detects all weapons the player starts with and adds to their weapon list
    private void DetectStartingWeapons()
    {
        Weapon[] playerWeapons = GetComponents<Weapon>(); 

        foreach (Weapon weapon in playerWeapons)
        {
            weapons.Add(weapon.type);  
        }
        string p = isPlayer2 ? "player 2" : "player 1";
        Debug.Log($"{p}: Starting Weapons: " + string.Join(", ", weapons));
    }

    // weapon / weapon manager can call this to add weapon to player 
    public void AddWeapon(WeaponType newWeaponType)
    {
        // make sure player does not already have the weapon
        if (weapons.Contains(newWeaponType))
        {
            Debug.Log("Player already has this weapon!");
            return;
        }

        weapons.Add(newWeaponType);

        // add the weapon script to the player
        if (newWeaponType == WeaponType.LASER)
        {
            gameObject.AddComponent<LaserPower>();
        }
        else if (newWeaponType == WeaponType.FOAM)
        {
            gameObject.AddComponent<FoamPower>();
        }
        else if (newWeaponType == WeaponType.BLADE)
        {
            gameObject.AddComponent<BladePower>();
        }
        else if (newWeaponType == WeaponType.BOMB)
        {
            gameObject.AddComponent<BombPower>();
        }
        else if (newWeaponType == WeaponType.SIGNAL)
        {
            gameObject.AddComponent<SignalPower>();
        }
        else if (newWeaponType == WeaponType.ROLLINGBALL)
        {
            gameObject.AddComponent<RollingBallPower>();
        }
        else if (newWeaponType == WeaponType.SWORD)
        {
            gameObject.AddComponent<SwordPower>();
        }
        string p = isPlayer2 ? "player 2" : "player 1";

        Debug.Log($"Weapon {newWeaponType} added to {p}");
    }

    public void UpgradeWeapon(WeaponType wt)
    {
        // make suer player has the weapon
        if (!weapons.Contains(wt))
        {
            Debug.Log("Player does NOT have this weapon!");
            return;
        }

        // upgrade the player's corresponding weapon
        if (wt == WeaponType.LASER)
        {
            LaserPower laserPower = gameObject.GetComponent<LaserPower>();
            laserPower.LevelUp();
            Debug.Log("LASER POWER LEVEL: " + laserPower.GetCurrLevel());
        }
        else if (wt == WeaponType.FOAM)
        {
            FoamPower foamPower = gameObject.GetComponent<FoamPower>();
            foamPower.LevelUp();
        }
        else if (wt == WeaponType.BLADE)
        {
            BladePower bladePower = gameObject.GetComponent<BladePower>();
            bladePower.LevelUp();
        }
        else if (wt == WeaponType.BOMB)
        {
            BombPower bombPower = gameObject.GetComponent<BombPower>();
            bombPower.LevelUp();
        }
        else if (wt == WeaponType.SIGNAL)
        {
            SignalPower signalPower = gameObject.GetComponent<SignalPower>();
            signalPower.LevelUp();
        }
        else if (wt == WeaponType.ROLLINGBALL)
        {
            RollingBallPower rollingBallPower = gameObject.GetComponent<RollingBallPower>();
            rollingBallPower.LevelUp();
        }
        else if (wt == WeaponType.SWORD)
        {
            SwordPower swordPower = gameObject.GetComponent<SwordPower>();
            swordPower.LevelUp();
        }

        string p = isPlayer2 ? "player 2" : "player 1";
        Debug.Log($"Weapon {wt} successfully upgraded for {p}");
    }

    // will deactivate all weapons on player death
    public void DeactivateAllWeapons()
    {
        foreach (var w in weapons)
        {
            if (w == WeaponType.LASER)
            {
                gameObject.GetComponent<LaserPower>().Deactivate();
            }
            else if (w == WeaponType.FOAM)
            {
                gameObject.GetComponent<FoamPower>().Deactivate();
            }
            else if (w == WeaponType.BLADE)
            {
                gameObject.GetComponent<BladePower>().Deactivate();
            }
            else if (w == WeaponType.BOMB)
            {
                gameObject.GetComponent<BombPower>().Deactivate();
            }
            else if (w == WeaponType.SIGNAL)
            {
                gameObject.GetComponent<SignalPower>().Deactivate();
            }
            else if (w == WeaponType.ROLLINGBALL)
            {
                gameObject.GetComponent<RollingBallPower>().Deactivate();
            }
            else if (w == WeaponType.SWORD)
            {
                gameObject.GetComponent<SwordPower>().Deactivate();
            }
        }
    }

    // will activate all weapons on player when revived
    public void ActivateAllWeapons()
    {
        foreach (var w in weapons)
        {
            if (w == WeaponType.LASER)
            {
                gameObject.GetComponent<LaserPower>().Activate();
            }
            else if (w == WeaponType.FOAM)
            {
                gameObject.GetComponent<FoamPower>().Activate();
            }
            else if (w == WeaponType.BLADE)
            {
                gameObject.GetComponent<BladePower>().Activate();
            }
            else if (w == WeaponType.BOMB)
            {
                gameObject.GetComponent<BombPower>().Activate();
            }
            else if (w == WeaponType.SIGNAL)
            {
                gameObject.GetComponent<SignalPower>().Activate();
            }
            else if (w == WeaponType.ROLLINGBALL)
            {
                gameObject.GetComponent<RollingBallPower>().Activate();
            }
            else if (w == WeaponType.SWORD)
            {
                gameObject.GetComponent<SwordPower>().Activate();
            }
        }
    }

    public HashSet<WeaponType> GetPlayerWeapons()
    {
        return weapons;
    }

    public bool IsWeaponMaxLevel(WeaponType wt)
    {
        Debug.Log($"checking if {wt} is max level out of {string.Join(", ", weapons)}");

        // make sure player has the weapon
        if (!weapons.Contains(wt))
        {
            Debug.Log("Player does NOT have this weapon!");
            return false;
        }

        // check if corresponding weapon is max level
        if (wt == WeaponType.LASER)
        {
            LaserPower laserPower = gameObject.GetComponent<LaserPower>();
            return laserPower.GetIsMaxLevel();
        }
        else if (wt == WeaponType.FOAM)
        {
            FoamPower foamPower = gameObject.GetComponent<FoamPower>();
            return foamPower.GetIsMaxLevel();
        }
        else if (wt == WeaponType.BLADE)
        {
            BladePower bladePower = gameObject.GetComponent<BladePower>();
            return bladePower.GetIsMaxLevel();
        }
        else if (wt == WeaponType.BOMB)
        {
            BombPower bombPower = gameObject.GetComponent<BombPower>();
            return bombPower.GetIsMaxLevel();
        }
        else if (wt == WeaponType.SIGNAL)
        {
            SignalPower signalPower = gameObject.GetComponent<SignalPower>();
            return signalPower.GetIsMaxLevel();
        }
        else if (wt == WeaponType.ROLLINGBALL)
        {
            RollingBallPower rollingBallPower = gameObject.GetComponent<RollingBallPower>();
            return rollingBallPower.GetIsMaxLevel();
        }
        else if (wt == WeaponType.SWORD)
        {
            SwordPower swordPower = gameObject.GetComponent<SwordPower>();
            return swordPower.GetIsMaxLevel();
        }

        // shouldnt get here 
        return false;
    }

    public bool GetIsPlayer2()
    {
        return isPlayer2;
    }


    public Vector2 GetPosition()
    {
        return transform.position;
    }

    private void ApplyBonus(CharacterType charType)
    {
        characterType = charType;

        switch (charType)
        {
        case CharacterType.HEALER:
            PlayerHealthController playerHealthController = GetComponent<PlayerHealthController>();
            playerHealthController.SetMaxHealth(Mathf.RoundToInt(1.5f * playerHealthController.GetMaxHealth()));
            break;
        case CharacterType.SPEED:
            moveSpeed *= 1.25f;
            break;
        case CharacterType.XP:
            // handled in XP.cs
            break;
        }
    }

    public CharacterType GetCharacterType()
    {
        return characterType;
    }

    public bool AreAllWeaponsMaxed()
    {
        int weaponCount = weapons.Count();

        if (weaponCount < 3) return false;
        
        foreach (WeaponType wt in weapons)
        {
            if (!IsWeaponMaxLevel(wt))
            {
                return false;
            }
        }

        return true;
    }

    public Vector2 GetRecentForward()
    {
        return recentForward;
    }
}
