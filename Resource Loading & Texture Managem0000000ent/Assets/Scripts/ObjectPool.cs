// ObjectPool.cs
// PURPOSE: Reuse frequently spawned objects instead of creating
// and destroying them repeatedly. This reduces memory allocation
// and improves performance.

using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Prefab to pool (bullet, enemy, power-up, etc.)
    public GameObject prefab;

    // How many objects to create at start
    public int poolSize = 10;

    // List to store pooled objects
    private List<GameObject> pool = new List<GameObject>();

    void Start()
    {
        // Pre-allocate objects at startup
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false); // Disable until needed
            pool.Add(obj);
        }

        Debug.Log($"ObjectPool initialized with {poolSize} objects.");
    }

    // Get an object from the pool
    public GameObject GetObject()
    {
        foreach (GameObject obj in pool)
        {
            // Reuse inactive object
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Optional: Expand pool if needed
        GameObject newObj = Instantiate(prefab);
        newObj.SetActive(true);
        pool.Add(newObj);
        Debug.Log("Pool expanded.");

        return newObj;
    }

    // Return object to the pool
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}