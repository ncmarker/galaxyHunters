using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public abstract class Weapon  : MonoBehaviour
{
    public WeaponType type;
    public abstract void Activate();
    public abstract void Deactivate();
    public abstract void DoDamage();

    public abstract void LevelUp();
    public abstract bool GetIsMaxLevel();

    protected float cooldown;
    protected float currCooldown = 0.0f;
    protected float duration;
    protected int damage;
    protected int currLevel = 1;
    protected bool isActive = false;
    protected bool isMaxLevel;
    protected int maxLevel;

    public int GetCurrLevel()
    {
        return currLevel;
    }

}
