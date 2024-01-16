using GameAnalyticsSDK;
using UnityEngine;


public class GameAnalyticsScr : MonoBehaviour
{
    private void Awake()
    {
        GameAnalytics.Initialize();
    }
}