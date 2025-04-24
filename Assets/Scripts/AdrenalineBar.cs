using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdrenalineBar : MonoBehaviour
{
    public event System.Action OnAdrenalineEmpty;

    [Header("Settings")]
    [SerializeField] private float maxAdrenaline = 100f;
    [SerializeField] private float regenerationRate = 0.6f;
    [SerializeField] private Vector2 activationBoostRange = new Vector2(15f, 35f);

    [Header("UI References")]
    [SerializeField] private Slider adrenalineSlider;
    [SerializeField] private GameObject activationPrompt;
    [SerializeField] private TextMeshProUGUI adrenalinePercentageText;
    [SerializeField] private TextMeshProUGUI adrenalineCheckText;

    public float CurrentAdrenaline { get; private set; } = 0f;
    private bool isActivated = false;
    private bool isDraining = false;
    private float drainRate = 0f;

    private void Start()
    {
        InitializeUI();
        adrenalineCheckText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDraining)
        {
            adrenalineCheckText.gameObject.SetActive(false);

            CurrentAdrenaline -= drainRate * Time.deltaTime;
            if (CurrentAdrenaline <= 0f)
            {
                CurrentAdrenaline = 0f;
                isDraining = false;
                OnAdrenalineEmpty?.Invoke();
            }
            UpdateUI();
        }
        else if (isActivated && CurrentAdrenaline < maxAdrenaline)
        {
            CurrentAdrenaline += regenerationRate * Time.deltaTime;
            CurrentAdrenaline = Mathf.Min(CurrentAdrenaline, maxAdrenaline);
            UpdateUI();

            if (adrenalineCheckText != null && CurrentAdrenaline >= 30f)
            {
                adrenalineCheckText.gameObject.SetActive(true);
            }
        }
    }

    public void ActivateAdrenaline()
    {
        if (isActivated) return;

        isActivated = true;
        float boostAmount = Random.Range(activationBoostRange.x, activationBoostRange.y);
        CurrentAdrenaline = Mathf.Min(CurrentAdrenaline + boostAmount, maxAdrenaline);
        UpdateUI();
        UpdateActivationPrompt();
    }

    public bool CanUseAdrenaline(float amount) => CurrentAdrenaline >= amount && isActivated;

    public bool UseAdrenaline(float amount)
    {
        if (!CanUseAdrenaline(amount)) return false;

        CurrentAdrenaline -= amount;
        UpdateUI();
        return true;
    }

    public void StartContinuousDrain(float rate)
    {
        isDraining = true;
        drainRate = rate;
    }

    public void StopContinuousDrain()
    {
        isDraining = false;
    }

    public float GetAdrenalinePercentage() => CurrentAdrenaline / maxAdrenaline;

    private void InitializeUI()
    {
        if (adrenalineSlider != null)
        {
            adrenalineSlider.maxValue = maxAdrenaline;
            adrenalineSlider.value = CurrentAdrenaline;
        }
        UpdateAdrenalinePercentageText();
        UpdateActivationPrompt();
    }

    private void UpdateUI()
    {
        if (adrenalineSlider != null)
        {
            adrenalineSlider.value = CurrentAdrenaline;
        }
        UpdateAdrenalinePercentageText();
    }

    private void UpdateAdrenalinePercentageText()
    {
        if (adrenalinePercentageText != null)
        {
            adrenalinePercentageText.text = $"{Mathf.RoundToInt(GetAdrenalinePercentage() * 100f)}%";
        }
    }

    private void UpdateActivationPrompt()
    {
        if (activationPrompt != null)
        {
            activationPrompt.SetActive(!isActivated);
        }
    }
}