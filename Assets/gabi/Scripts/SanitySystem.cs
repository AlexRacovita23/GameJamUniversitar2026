using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SanitySystem : MonoBehaviour
{
    public static SanitySystem Instance { get; private set; }
    [SerializeField] private float sanity = 100;
    [SerializeField] private float sanityDecreaseRatePerSecond = 0.4f;
    [SerializeField] private float sanityDecreaseOutsideBorder = 2f;

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

    [Header("Ambient Light")]
    [SerializeField] private Light ambientLight;
    [SerializeField] private float defaultLightIntensity = 1f;
    [SerializeField] private float maxLightIntensity = 1.5f;

    [SerializeField] private GameObject blurbOverlay;
    private float blurbTimer = 0f;
    private bool isBlurbActive = false;

    private int frameCounter = 0;
    private const int UPDATE_INTERVAL = 3;

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
        if (volume.profile.TryGet<Vignette>(out var vig))
        {
            vignette = vig;
        }
        if (blurbOverlay != null)
        {
            blurbOverlay.SetActive(false);
        }
        if (ambientLight != null)
        {
            defaultLightIntensity = ambientLight.intensity;
        }

        if (sandstormParticles != null)
        {
            sandstormEmission = sandstormParticles.emission;
        }
    }

    void Update()
    {
        sanity -= sanityDecreaseRatePerSecond * Time.deltaTime;
        sanity = Mathf.Clamp(sanity, 0, 100);

        if (sanity <= 0)
        {
            Debug.Log("Player has lost all sanity!");
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
    }

    private void HandleEnterBorder()
    {
        isOutsideBorder = false;
        sanityDecreaseRatePerSecond = 0.4f;
    }

    private void UpdateVisuals()
    {
        if (debugSlider != null)
            debugSlider.value = sanity;

        frameCounter++;
        if (frameCounter < UPDATE_INTERVAL) return;
        frameCounter = 0;

        float sanityNormalized = sanity / 100f; // 0 to 1

        UpdateSandstorm(sanityNormalized);
        UpdateVignette(sanityNormalized);
        UpdateAmbientLight();
        UpdateAudio(sanityNormalized);
    }

    private void UpdateSandstorm(float sanityNormalized)
    {
        if (sandstormParticles == null) return;

        // High sanity = intense sandstorm, Low sanity = mild sandstorm
        float emissionRate = Mathf.Lerp(minParticleEmissionRate, maxParticleEmissionRate, sanityNormalized);
        sandstormEmission.rateOverTime = emissionRate;

        if (!sandstormParticles.isPlaying)
        {
            sandstormParticles.Play();
        }
    }

    private void UpdateVignette(float sanityNormalized)
    {
        if (vignette == null) return;

        // Low sanity = intense vignette (max 0.6 at 0 sanity)
        // High sanity = no vignette
        float vignetteIntensity = Mathf.Lerp(0.6f, 0f, sanityNormalized);
        vignette.intensity.value = vignetteIntensity;
    }

    private void UpdateAmbientLight()
    {
        if (ambientLight == null) return;

        float lightIntensity;

        if (sanity > 50f)
        {
            lightIntensity = defaultLightIntensity;
        }
        else if (sanity > 35f)
        {
            float t = (50f - sanity) / 15f;
            lightIntensity = Mathf.Lerp(defaultLightIntensity, maxLightIntensity, t);
        }
        else if (sanity > 20f)
        {
            float t = (35f - sanity) / 15f;
            lightIntensity = Mathf.Lerp(maxLightIntensity, defaultLightIntensity, t);
        }
        else
        {
            lightIntensity = defaultLightIntensity;
        }

        ambientLight.intensity = lightIntensity;
    }

    private void UpdateAudio(float sanityNormalized)
    {
        if (AudioManager.Instance == null) return;

        // Background intensity inversely proportional to sanity
        AudioManager.Instance.SetBackground(100f - sanity);

        // Wind power: higher at high sanity (matches sandstorm intensity)
        float windPower = sanityNormalized * 20f;
        AudioManager.Instance.SetWindPower(windPower);
    }
}