using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InspectorNavMesh : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform navMeshDestination;

    void Start()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

     // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            navMeshAgent.ResetPath();
            MoveToMousePosition();
        }
    }

    private void MoveToMousePosition()
    {
        // Calculate distance from camera to object to maintain depth
        float distanceToScreen = Camera.main.WorldToScreenPoint(transform.position).z;
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = distanceToScreen;
        
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);
        
        navMeshAgent.destination = targetPos;

        if(targetPos.x > transform.position.x)
        {
            transform.localScale = new Vector3(10.9094f, 10.9094f, 10.9094f);
        }
        else if(targetPos.x < transform.position.x)
        {
            transform.localScale = new Vector3(-10.9094f, 10.9094f, 10.9094f);
        }
    }
}
