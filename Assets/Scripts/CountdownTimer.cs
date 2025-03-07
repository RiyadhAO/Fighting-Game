using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public int Time;
    public TMP_Text CountdownText;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        while (Time > 0)
        {
            CountdownText.text = Time.ToString();

            yield return new WaitForSeconds(1f);

            Time--;
        }

        CountdownText.text = "Fight!";

        yield return new WaitForSeconds(1f);

        CountdownText.gameObject.SetActive(false);
    }
}
