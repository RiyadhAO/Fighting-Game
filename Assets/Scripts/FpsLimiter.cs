using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsLimiter : MonoBehaviour
{
    public TMP_Text fpsText;
    private float deltaTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();
    }
}
