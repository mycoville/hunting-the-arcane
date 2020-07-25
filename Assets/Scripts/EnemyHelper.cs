/*
In this script:
- Helper functions for all enemy types to utilize
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyHelper
{
    // Getting a random direction vector for movement etc
    public static Vector2 GetRandomDirection()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    public static void RotateSpriteToDirection(Vector2 moveDirection, Transform transformToRotate)
    {
        if(moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transformToRotate.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
