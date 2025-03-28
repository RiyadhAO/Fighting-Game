using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CountdownTimer : MonoBehaviour
{
    public int Time;
    public TMP_Text CountdownText;
    public StopwatchCountdown stopwatch;
    public PlayerInput p1;
    public PlayerInput p2;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        p1.enabled = false;
        p2.enabled = false;

        while (Time > 0)
        {

            CountdownText.text = Time.ToString();

            yield return new WaitForSeconds(1f);

            Time--;
        }

        CountdownText.text = "Fight!";

        yield return new WaitForSeconds(1f);

        stopwatch.ResumeTimer();

        p1.enabled = true;
        p2.enabled = true;

        CountdownText.gameObject.SetActive(false);
    }
}
