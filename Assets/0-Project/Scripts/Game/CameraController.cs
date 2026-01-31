using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    private Transform initialPosition;
    private float initialOrthographicSize;
    public Transform targetPosition;
    public float targetOrthographicSize;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform;
        initialOrthographicSize = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
