/*
This script:
- For the player's damage trigger gameobject: Collision layer should be PDamageTrigger
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerDamage : MonoBehaviour
{
    public Player player;

    // Collisions with enemies / enemy projectiles: The PDamageTrigger collision layer only interacts with the Enemy layer
    void OnTriggerEnter2D(Collider2D col)
    {
        player.beingDamaged = true;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        player.beingDamaged = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        player.beingDamaged = false;
    }
}
