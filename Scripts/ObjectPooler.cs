using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public InputHandler.Input inputType;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<InputHandler.Input, Queue<GameObject>> poolDictionary;

    #region Singleton
    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    void Start()
    {
        poolDictionary = new Dictionary<InputHandler.Input, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.inputType, objectPool);
        }
    }

    public GameObject SpawnFromPool(Note note, float spawnDistance)
    {
        if (!poolDictionary.ContainsKey(note.key))
        {
            Debug.LogWarning("Pool with tag \"" + note.key + "\" doesn't exist!");
            return null;
        }

        float horizontalPos = LevelInformation.centerHorizontalPosition;
        switch (note.positionInSpace)
        {
            case Note.Position.LEFT:
                horizontalPos -= LevelInformation.wallDistanceFromCenter;
                break;
            case Note.Position.RIGHT:
                horizontalPos += LevelInformation.wallDistanceFromCenter;
                break;
            case Note.Position.MIDDLE:
            default:
                break;
        }

        GameObject objectToSpawn = poolDictionary[note.key].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = new Vector3(horizontalPos, spawnDistance, 0);
        objectToSpawn.transform.rotation = Quaternion.identity;
        poolDictionary[note.key].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

}
