using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextCameraLook : MonoBehaviour
{
    private Transform camera;
    public float damping = 40f;
    void Start()
    {
        camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        var lookPos = camera.position - transform.position;
        //lookPos.y = 0;
        var rotation = Quaternion.LookRotation(-lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }
}
