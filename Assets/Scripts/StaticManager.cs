/*
This static script:
- Provides easy access to several variables regardless of the script requesting them
- Checks the status of the enemy (transform) list
- Updates the enemy list
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticManager
{
    public static List<Transform> enemyTransforms = new List<Transform>();

    public static bool listChanged = false;

    public static List <Transform> tfsToRemove = new List<Transform>(); // List for enemy transforms to remove as the enemies have been killed
    

    // Called by PlayerProjectile each time it manages to hit an enemy
    public static void CheckIfEnemiesDead()
    {
        tfsToRemove = new List<Transform>();
        
        for(int i = 0; i < enemyTransforms.Count; i++)
        {
            if(enemyTransforms[i].gameObject.GetComponent<Enemy>().currHP == 0)
            {
                tfsToRemove.Add(enemyTransforms[i]);
            }
        }

        listChanged = true; // This variable indicates the need for other updates (such as EnemyManager's object removal etc.)
    }
}
