/*
This script:
- For the player's damage trigger gameobject: Collision layer should be PDamageTrigger
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerDamage : MonoBehaviour
{
    public ContactFilter2D hitFilter;

    private float leaveTimer = 1f;

    // Collisions with enemies / enemy projectiles: The PDamageTrigger collision layer only interacts with the Enemy layer
    void OnTriggerEnter2D(Collider2D col)
    {
        leaveTimer = 0f;
        Player.plrInstance.beingDamaged = true;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        leaveTimer = 0f;
        Player.plrInstance.beingDamaged = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Player.plrInstance.beingDamaged = false;
    }

    void FixedUpdate()
    {
        leaveTimer += Time.fixedDeltaTime;

        // Making sure that destroyed enemies or enemy projectiles register as leaving the player's hitbox
        if(Player.plrInstance.beingDamaged && leaveTimer > 0.25f)
        {
            if(Physics2D.OverlapCircle(this.transform.position, 0.35f, hitFilter.layerMask))
            {
                //Player.plrInstance.beingDamaged = true;
                Debug.Log("Enemy still in reach");
            }
            else
            {
                Player.plrInstance.beingDamaged = false;
                Debug.Log("No enemy");
            }
        }
    }
}
