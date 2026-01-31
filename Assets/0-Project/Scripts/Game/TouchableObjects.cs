using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TouchableObjects : MonoBehaviour
{
    public Camera fakeCamera;
    //CameraConfig
    private Vector3 newCameraPosition;
    private float ortographicSize = 5f;

    public Transform targetTransform;

    void Start()
    {
        newCameraPosition = fakeCamera.transform.position;
        ortographicSize = fakeCamera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetMouseButtonDown(0) && GameManager.Instance.gameState == GameManager.GameState.Free)
	    {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.transform.CompareTag("Touchable"))
                {
                    Debug.Log(hit.transform.gameObject.name);

                    GameManager.Instance.nextCameraPosition = newCameraPosition;
                    GameManager.Instance.nextOrtographicSize = ortographicSize;

                    GameManager.Instance.lastTargetTransformForInspector = targetTransform;
                    InspectorNavMesh.Instance.navMeshAgent.SetDestination(targetTransform.position);

                    GameManager.Instance.gameState = GameManager.GameState.MoveToTarget;

                }
            }
        }
    }

}
