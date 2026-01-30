using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHand : MonoBehaviour
{
    void Update()
    {
        transform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            transform.DOScale(transform.localScale * .8f, .1f);
            transform.DORotate(new Vector3(0, 0, 30), .1f);
        }
        if (Input.GetMouseButtonUp(0))
        {
            transform.DOScale(Vector3.one, .1f);
            transform.DORotate(Vector3.zero, .1f);
        }
    }
}