using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    void Update()
    {
        double dt = LevelInformation.dt;
        double speed = LevelInformation.defaultSpeed;
        transform.position = new Vector3(
            transform.position.x, 
            transform.position.y - (float)(speed * dt), 
            transform.position.z
        );
    }
}
