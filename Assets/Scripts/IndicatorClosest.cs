/*
In this script:
- Player assistance via indicating the closest enemy (if there are only a few left)
- 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorClosest : MonoBehaviour
{
    private Transform indicatorTF;
    private Transform targetTF;
    public GameObject indicatorSpriteObj;
    private bool fewEnemiesLeft = false;

    public static IndicatorClosest icInstance; // Static version for ease of access

    void Awake()
    {
        icInstance = this;
    }

    void Start()
    {
        indicatorTF = this.transform;
        fewEnemiesLeft = false;
        indicatorSpriteObj.SetActive(false);
        targetTF = null;
    }

    void FixedUpdate()
    {
        if(fewEnemiesLeft)
        {
            CheckForClosestEnemy();
            if(targetTF) // If we have a closest "target"
            {
                RotateSpriteToDirection(targetTF.position);
            }
        }
    }

    // Called by EnemyManager on enemy death
    public void CheckIfFewEnemies() 
    {
        int currentEnemyAmount = EnemyManager.emInstance.enemyTransforms.Count;

        if(currentEnemyAmount < 4 && currentEnemyAmount > 0) // If there are between 3 and 1 enemies left
        {
            indicatorSpriteObj.SetActive(true);
            fewEnemiesLeft = true;
        }
        else if(currentEnemyAmount == 0) // No need to show the indicator or rotate it in case all enemies are dead
        {
            indicatorSpriteObj.SetActive(false);
            fewEnemiesLeft = false;
        }
    }

    private void CheckForClosestEnemy()
    {
        List<Transform> enemyTransforms = EnemyManager.emInstance.enemyTransforms;
        float previousDistance = 9999;

        if(enemyTransforms.Count != 0) // There are still enemies
        {
            for(int i = 0; i < enemyTransforms.Count; i++)
            {
                float currentDistance = Vector2.Distance(indicatorTF.position, enemyTransforms[i].position);
                // At the first index we set up our values for comparison
                if(i == 0)
                {
                    previousDistance = currentDistance;
                    targetTF = enemyTransforms[0];
                }
                else
                {
                    if(currentDistance < previousDistance)
                    {
                        previousDistance = currentDistance;
                        targetTF = enemyTransforms[i];
                    }
                }
            }
        }
        else
        {
            targetTF = null;
        }

    }
    
    // Adjusting the Sprite rotation to match the given vector direction
    private void RotateSpriteToDirection(Vector2 targetPos)
    {
        Vector2 lookVector = targetPos - (Vector2)indicatorTF.position;

        if(lookVector != Vector2.zero)
        {
            float angle = Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg;
            indicatorTF.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
