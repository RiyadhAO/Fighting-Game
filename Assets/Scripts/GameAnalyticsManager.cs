using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class GameAnalyticsManager : MonoBehaviour
{
    public static GameAnalyticsManager Instance;

    private float sessionStartTime;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Analytics initialization failed: " + e.Message);
        }

        sessionStartTime = Time.time;
    }

    private void OnApplicationQuit()
    {
        TrackPlaytime();
    }

    //Public Analytics Methods

    public void TrackMatchResult(string characterUsed, bool didWin)
    {
        var customEvent = new CustomEvent("match_result");
        customEvent["character"] = characterUsed;
        customEvent["result"] = didWin ? "win" : "loss";

        AnalyticsService.Instance.RecordEvent(customEvent);
    }

    public void TrackMechanicUse(string mechanicName, string character)
    {
        var customEvent = new CustomEvent("mechanic_used");
        customEvent["mechanic"] = mechanicName;
        customEvent["character"] = character;

        AnalyticsService.Instance.RecordEvent(customEvent);
    }

    public void TrackGameModeStart(string modeName)
    {
        var customEvent = new CustomEvent("game_mode_started");
        customEvent["mode"] = modeName;

        AnalyticsService.Instance.RecordEvent(customEvent);
    }

    public void TrackPlaytime()
    {
        float playTimeSeconds = Time.time - sessionStartTime;

        var customEvent = new CustomEvent("session_playtime");
        customEvent["duration_seconds"] = playTimeSeconds;

        AnalyticsService.Instance.RecordEvent(customEvent);
    }
}
