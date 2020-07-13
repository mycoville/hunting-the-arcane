/*
In this script:
- Enemy spawning, spawn positions
- Enemy destruction based on StaticManager's values
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    
    public Enemy enemyPF; // Enemy prefab reference
    public Transform spawnTF;
    public int enemyAmount = 10;
    public GameObject levelPassText;

    private int counter = 0;

    // Minimalistic spawn area adjustment: Lists for spawn area corners (top left, bottom right) allocating a rectangular spawn area.
    public List<Transform> spawnTopLefts;
    public List<Transform> spawnBottomRights;

    void Start()
    {
        StaticManager.enemyTransforms = new List<Transform>();

        // Spawning enemies for the adjusted amount
        for(int i = 0; i < enemyAmount; i++)
        {
            SpawnEnemy();
        }

        levelPassText.SetActive(false);
    }

    void FixedUpdate()
    {
        // In case there are changed is the overall enemy Transform list
        if(StaticManager.listChanged)
        {
            // We remove and destroy the Transforms and Gameobjects indicated by StaticManager
            for(int j = 0; j < StaticManager.tfsToRemove.Count; j++)
            {
                StaticManager.enemyTransforms.Remove(StaticManager.tfsToRemove[j]);
                Destroy(StaticManager.tfsToRemove[j].gameObject);
            }

            StaticManager.listChanged = false;

            // If we've killed all enemies, the level is done!
            if(StaticManager.enemyTransforms.Count == 0)
            {
                levelPassText.SetActive(true);
            }
        }
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
        var spawnedEnemy = Instantiate(enemyPF, spawnTF);
        spawnedEnemy.transform.parent = null;
        // Making the enemies more "unique" with the counter to their names
        spawnedEnemy.transform.name += counter;
        counter++;
        // Populating the enemy Transform list one at a time
        StaticManager.enemyTransforms.Add(spawnedEnemy.transform);

        // Making random enemies aggressive
        int rolledChance = Random.Range(0,10);
        if(rolledChance < 2)
        {
            spawnedEnemy.GetComponent<Follower>().MakeAggressive();
        }
        
    }

}
