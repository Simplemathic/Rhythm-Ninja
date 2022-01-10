using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextBoing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(1 + (float)Math.Sin(3 * Time.realtimeSinceStartup) / 6, 1 + (float)Math.Sin(3 * Time.realtimeSinceStartup) / 6, 1);
    }
}
