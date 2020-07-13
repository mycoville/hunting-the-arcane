/*
In this script:
- Powerup management and their effects on the StaticPlayer script
- Checking for collisions with Player
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum Type
    {
        FiringSpeed,
        ShotMoveSpeed,
        Damage,
        MoveSpeed,
        WeaponTypeMagnum,
        WeaponTypeRifle,
        WeaponTypeSniper,
        Health,
    }
    /*
    Weapon types:

    0 = Magnum: Baseline weapon
        0.75/s Firing Speed
        15 unit/s Shot MoveSpeed
        100 Damage
        0.12s Stun

    1 = Rifle: 
        4 x Firing speed
        1/4 x Damage
        1.25 x Shot MoveSpeed 
        0.08s Stun

    2 = TODO | Shotgun:
        1/2 x Firing speed
        5 Shot spread
        TODO: 0.95 x Shot MoveSpeed 
        2/5 x Damage
        0.12s Stun

    3 = Bolt-action Sniper Rifle:
        1/2 x Firing speed
        1.5 x Shot MoveSpeed 
        2 x Damage
        0.25s Stun
        TODO: Laser-Sight
    */

    public Type powerupType;

    void OnTriggerEnter2D(Collider2D col)
    {
        // The basic upgrades better the weapon for 8%
        switch(powerupType)
        {
            case Type.FiringSpeed:
                StaticPlayer.shotDelayInSec *= 0.92f;
            break;
            case Type.ShotMoveSpeed:
                StaticPlayer.shotMoveSpeed *= 1.08f;
            break;
            case Type.Damage:
                StaticPlayer.shotDamage *= 1.08f;
            break;
            case Type.MoveSpeed:
                StaticPlayer.moveSpeed *= 1.08f;
            break;
            case Type.WeaponTypeMagnum:
                if(StaticPlayer.weaponType != 0)
                {
                    DefaultToMagnum();
                    col.GetComponentInParent<Player>().RefreshSprite();
                }
            break;
            case Type.WeaponTypeRifle:
                if(StaticPlayer.weaponType != 1)
                {
                    AdjustViaMagnum();
                    StaticPlayer.shotDelayInSec *= 0.25f;
                    StaticPlayer.shotDamage *= 0.25f;
                    StaticPlayer.shotMoveSpeed *= 1.25f;
                    StaticPlayer.shotStunTime = 0.08f;
                    StaticPlayer.weaponType = 1;
                    col.GetComponentInParent<Player>().RefreshSprite();
                    
                }
            break;
            case Type.WeaponTypeSniper:
                if(StaticPlayer.weaponType != 3)
                {
                    AdjustViaMagnum();
                    StaticPlayer.shotDelayInSec *= 2f;
                    StaticPlayer.shotDamage *= 2f;
                    StaticPlayer.shotMoveSpeed *= 1.5f;
                    StaticPlayer.shotStunTime = 0.25f;
                    StaticPlayer.weaponType = 3;
                    col.GetComponentInParent<Player>().RefreshSprite();
                }
            break;
            case Type.Health:
                StaticPlayer.currHP += 200f;
                if(StaticPlayer.currHP > StaticPlayer.maxHP)
                {
                    StaticPlayer.currHP = StaticPlayer.maxHP;
                }
                col.GetComponentInParent<Player>().HealthUpdate();
            break;
        }

        this.gameObject.SetActive(false);
    }

    // Changing values back to default magnum values: The baseline for other weapons
    private void DefaultToMagnum()
    {
        StaticPlayer.shotDelayInSec = 0.75f;
        StaticPlayer.shotDamage = 100f;
        StaticPlayer.shotStunTime = 0.12f;
        StaticPlayer.shotMoveSpeed = 15f;
        StaticPlayer.weaponType = 0;
    }

    // Adjusting the values based on their multipliers (keeping the collected powerups' effects)
    private void AdjustViaMagnum()
    {
        switch(StaticPlayer.weaponType)
        {
            case 1:
                StaticPlayer.shotDelayInSec *= 4f;
                StaticPlayer.shotDamage *= 4f;
                StaticPlayer.shotMoveSpeed *= 0.8f;
            break;
            case 3:
                StaticPlayer.shotDelayInSec /= 2f;
                StaticPlayer.shotDamage /= 2f;
                StaticPlayer.shotMoveSpeed *= 2/3f;
            break;
        }
        StaticPlayer.shotStunTime = 0.12f;
        StaticPlayer.weaponType = 0;
    }
}
