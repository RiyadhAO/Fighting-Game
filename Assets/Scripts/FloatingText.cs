using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1.5f;
    public float fadeDuration = 1f;
    private TextMeshPro textMesh;
    private Color textColor;

    void Start()
    {
        textMesh = GetComponentInChildren<TextMeshPro>();
        if (textMesh != null)
        {
            textColor = textMesh.color;
        }
        Destroy(gameObject, fadeDuration); // Automatically destroy after fading out
    }

    void Update()
    {
        // Move the text upwards
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Gradually fade out
        if (textMesh != null)
        {
            textColor.a -= Time.deltaTime / fadeDuration;
            textMesh.color = textColor;
        }
    }
}
