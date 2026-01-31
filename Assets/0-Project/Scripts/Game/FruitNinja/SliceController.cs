using UnityEngine;

public class SliceController : MonoBehaviour
{
    [Header("Slice Settings")]
    [SerializeField] private float minSliceVelocity = 0.1f;
    [SerializeField] private float sliceZPosition = 0f;
    
    [Header("Visual Settings")]
    [SerializeField] private TrailRenderer sliceTrail;
    [SerializeField] private float trailTime = 0.3f;
    [SerializeField] private float trailWidth = 0.15f;
    [SerializeField] private Material trailMaterial;
    
    [Header("Collision Settings")]
    [SerializeField] private float sliceRadius = 0.5f;
    [SerializeField] private LayerMask sliceableLayer;
    
    private Camera mainCamera;
    private bool isSlicing = false;
    private Vector3 lastMousePosition;
    
    void Awake()
    {
        mainCamera = Camera.main;
        SetupTrailRenderer();
    }
    
    void SetupTrailRenderer()
    {
        if (sliceTrail == null)
        {
            GameObject trailObj = new GameObject("SliceTrail");
            trailObj.transform.SetParent(transform);
            sliceTrail = trailObj.AddComponent<TrailRenderer>();
        }
        
        // TrailRenderer settings
        sliceTrail.time = trailTime; // Auto fade-out time
        sliceTrail.startWidth = trailWidth;
        sliceTrail.endWidth = trailWidth * 0.5f;
        sliceTrail.minVertexDistance = 0.05f;
        sliceTrail.autodestruct = false;
        sliceTrail.emitting = false;
        sliceTrail.sortingOrder = 100;
        
        if (trailMaterial != null)
        {
            sliceTrail.material = trailMaterial;
        }
        else
        {
            // Create simple white material with fade
            sliceTrail.material = new Material(Shader.Find("Sprites/Default"));
            sliceTrail.startColor = Color.white;
            sliceTrail.endColor = new Color(1f, 1f, 1f, 0f); // Fade to transparent
        }
    }
    
    void Update()
    {
        HandleSliceInput();
    }
    
    void HandleSliceInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartSlice();
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueSlice();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndSlice();
        }
    }
    
    void StartSlice()
    {
        isSlicing = true;
        sliceTrail.Clear();
        sliceTrail.emitting = true;
        
        Vector3 mousePos = GetMouseWorldPosition();
        sliceTrail.transform.position = mousePos;
        lastMousePosition = mousePos;
    }
    
    void ContinueSlice()
    {
        if (!isSlicing) return;
        
        Vector3 mousePos = GetMouseWorldPosition();
        sliceTrail.transform.position = mousePos;
        
        // Check for sliced objects
        if (Vector3.Distance(mousePos, lastMousePosition) > minSliceVelocity)
        {
            CheckForSlicedObjects(lastMousePosition, mousePos);
            lastMousePosition = mousePos;
        }
    }
    
    void EndSlice()
    {
        isSlicing = false;
        sliceTrail.emitting = false;
        // Trail will auto fade-out based on trailTime
    }
    
    void CheckForSlicedObjects(Vector3 startPoint, Vector3 endPoint)
    {
        // Calculate slice direction
        Vector2 sliceDirection = (endPoint - startPoint).normalized;
        Vector2 slicePoint = (startPoint + endPoint) / 2f;
        
        // Check for objects along the slice path
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            startPoint,
            sliceRadius,
            sliceDirection,
            Vector2.Distance(startPoint, endPoint),
            sliceableLayer
        );
        
        foreach (RaycastHit2D hit in hits)
        {
            SliceableObject sliceable = hit.collider.GetComponent<SliceableObject>();
            if (sliceable != null)
            {
                sliceable.OnSliced(sliceDirection, hit.point);
            }
        }
    }
    
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z - sliceZPosition);
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
    
    void OnDrawGizmos()
    {
        // Visualize slice radius in editor
        if (isSlicing)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastMousePosition, sliceRadius);
        }
    }
}
