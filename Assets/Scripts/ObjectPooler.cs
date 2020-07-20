/*
In this script:
- Pool setup for object pooling
- Method to spawn bullets
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class ObjPool
    {
        public string name;
        public GameObject prefab;
        public int size;
    }

    public List<ObjPool> objectPools;

    public Dictionary<string, Queue<GameObject>> objPoolDict;

    // Setting up the script with a static reference for easy access
    public static ObjectPooler opInstance;

    void Awake()
    {
        opInstance = this;
    }

    void Start()
    {
        objPoolDict = new Dictionary<string, Queue<GameObject>>();

        // Creating object queues for each pool and filling them with instantiation
        for(int i = 0; i < objectPools.Count; i++)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int j = 0; j < objectPools[i].size; j++)
            {
                GameObject poolObject = Instantiate(objectPools[i].prefab);
                poolObject.SetActive(false);
                objectPool.Enqueue(poolObject);
            }

            objPoolDict.Add(objectPools[i].name, objectPool);
        }
    }

    public void SpawnBullet(Vector2 spawnPosition, Quaternion spawnRotation)
    {
        if(objPoolDict.ContainsKey("bullet"))
        {
            GameObject spawnedObject = objPoolDict["bullet"].Dequeue();

            spawnedObject.transform.localPosition = spawnPosition;
            spawnedObject.transform.rotation = spawnRotation;
            spawnedObject.SetActive(true);

            objPoolDict["bullet"].Enqueue(spawnedObject);
        }

    }

}