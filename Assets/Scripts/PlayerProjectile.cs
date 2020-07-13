/*
In this script:
- Player projectile collision checking
- Player projectile impact force
- Damaging (and possibly stunning) suitable targets
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private Rigidbody2D rb2d;

    void Awake()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        rb2d.velocity = this.transform.right * StaticPlayer.shotMoveSpeed;
    }

    // If the bullet's trigger Collider hits something...
    void OnTriggerEnter2D(Collider2D col)
    {
        Enemy enemyScript = col.GetComponent<Enemy>();

        if(enemyScript != null)
        {
            // Damaging enemy (currently by a static amount)
            enemyScript.DamageEnemy(StaticPlayer.shotDamage);

            Follower followerScript = col.GetComponent<Follower>();
            // Stun enemy if its type is Follower
            if(followerScript != null)
            {
                followerScript.Stun(StaticPlayer.shotStunTime);
            }

            // Calling the StaticManager to check the enemy list HP situation
            StaticManager.CheckIfEnemiesDead();
        }

        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
