/*
In this script:
- External Player stat management
- Stats persist regardless of Scene changes due to the static nature, but will be nullified at the start of a session
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticPlayer
{
    // Health
    public static float currHP = 600f;
    public static float maxHP = 600f;
    public static bool statusChanged = false;

    // Movement
    public static float moveSpeed = 4.5f;
    public static Transform playerTransform;
    public static int joystickType;

    // Shooting
    public static float shotDamage = 100f;
    public static bool alive = true;
    public static int weaponType = 0;
    public static float shotDelayInSec = 0.75f;
    public static float shotMoveSpeed = 15f;
    public static float shotStunTime = 0.12f;

    /*
    Weapon types:

    0 = Magnum: Baseline weapon
        0.75/s Firing Speed
        15 unit/s Shot MoveSpeed
        100 Damage
        0.12s Stun

    1 = Rifle: 
        3 x Firing speed
        1/2.25 x Damage > Balansing as you spend move time standing still
        1.25 x Shot MoveSpeed 
        0.08s Stun

    2 = Shotgun:
        1/2 x Firing speed
        5 Shot spread
        2/5 x Damage
        0.15s Stun

    3 = Bolt-action Sniper Rifle:
        1/2 x Firing speed
        1.5 x Shot MoveSpeed 
        2 x Damage
        0.25s Stun
        TODO: Laser-Sight
    */

    public static List<int> acquiredWeapons = new List<int>();

    public static void DamagePlayer(float dmgAmount)
    {
        currHP -= dmgAmount;
        if(currHP < 0)
        {
            currHP = 0;
        }

        Player.plrInstance.damageTimer = 0f;
        Player.plrInstance.playerSpriteRend.color = Color.red; // Changing the sprite color for a short while to better indicate being damaged
        Player.plrInstance.HealthUpdate();
        statusChanged = true;
    }

    // Resets player stats to default at new session start
    public static void ResetStats()
    {
        maxHP = 600f; 
        currHP = maxHP;
        moveSpeed = 4.5f;
        playerTransform = null;
        shotDamage = 100f;
        alive = true;
        weaponType = 0;
        shotDelayInSec = 0.75f;
        shotMoveSpeed = 15f;
        shotStunTime = 0.12f;
        statusChanged = false;
        acquiredWeapons = new List<int>();
    }

    // Adjusting the values based on their multipliers (keeping the collected powerups' effects)
    public static void AdjustViaMagnum()
    {
        switch(weaponType)
        {
            case 1:
                shotDelayInSec *= 3f;
                shotDamage *= 2.25f;
                shotMoveSpeed *= 0.8f;
            break;
            case 2:
                shotDelayInSec /= 2f;
                shotDamage *= (5f/2f);
            break;
            case 3:
                shotDelayInSec /= 2f;
                shotDamage /= 2f;
                shotMoveSpeed *= 2/3f;
            break;
        }
        shotStunTime = 0.12f;
        weaponType = 0;
    }

    // Changing values back to default magnum values: The baseline for other weapons
    public static void DefaultToMagnum()
    {
        shotDelayInSec = 0.75f;
        shotDamage = 100f;
        shotStunTime = 0.12f;
        shotMoveSpeed = 15f;
        weaponType = 0;
        
        bool alreadyAcquired = acquiredWeapons.Contains(0);
        if(!alreadyAcquired)
        {
            acquiredWeapons.Add(0);
            GameplayManager.gpManagerInstance.RefreshWeaponButtons();
        }
    }

    public static void SwitchToWeapon(int weaponIndex)
    {
        bool alreadyAcquired;
        switch(weaponIndex)
        {
            case 0:
                AdjustViaMagnum();
                ScreenMessager.smInstance.ShowMessage("Switched weapon to Magnum!");
            break;
            case 1:
                AdjustViaMagnum();
                shotDelayInSec *= (1f/3f);
                shotDamage *= (1f/2.25f);
                shotMoveSpeed *= 1.25f;
                shotStunTime = 0.1f;
                weaponType = 1;
                // Adding to acquired weapons if not already there
                alreadyAcquired = acquiredWeapons.Contains(1);
                if(!alreadyAcquired)
                {
                    acquiredWeapons.Add(1);
                    GameplayManager.gpManagerInstance.RefreshWeaponButtons();
                }
                ScreenMessager.smInstance.ShowMessage("Switched weapon to Assault Rifle");
            break;
            case 2:
                AdjustViaMagnum();
                shotDelayInSec *= 2;
                shotDamage *= (2f/5f);
                shotStunTime = 0.15f;
                weaponType = 2;
                // Adding to acquired weapons if not already there
                alreadyAcquired = acquiredWeapons.Contains(2);
                if(!alreadyAcquired)
                {
                    acquiredWeapons.Add(2);
                    GameplayManager.gpManagerInstance.RefreshWeaponButtons();
                }
                ScreenMessager.smInstance.ShowMessage("Switched weapon to Shotgun");
            break;
            case 3:
                AdjustViaMagnum();
                shotDelayInSec *= 2f;
                shotDamage *= 2f;
                shotMoveSpeed *= 1.5f;
                shotStunTime = 0.25f;
                weaponType = 3;
                // Adding to acquired weapons if not already there
                alreadyAcquired = acquiredWeapons.Contains(3);
                if(!alreadyAcquired)
                {
                    acquiredWeapons.Add(3);
                    GameplayManager.gpManagerInstance.RefreshWeaponButtons();
                }
                ScreenMessager.smInstance.ShowMessage("Switched weapon to Sniper Rifle");
            break;
        }
    }

    // Basic powerups
    public static void UpgradeFiringSpeed()
    {
        shotDelayInSec *= 0.92f;
        ScreenMessager.smInstance.ShowMessage("Fire rate increased!");
    }
    public static void UpgradeBulletSpeed()
    {
        shotMoveSpeed *= 1.08f;
        ScreenMessager.smInstance.ShowMessage("Bullet speed increased!");
    }
    public static void UpgradeDamage()
    {
        shotDamage *= 1.08f;
        ScreenMessager.smInstance.ShowMessage("Damage increased!");
    }
    public static void UpgradeMoveSpeed()
    {
        moveSpeed *= 1.08f;
        ScreenMessager.smInstance.ShowMessage("Movement speed increased!");
    }
    public static void HealPlayer()
    {
        currHP += 200f;
        if(currHP > maxHP)
        {
            currHP = maxHP;
        }
        ScreenMessager.smInstance.ShowMessage("Picked up a health kit!");
    }

}
