using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;

        if (Input.GetKey(KeyCode.A))
        {
            position.x += -panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            position.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            position.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            position.z += -panSpeed * Time.deltaTime;
        }

        transform.position = position;
    }
}
