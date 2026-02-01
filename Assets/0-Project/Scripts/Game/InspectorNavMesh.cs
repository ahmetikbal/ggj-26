using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InspectorNavMesh : MonoBehaviour
{
    public static InspectorNavMesh Instance { get; private set; }

    
    public NavMeshAgent navMeshAgent;
    public Transform navMeshDestination;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

     // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.gameState == GameManager.GameState.Free)
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
            SetXScale(10.9094f);
        }
        else if(targetPos.x < transform.position.x)
        {
            SetXScale(-10.9094f);
        }
    }

    public void SetXScale(float targetPosX)
    {
        transform.localScale = new Vector3(targetPosX, transform.localScale.y, transform.localScale.z);
    }
}
