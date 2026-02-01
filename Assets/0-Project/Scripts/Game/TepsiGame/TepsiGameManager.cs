using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TepsiGameManager : MonoBehaviour
{
    public static TepsiGameManager Instance { get; private set; }

    public bool table1Completed, table2Completed, table3Completed, table4Completed;
    
    private bool gameCompleted = false;

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
        ResetGame();
    }

    public void SetAllTablesTrue()
    {
        table1Completed = true;
        table2Completed = true;
        table3Completed = true;
        table4Completed = true;
    }
    
    public void ResetGame()
    {
        table1Completed = false;
        table2Completed = false;
        table3Completed = false;
        table4Completed = false;
        gameCompleted = false;
    }

    void Update()
    {
        // Oyun zaten tamamlandıysa tekrar çağırma
        if (gameCompleted) return;
        
        if (table1Completed && table2Completed && table3Completed && table4Completed)
        {
            gameCompleted = true;
            Debug.Log("[TepsiGameManager] All tables completed! Calling OnTableCleanComplete...");
            DialogueActionHandler.Instance.OnTableCleanComplete();
        }
    }
}
