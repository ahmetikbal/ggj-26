using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class SliceableObject : MonoBehaviour
{
    public enum ObjectType
    {
        Fruit,
        Bomb
    }
    
    [SerializeField] [EnumToggleButtons] private ObjectType objectType = ObjectType.Fruit;
    [SerializeField] private GameObject sliceEffectPrefab;
    [SerializeField] private float slicedPieceLifetime = 2f;
    
    [Header("Bomb Effect")]
    [SerializeField] private Sprite bombSplashSprite;
    [SerializeField] private float bombSplashDuration = 3.5f;
    [SerializeField] private Vector3 bombSplashSize = new Vector3(2f, 2f, 1f);
    
    [Header("Drag Settings")]
    [SerializeField] private float upwardDrag = 0.8f;
    [SerializeField] private float downwardDrag = 0f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private FruitNinjaManager gameManager;
    private bool hasBeenSliced = false;
    
    // Bounds for despawning
    private float despawnYPosition = -30f;
    private float maxReachedY = float.MinValue;
    private bool hasReachedTop = false;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void Initialize(FruitNinjaManager manager)
    {
        gameManager = manager;
        hasBeenSliced = false;
        maxReachedY = transform.position.y;
        hasReachedTop = false;
    }
    
    void Update()
    {
        // Track highest point reached
        if (transform.position.y > maxReachedY)
        {
            maxReachedY = transform.position.y;
            hasReachedTop = true;
        }
        
        // Dynamic drag: high drag while going up, low drag while falling
        if (rb != null)
        {
            if (rb.velocity.y > 0)
            {
                // Going up - apply drag to make it float
                rb.drag = upwardDrag;
            }
            else
            {
                // Falling down - remove drag for faster fall
                rb.drag = downwardDrag;
                rb.mass = 1f;
                rb.gravityScale = 3f;
            }
        }
        
        // Despawn only if object has reached top and is now falling below screen
        if (hasReachedTop && transform.position.y < despawnYPosition)
        {
            OnMissed();
        }
    }
    
    public void OnSliced(Vector2 sliceDirection, Vector2 slicePoint)
    {
        if (hasBeenSliced) return;
        hasBeenSliced = true;
        
        if (objectType == ObjectType.Fruit)
        {
            gameManager?.OnFruitSliced();
            CreateSlicedPieces(sliceDirection, slicePoint);
        }
        else if (objectType == ObjectType.Bomb)
        {
            gameManager?.OnBombSliced();
            CreateExplosionEffect();
        }
        
        // Destroy or return to pool
        Destroy(gameObject);
    }
    
    private void CreateSlicedPieces(Vector2 sliceDirection, Vector2 slicePoint)
    {
        // Calculate perpendicular direction for separation
        Vector2 perpendicular = new Vector2(-sliceDirection.y, sliceDirection.x).normalized;
        
        // Create two sliced pieces
        CreateSlicedPiece(perpendicular, slicePoint);
        CreateSlicedPiece(-perpendicular, slicePoint);
        
        // Play slice effect
        if (sliceEffectPrefab != null)
        {
            GameObject effect = Instantiate(sliceEffectPrefab, slicePoint, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }
    
    private void CreateSlicedPiece(Vector2 direction, Vector2 slicePoint)
    {
        // Create a copy of this object for the sliced piece
        GameObject piece = new GameObject("SlicedPiece");
        piece.transform.position = transform.position;
        piece.transform.localScale = transform.localScale;
        
        // Add sprite renderer
        SpriteRenderer pieceRenderer = piece.AddComponent<SpriteRenderer>();
        pieceRenderer.sprite = spriteRenderer.sprite;
        pieceRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        pieceRenderer.sortingOrder = spriteRenderer.sortingOrder;
        
        // Add rigidbody for physics
        Rigidbody2D pieceRb = piece.AddComponent<Rigidbody2D>();
        pieceRb.gravityScale = rb.gravityScale;
        pieceRb.mass = rb.mass;
        
        // Apply force in the slice direction
        Vector2 force = direction * Random.Range(3f, 6f) + Vector2.up * Random.Range(1f, 3f);
        pieceRb.AddForce(force, ForceMode2D.Impulse);
        
        // Add rotation
        pieceRb.angularVelocity = Random.Range(-360f, 360f);
        
        // Fade out and destroy
        pieceRenderer.DOFade(0f, slicedPieceLifetime).SetEase(Ease.InQuad);
        Destroy(piece, slicedPieceLifetime);
        
        // Add simple masking effect
        // For better visual, we should use a shader or mesh slicing
        // But for now, we'll scale pieces to simulate slicing
        float maskDirection = direction.x > 0 ? 1 : -1;
        piece.transform.localScale = new Vector3(
            piece.transform.localScale.x * 0.5f,
            piece.transform.localScale.y,
            piece.transform.localScale.z
        );
        piece.transform.position += new Vector3(maskDirection * 0.3f, 0, 0);
    }
    
    private void CreateExplosionEffect()
    {
        // Create bomb splash sprite effect
        if (bombSplashSprite != null)
        {
            GameObject splashObj = new GameObject("BombSplash");
            splashObj.transform.position = transform.position;
            splashObj.transform.localScale = bombSplashSize;
            
            SpriteRenderer splashRenderer = splashObj.AddComponent<SpriteRenderer>();
            splashRenderer.sprite = bombSplashSprite;
            splashRenderer.sortingOrder = -1; // Behind other objects
            
            // Fade out over time
            Color color = splashRenderer.color;
            splashRenderer.DOFade(0f, bombSplashDuration).SetEase(Ease.InQuad);
            
            Destroy(splashObj, bombSplashDuration);
        }
        
        // Particle effect (optional)
        if (sliceEffectPrefab != null)
        {
            GameObject effect = Instantiate(sliceEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }
    
    private void OnMissed()
    {
        if (hasBeenSliced) return;
        
        if (objectType == ObjectType.Fruit)
        {
            gameManager?.OnObjectMissed();
        }
        
        Destroy(gameObject);
    }
    
    public ObjectType GetObjectType() => objectType;
    public void SetObjectType(ObjectType type) => objectType = type;
}
