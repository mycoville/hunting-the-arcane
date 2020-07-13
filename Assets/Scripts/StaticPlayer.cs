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
    public static float currHP = 500f;
    public static float maxHP = 500f;
    public static bool statusChanged = false;

    // Movement
    public static float moveSpeed = 4.5f;
    public static Transform playerTransform;

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
    1 = Rifle
    2 = TODO | Shotgun
    3 = Bolt-action Sniper Rifle
    */


    public static void DamagePlayer(float dmgAmount)
    {
        StaticPlayer.currHP -= dmgAmount;
        if(StaticPlayer.currHP < 0)
        {
            StaticPlayer.currHP = 0;
        }
    }

    // Resets player stats to default at new session start
    public static void ResetStats()
    {
        maxHP = 500f;
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
    }
}
