using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMover : MonoBehaviour
{
    public const double speed = 70.0;
    private double lifeStartTime;
    public double lifeSpan = 2.0;
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        lifeStartTime = Time.realtimeSinceStartup;
        if (lifeSpan == 0) lifeSpan = 0.1;
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        double currentLifeTime = Time.realtimeSinceStartup - lifeStartTime;

        text.color = new Color(
            text.color.r, 
            text.color.g, 
            text.color.b, 
            text.color.a <= 0 ? 0 : text.color.a - Time.deltaTime * 0.5f
        );

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + (float)speed * Time.deltaTime,
            transform.position.z
        );
    }
}
