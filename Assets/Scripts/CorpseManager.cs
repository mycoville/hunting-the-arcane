/*
In this script:
- Enemy corpse management
- Pre-spawning them at scene start and using this list to spawn the sprites where necessary
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseManager : MonoBehaviour
{
    public static CorpseManager cmInstance;
    public GameObject corpsePrefab;
    public Transform corpseSpawnTF;

    private static List<GameObject> corpseList;

    private static int currentCorpseId = 0;

    void Awake()
    {
        cmInstance = this;
        currentCorpseId = 0;
    }

    // Called by EnemyManager after spawning the actual enemies
    public void PreSpawnCorpses(int corpseAmount)
    {
        corpseList = new List<GameObject>();
        for(int i = 0; i < corpseAmount; i++)
        {
            GameObject corpseObj = Instantiate(corpsePrefab, corpseSpawnTF);
            corpseObj.transform.parent = null;
            corpseObj.SetActive(false);
            corpseList.Add(corpseObj);
        }
    }

    // Called whenever an enemy dies
    public void NewCorpse(Vector2 newPosition)
    {
        corpseList[currentCorpseId].transform.position = newPosition;
        corpseList[currentCorpseId].SetActive(true);
        currentCorpseId++;
    }
}
