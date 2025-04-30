using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine;

public class AnalyticsInitializer : MonoBehaviour
{
    async void Awake()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }
}
