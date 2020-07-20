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
        WeaponTypeShotgun,
        WeaponTypeSniper,
        Health,
    }

    public Type powerupType;

    void OnTriggerEnter2D(Collider2D col)
    {
        // The basic upgrades better the weapon for 8%
        switch(powerupType)
        {
            case Type.FiringSpeed:
                StaticPlayer.UpgradeFiringSpeed();
            break;
            case Type.ShotMoveSpeed:
                StaticPlayer.UpgradeBulletSpeed();
            break;
            case Type.Damage:
                StaticPlayer.UpgradeDamage();
            break;
            case Type.MoveSpeed:
                StaticPlayer.UpgradeMoveSpeed();
            break;
            case Type.WeaponTypeMagnum:
                if(StaticPlayer.weaponType != 0)
                {
                    StaticPlayer.SwitchToWeapon(0);
                    Player.plrInstance.RefreshSprite();
                }
            break;
            case Type.WeaponTypeRifle:
                if(StaticPlayer.weaponType != 1)
                {
                    StaticPlayer.SwitchToWeapon(1);
                    Player.plrInstance.RefreshSprite();
                }
            break;
            case Type.WeaponTypeShotgun:
                if(StaticPlayer.weaponType != 2)
                {
                    StaticPlayer.SwitchToWeapon(2);
                    Player.plrInstance.RefreshSprite();
                }
            break;
            case Type.WeaponTypeSniper:
                if(StaticPlayer.weaponType != 3)
                {
                    StaticPlayer.SwitchToWeapon(3);
                    Player.plrInstance.RefreshSprite();
                }
            break;
            case Type.Health:
                StaticPlayer.HealPlayer();
                Player.plrInstance.HealthUpdate();
            break;
        }

        this.gameObject.SetActive(false);
    }
    
    
}
