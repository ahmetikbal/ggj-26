using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RunnerController : MonoBehaviour
{
    public Rigidbody rb;

    public Camera fixedCamera;

    public float movementSpeed = 9f;
    public float slidingSpeed = 15f;

    private void FixedUpdate()
    {
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, movementSpeed * 0.8f), Time.fixedDeltaTime * 10f);
    }

    Vector3 firstTouchPos;
    Vector3 firstPlayerPos;

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && (GameManager.Instance._gameState != GameManager.GameState.GameOver && GameManager.Instance._gameState != GameManager.GameState.Win))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (GameManager.Instance._gameState == GameManager.GameState.NotStarted)
                    UIManager.Instance.PlayArea();

                firstTouchPos = fixedCamera.ScreenToWorldPoint(Input.mousePosition - new Vector3(0, 0, 1));
                firstPlayerPos = transform.localPosition;

                MMVibrationManager.Haptic(HapticTypes.Selection);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 movementVector = fixedCamera.ScreenToWorldPoint(Input.mousePosition - new Vector3(0, 0, 1)) - firstTouchPos;

                float finalXPos = firstPlayerPos.x - movementVector.x * slidingSpeed;
                finalXPos = Mathf.Clamp(finalXPos, -2.2f, 2.2f);

                rb.position = Vector3.Lerp(rb.position, new Vector3(finalXPos, rb.position.y, rb.position.z), Time.fixedDeltaTime * 10f);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
            }

        }
    }
}