using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    [SerializeField] private float sanity = 100;
    [SerializeField] private float sanityDecreaseRatePerSecond = 0.4f; // 0.4 sanity/seconds -> 98 sanity in 4 minutes

    [Header("Debug")]
    [SerializeField] private Slider debugSlider;

    [Header("Post-Processing")]
    [SerializeField] private Volume volume;
    private Vignette vignette;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
    }

    void Update()
    {
        sanity -= sanityDecreaseRatePerSecond * Time.deltaTime;
        sanity = Mathf.Clamp(sanity, 0, 100);

        if (sanity <= 0)
        {
            // Handle player death or game over logic here
            Debug.Log("Player has lost all sanity!");
        }

        UpdateVisuals();
    }

    public void IncreaseSanity(float amount)
    {
        sanity += amount;
        sanity = Mathf.Clamp(sanity, 0, 100);
        debugSlider.value = sanity;
    }

    public void DecreaseSanity(float amount)
    {
        sanity -= amount;
        sanity = Mathf.Clamp(sanity, 0, 100);
        debugSlider.value = sanity;
    }

    private void UpdateVisuals()
    {
        if (debugSlider != null)
        {
            debugSlider.value = sanity;
        }
        if (vignette != null)
        {
            if (sanity < 50 && sanity >= 20)
            {
                // Increase vignette intensity as sanity decreases
                float intensity = Mathf.Lerp(0, 0.5f, (50 - sanity) / 30);
                vignette.intensity.value = intensity;
            }
            else if (sanity < 20 && sanity > 0)
            {
                // Decide on maximum insanity effect
                Debug.Log("Player is in critical sanity state!");
            }
            else
            {
                vignette.intensity.value = 0;
            }
        }
    }    
}
