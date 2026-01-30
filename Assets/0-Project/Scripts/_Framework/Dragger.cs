using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragger : MonoBehaviour
{
    public float yOffsetForDraggedObject = .1f;

    Plane plane;
    float distance;

    public float maxDragDistance = 1.2f;

    public Camera fixedCameraForRaycast; //URP OVERLAY CAMERA

    private void Start()
    {
        plane = new Plane(Vector3.forward, new Vector3(0, /*yOffsetForDraggedObject*/ GameManager.Instance.player.transform.position.y, 0));
    }

    Vector3 firstTouchPos;
    Vector3 firstPlayerPos;
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && (GameManager.Instance._gameState != GameManager.GameState.GameOver && GameManager.Instance._gameState != GameManager.GameState.Win))
        {
            Ray ray = fixedCameraForRaycast.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (GameManager.Instance._gameState == GameManager.GameState.NotStarted)
                        UIManager.Instance.PlayArea();

                    firstTouchPos = ray.GetPoint(distance);
                    firstPlayerPos = GameManager.Instance.player.transform.position;

                    MMVibrationManager.Haptic(HapticTypes.Selection);
                }
                if (Input.GetMouseButton(0))
                {
                    Vector3 movementVector = ray.GetPoint(distance) - firstTouchPos;

                    //around circle
                    if (Vector3.Distance(firstTouchPos, ray.GetPoint(distance)) > maxDragDistance)
                        movementVector = movementVector.normalized * maxDragDistance;

                    Vector3 finalPos = firstPlayerPos + movementVector;
                    GameManager.Instance.player.transform.position = finalPos;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    MMVibrationManager.Haptic(HapticTypes.LightImpact);
                }
            }
        }
    }



    #region Draggable Object

    //  public bool movement = false;
    //  private void OnMouseDown()
    //  {
    //      MMVibrationManager.Haptic(HapticTypes.Selection);
    //
    //      movement = true;
    //      //plane = new Plane(Vector3.up, new Vector3(0, yOffsetForDraggedObject /*transform.position.y*/, 0));
    //  }
    //
    //  void OnMouseDrag()
    //  {
    //      if (movement)
    //      {
    //          Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //          if (plane.Raycast(ray, out distance))
    //          {
    //                  transform.position = ray.GetPoint(distance);
    //          }
    //      }
    //  }

    #endregion

}