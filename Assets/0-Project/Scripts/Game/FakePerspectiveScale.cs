using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePerspectiveScale : MonoBehaviour
{
    public float frontY = -16.0f;
    public float backY = 12.0f;

    public float frontScale = 1.0f;
    public float backScale = 0.7f;

    void Update()
    {
        float y = transform.position.y;

        // Y'yi 0–1 aralýðýna çevir
        float t = Mathf.InverseLerp(frontY, backY, y);

        // Scale'i buna göre hesapla
        float scale = Mathf.Lerp(frontScale, backScale, t);

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
