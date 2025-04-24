using UnityEngine;
using UnityEngine.InputSystem;

public class UseAdrenaline : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private AdrenalineBar adrenalineBar;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CharacterInjury characterInjury; // New reference

    [Header("Adrenaline Settings")]
    [SerializeField] private float adrenalineCost = 30f;
    [SerializeField] private float adrenalineDrainRate = 2f;
    [SerializeField] private float speedMultiplier = 1.5f;

    public GameObject Aura;
    public bool isAdrenalineActive = false;
    private float originalSpeed;
    private PlayerInput playerInput;
    private InputAction useAdrenalineAction;

    private void Awake()
    {
        // Get references if not set in inspector
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (adrenalineBar == null)
            adrenalineBar = FindObjectOfType<AdrenalineBar>();

        if (characterInjury == null)
            characterInjury = GetComponent<CharacterInjury>();

        if (playerMovement != null)
            originalSpeed = playerMovement.speed;

        if (Aura != null)
            Aura.SetActive(false);

        // Set up input
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
            useAdrenalineAction = playerInput.actions["UseAdrenaline"];
    }

    private void OnEnable()
    {
        if (adrenalineBar != null)
            adrenalineBar.OnAdrenalineEmpty += EndAdrenalineBoost;

        if (useAdrenalineAction != null)
            useAdrenalineAction.performed += OnAdrenalineInput;
    }

    private void OnDisable()
    {
        if (adrenalineBar != null)
            adrenalineBar.OnAdrenalineEmpty -= EndAdrenalineBoost;

        if (useAdrenalineAction != null)
            useAdrenalineAction.performed -= OnAdrenalineInput;

        // Ensure we reset stats if disabled during boost
        if (isAdrenalineActive)
            EndAdrenalineBoost();
    }

    private void OnAdrenalineInput(InputAction.CallbackContext context)
    {
        if (!isAdrenalineActive && adrenalineBar != null && adrenalineBar.CanUseAdrenaline(adrenalineCost))
        {
            TryUseAdrenaline();
        }
    }

    private void TryUseAdrenaline()
    {
        if (adrenalineBar.CurrentAdrenaline > adrenalineCost)
        {
            ActivateAdrenalineBoost();
        }
    }

    private void ActivateAdrenalineBoost()
    {
        isAdrenalineActive = true;
        Aura.SetActive(true);

        if (playerMovement != null)
            playerMovement.speed = originalSpeed * speedMultiplier;

        if (characterInjury != null)
            characterInjury.ActivateAdrenaline(); // Injuries are temporarily negated

        adrenalineBar.StartContinuousDrain(adrenalineDrainRate);
    }

    private void EndAdrenalineBoost()
    {
        if (!isAdrenalineActive) return;

        isAdrenalineActive = false;
        Aura.SetActive(false);

        if (playerMovement != null)
            playerMovement.speed = originalSpeed;

        if (characterInjury != null)
            characterInjury.DeactivateAdrenaline(); // Injuries are reapplied

        adrenalineBar.StopContinuousDrain();
    }
}
