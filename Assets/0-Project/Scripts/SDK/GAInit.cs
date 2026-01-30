using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class GAInit : MonoBehaviour
{
    private void Awake()
    {
        GameAnalytics.Initialize();
    }
}
