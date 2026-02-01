using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchableController : MonoBehaviour
{
    public static TouchableController Instance { get; private set; }

    public List<TouchableObjects> touchables;
    
    // Şu an hedeflenen touchable (diyalog için)
    private TouchableObjects currentTarget;

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
        touchables = new List<TouchableObjects>();
        touchables.AddRange(FindObjectsOfType<TouchableObjects>());
        touchables.ForEach(t => t.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("OUTBASE_ON"));
 
    }


    void Update()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if(hit.collider != null && hit.transform.CompareTag("Touchable"))
        {
            TouchableObjects touchable = hit.transform.GetComponent<TouchableObjects>();
            
            // Sadece müsait karakterleri highlight yap
            if (touchable != null && touchable.CanInteract())
            {
                hit.collider.transform.GetChild(0).GetComponent<Renderer>().material.EnableKeyword("OUTBASE_ON"); 

                if (Input.GetMouseButtonDown(0) && GameManager.Instance.gameState == GameManager.GameState.Free)
                {
                    Debug.Log(hit.transform.gameObject.name);

                    currentTarget = touchable;
                    GameManager.Instance.nextCameraPosition = touchable.newCameraPosition;
                    GameManager.Instance.nextOrtographicSize = touchable.ortographicSize;

                    GameManager.Instance.lastTargetTransformForInspector = touchable.targetTransform;
                    InspectorNavMesh.Instance.navMeshAgent.SetDestination(touchable.targetTransform.position);

                    GameManager.Instance.gameState = GameManager.GameState.MoveToTarget;
                }
            }
            else
            {
                // Müsait olmayan karakter - highlight yapma
                hit.collider.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("OUTBASE_ON");
            }
        }
        else
        {
            touchables.ForEach(t => t.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("OUTBASE_ON"));
        }
    }
    
    /// <summary>
    /// Inspector hedefe ulaştığında GameManager tarafından çağrılır
    /// </summary>
    public void OnInspectorReachedTarget()
    {
        if (currentTarget != null && currentTarget.dialoguePanel != null)
        {
            currentTarget.StartDialogue();
            currentTarget = null;
        }
    }
    
    /// <summary>
    /// Belirli bir karakterin TouchableObjects'ini döndürür
    /// </summary>
    public TouchableObjects GetTouchableByCharacter(CharacterType characterType)
    {
        return touchables.Find(t => t.characterType == characterType);
    }
    
    /// <summary>
    /// Bir karakteri zorla tıklatır (zorunlu geçişler için)
    /// </summary>
    public void ForceInteract(CharacterType characterType)
    {
        var touchable = GetTouchableByCharacter(characterType);
        if (touchable != null)
        {
            currentTarget = touchable;
            GameManager.Instance.nextCameraPosition = touchable.newCameraPosition;
            GameManager.Instance.nextOrtographicSize = touchable.ortographicSize;
            
            GameManager.Instance.lastTargetTransformForInspector = touchable.targetTransform;
            InspectorNavMesh.Instance.navMeshAgent.SetDestination(touchable.targetTransform.position);
            
            GameManager.Instance.gameState = GameManager.GameState.MoveToTarget;
        }
    }
}
