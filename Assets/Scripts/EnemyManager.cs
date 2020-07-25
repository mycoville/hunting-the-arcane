/*
In this script:
- Enemy spawning, spawn positions
- Enemy destruction based on StaticManager's values
- Enemy transforms, checking for enemy deaths
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    
    public GameObject enemyFollowerPF; // Follower Prefab
    public GameObject enemyShooterPF; // Shooter Prefab
    public Transform spawnTF;
    public int enemyAmount = 10;

    private int counter = 0;

    // Minimalistic spawn area adjustment: Lists for spawn area corners (top left, bottom right) allocating a rectangular spawn area.
    public List<Transform> spawnTopLefts;
    public List<Transform> spawnBottomRights;

    [Header("Don't adjust the values below in the Inspector")]

    public List<Transform> enemyTransforms = new List<Transform>();
    public bool enemyDeath = false;
    public List <Transform> tfsToRemove = new List<Transform>(); // List for enemy transforms to remove as the enemies have been killed

    public static EnemyManager emInstance; // Static instance for easy references

    void Awake()
    {
        emInstance = this;
    }

    void Start()
    {
        enemyTransforms = new List<Transform>();

        // Spawning enemies for the adjusted amount
        for(int i = 0; i < enemyAmount; i++)
        {
            SpawnEnemy();
        }

        // Pre-spawning as many corpses as there are enemies
        CorpseManager.cmInstance.PreSpawnCorpses(enemyTransforms.Count);
    }

    // Spawning an enemy in a random position in one of the allocated spawn areas
    private void SpawnEnemy()
    {
        int spawnIndex = Random.Range(0, spawnTopLefts.Count-1);      

        float leftLimit = spawnTopLefts[spawnIndex].position.x;
        float rightLimit = spawnBottomRights[spawnIndex].position.x;
        float topLimit = spawnTopLefts[spawnIndex].position.y;
        float bottomLimit = spawnBottomRights[spawnIndex].position.y;

        Vector2 enemyPos = new Vector2(Random.Range(leftLimit, rightLimit), Random.Range(bottomLimit, topLimit));
        spawnTF.position = enemyPos;

        int enemyTypeRoll = Random.Range(0,10);
        GameObject spawnedEnemy;
        if(enemyTypeRoll < 2)
        {
            spawnedEnemy = Instantiate(enemyShooterPF, spawnTF);
        }
        else
        {
            spawnedEnemy = Instantiate(enemyFollowerPF, spawnTF);
            // Making 10% of Follower enemies aggressive
            int rolledChance = Random.Range(0,10);
            if(rolledChance < 2)
            {
                spawnedEnemy.GetComponent<Follower>().MakeAggressive();
            }
        }
        spawnedEnemy.transform.parent = null;
        // Making the enemies more "unique" with the counter to their names
        spawnedEnemy.transform.name += counter;
        counter++;
        // Populating the enemy Transform list one at a time
        //StaticManager.enemyTransforms.Add(spawnedEnemy.transform);
        enemyTransforms.Add(spawnedEnemy.transform);
    }

    // Called by PlayerProjectile each time it manages to hit an enemy
    public void CheckIfEnemiesDead()
    {
        tfsToRemove = new List<Transform>();
        enemyDeath = false;
        
        for(int i = 0; i < enemyTransforms.Count; i++)
        {
            if(enemyTransforms[i].gameObject.GetComponent<Enemy>().currHP == 0)
            {
                tfsToRemove.Add(enemyTransforms[i]);
                CorpseManager.cmInstance.NewCorpse(enemyTransforms[i].position);
                enemyDeath = true;
            }
        }

        // In case we notice that at least 1 enemy has died
        if(enemyDeath)
        {
            // We remove and destroy the Transforms and Gameobjects indicated by StaticManager
            for(int j = 0; j < tfsToRemove.Count; j++)
            {
                enemyTransforms.Remove(tfsToRemove[j]);
                Destroy(tfsToRemove[j].gameObject);
            }

            // If we've killed all enemies, the level is done!
            if(enemyTransforms.Count == 0)
            {
                GameplayManager.gpManagerInstance.PassLevel();
            }

            // Telling the Indicator to check if there are only a few enemies
            IndicatorClosest.icInstance.CheckIfFewEnemies();
        }

    }

}
