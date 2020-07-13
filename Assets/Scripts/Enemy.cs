/*
In this script:
- Enemy health and damage management
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHP = 1000f;
    public float currHP = 1000f;

    private float damageTimer = 0f;
    public SpriteRenderer spriteRend;

    // Called by the PlayerProjectile when it hits the enemy collider
    public void DamageEnemy(float dmgAmount)
    {
        currHP -= dmgAmount;
        if(currHP < 0)
        {
            currHP = 0;
        }
        damageTimer = 0;
        spriteRend.color = Color.red;
    }

    void FixedUpdate()
    {
        if(damageTimer > 0.2f)
        {
            // Changing the sprite color to indicate being damaged
            spriteRend.color = Color.white;
        }

        damageTimer += Time.fixedDeltaTime;
    }

}
