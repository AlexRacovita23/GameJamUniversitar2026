using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SanitySystem : MonoBehaviour
{
    public static SanitySystem Instance { get; private set; }
    [SerializeField] private float sanity = 100;
    [SerializeField] private float sanityDecreaseRatePerSecond = 0.4f; // 0.4 sanity/seconds -> 98 sanity in 4 minutes
    [SerializeField] private float sanityDecreaseOutsideBorder = 2f; // 2 sanity/seconds -> 0 sanity in 50 seconds

    [Header("Debug")]
    [SerializeField] private Slider debugSlider = null;

    [Header("Effects")]
    [SerializeField] private Volume volume;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    [SerializeField] private ParticleSystem sandstormParticles;
    private ParticleSystem.EmissionModule sandstormEmission;
    [SerializeField] private float maxParticleEmissionRate = 2500f;
    [SerializeField] private float minParticleEmissionRate = 1000f;
    [SerializeField] private WorldBorder worldBorder;
    private bool isOutsideBorder = false;

    [SerializeField] private GameObject blurbOverlay;
    private float blurbTimer = 0f;
    private bool isBlurbActive = false;

    public GameObject LoseScreen;
    [SerializeField] private PlayerMovement playerMovement;

    public float Sanity => sanity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        worldBorder.exitBorder += HandleExitBorder;
        worldBorder.enterBorder += HandleEnterBorder;
        InventoryUIManager.OnItemConsumed += HandleItemConsumed;
    }

    private void OnDisable()
    {
        worldBorder.exitBorder -= HandleExitBorder;
        worldBorder.enterBorder -= HandleEnterBorder;
        InventoryUIManager.OnItemConsumed -= HandleItemConsumed;
    }

    private void Start()
    {
        if (debugSlider != null)
        {
            debugSlider.minValue = 0;
            debugSlider.maxValue = 100;
            debugSlider.value = sanity;
        }
        if (volume.profile.TryGet<Vignette>(out var vignette))
        {
            this.vignette = vignette;
        }
        if (blurbOverlay != null)
        {
            blurbOverlay.SetActive(false);
        }
    }

    void Update()
    {
        sanity -= sanityDecreaseRatePerSecond * Time.deltaTime;
        sanity = Mathf.Clamp(sanity, 0, 100);

        if (sanity <= 0)
        {
            // Handle player death or game over logic here
            Debug.Log("Player has lost all sanity!");
            if (LoseScreen != null && !LoseScreen.activeSelf)
            {
                LoseScreen.SetActive(true);
                playerMovement.ChangeCoursorState();
            }
        }

        UpdateVisuals();
        UpdateBlurb();
    }

    private void HandleItemConsumed(ItemData item)
    {
        if (item == null) return;

        switch (item.ItemName)
        {
            case "GoodPotion":
                IncreaseSanity(30f * sanityDecreaseRatePerSecond);
                Debug.Log("[SanitySystem] Good Potion consumed: +30 seconds of sanity");
                Debug.Log($"[SanitySystem] Current Sanity Level: {sanity}");
                break;

            case "BadPotion":
                DecreaseSanity(20f * sanityDecreaseRatePerSecond);
                Debug.Log("[SanitySystem] Bad Potion consumed: -20 seconds of sanity");
                Debug.Log($"[SanitySystem] Current Sanity Level: {sanity}");
                break;

            case "NeutralPotion":
                StartBlurb(3f);
                Debug.Log("[SanitySystem] Neutral Potion consumed: 3 second blurb effect");
                Debug.Log($"[SanitySystem] Current Sanity Level: {sanity}");
                break;

            case "TemplePotion":
                // Spawns the win condition, cannot be used on self/outside obelisk range
                // TODO: Implement win condition spawn - requires obelisk range check
                Debug.Log("[SanitySystem] Temple Potion consumed: Win condition trigger (not implemented)");
                break;

            case "CactusFlower":
                IncreaseSanity(10f * sanityDecreaseRatePerSecond);
                Debug.Log("[SanitySystem] Cactus Flower consumed: +10 seconds of sanity");
                Debug.Log($"[SanitySystem] Current Sanity Level: {sanity}");
                break;

            case "ScorpionVenom":
                DecreaseSanity(15f * sanityDecreaseRatePerSecond);
                Debug.Log("[SanitySystem] Scorpion Venom consumed: -15 seconds of sanity");
                Debug.Log($"[SanitySystem] Current Sanity Level: {sanity}");
                break;

            case "LizardBlood":
                DecreaseSanity(10f * sanityDecreaseRatePerSecond);
                Debug.Log("[SanitySystem] Lizard Blood consumed: -10 seconds of sanity");
                Debug.Log($"[SanitySystem] Current Sanity Level: {sanity}");
                break;

            default:
                Debug.Log($"[SanitySystem] Unknown item consumed: {item.ItemName}");
                Debug.Log($"[SanitySystem] Current Sanity Level: {sanity}");
                break;
        }
    }

    private void StartBlurb(float duration)
    {
        blurbTimer = duration;
        isBlurbActive = true;
        if (blurbOverlay != null)
        {
            blurbOverlay.SetActive(true);
        }
    }

    private void UpdateBlurb()
    {
        if (!isBlurbActive) return;

        blurbTimer -= Time.deltaTime;
        if (blurbTimer <= 0f)
        {
            isBlurbActive = false;
            if (blurbOverlay != null)
            {
                blurbOverlay.SetActive(false);
            }
        }
    }

    public void IncreaseSanity(float amount)
    {
        sanity += amount;
        sanity = Mathf.Clamp(sanity, 0, 100);
        if (debugSlider != null)
            debugSlider.value = sanity;
    }

    public void DecreaseSanity(float amount)
    {
        sanity -= amount;
        sanity = Mathf.Clamp(sanity, 0, 100);
        if (debugSlider != null)
            debugSlider.value = sanity;
    }

    private void HandleExitBorder()
    {
        isOutsideBorder = true;
        sanityDecreaseRatePerSecond = sanityDecreaseOutsideBorder;
        if (sanity > 20)
        {
            sandstormParticles.Play(); // Start sandstorm effect if sanity is above critical level
            AudioManager.Instance.SetWindPower(20f);
        }
    }

    private void HandleEnterBorder()
    {
        isOutsideBorder = false;
        sanityDecreaseRatePerSecond = 0.4f; // Reset to default rate
        if (sanity > 20)
        {
            sandstormParticles.Stop(); // Stop sandstorm effect if sanity is above critical level
            AudioManager.Instance.SetWindPower(0f);
        }
    }

    private void UpdateVisuals()
    {
        if (debugSlider != null)
        {
            debugSlider.value = sanity;
        }

        if (vignette != null)
        {
            AudioManager.Instance.SetBackground(100 - sanity); // Adjust background music based on sanity
            if (sanity < 50 && sanity >= 20)
            {
                // Increase vignette intensity as sanity decreases
                float intensity = Mathf.Lerp(0, 0.5f, (50 - sanity) / 30);
                vignette.intensity.value = intensity;
                sandstormParticles.Stop(); // Stop sandstorm effect if sanity is above critical level
                AudioManager.Instance.SetWindPower(0f);
            }
            else if (sanity < 20 && sanity > 0)
            {
                // Decide on maximum insanity effect
                Debug.Log("Player is in critical sanity state!");
                vignette.intensity.value = 0.5f; // Max intensity
                sandstormParticles.Play(); // Start sandstorm effect
                float intensity = Mathf.Lerp(minParticleEmissionRate, maxParticleEmissionRate, sanity / 20); // Adjust particle emission based on sanity
                sandstormEmission = sandstormParticles.emission;
                sandstormEmission.rateOverTime = intensity;
                AudioManager.Instance.SetWindPower(20f - sanity); // Increase wind sound as sanity decreases
            }
            else
            {
                vignette.intensity.value = 0;
                if (!isOutsideBorder)
                {
                    sandstormParticles.Stop(); // Stop sandstorm effect
                    AudioManager.Instance.SetWindPower(0f);
                }
            }
        }
    }
}