using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class PapaganClicker : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Ease moveEase = Ease.OutQuad;

    // Start is called before the first frame update
    void Start()
    {
        //transform.DOMoveY(2, 1).SetRelative().SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
        
        // Stop any current movement before starting new one
        transform.DOKill();
        
        // Move to target position smoothly
        transform.DOMove(targetPos, moveDuration).SetEase(moveEase);
    }
}
