using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour
{
    public bool wasPushed = false;
    public bool hasTakenSpikeDamage = false;

    public void RegisterPush()
    {
        wasPushed = true;
        hasTakenSpikeDamage = false; // Reset spike damage flag on new push
        StartCoroutine(ResetPush());
    }

    private IEnumerator ResetPush()
    {
        yield return new WaitForSeconds(0.5f); // Adjust timing as needed
        wasPushed = false;
        hasTakenSpikeDamage = false;
    }
}

