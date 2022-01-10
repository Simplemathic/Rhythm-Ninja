using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPooler : MonoBehaviour
{
    public Canvas canvas;
    public int poolSize = 10;
    Queue<GameObject> objectPool;

    #region Singleton
    public static TextPooler Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    void Start()
    {
        objectPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject();
            Text text = obj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            obj.AddComponent<TextMover>();
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    public GameObject SpawnFromPool(string text, Vector3 color, Vector3 position, Quaternion rotation)
    {
        GameObject objectToSpawn = objectPool.Dequeue();
        objectToSpawn.transform.SetParent(canvas.transform);
        Text goText = objectToSpawn.GetComponent<Text>();
        goText.text = text;
        goText.color = new Color(color.x, color.y, color.z, 1);
        goText.rectTransform.position = new Vector3(
            canvas.GetComponent<RectTransform>().rect.width  * canvas.GetComponent<RectTransform>().localScale.x / 2,
            canvas.GetComponent<RectTransform>().rect.height * canvas.GetComponent<RectTransform>().localScale.y / 2,
            0);
        objectToSpawn.SetActive(true);
        goText.transform.SetParent(canvas.transform);
        //objectToSpawn.transform.position = position;
        //objectToSpawn.transform.rotation = rotation;
        objectPool.Enqueue(objectToSpawn);
        return objectToSpawn;
    }

}
