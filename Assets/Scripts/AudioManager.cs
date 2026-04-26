using UnityEngine;

using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private EventReference footstepEvent;
    [SerializeField] private EventReference jumpEvent;
    [SerializeField] private EventReference ambientEvent;
    [SerializeField] private EventReference backgroundEvent;
    [SerializeField] private EventReference finalCue;
    [SerializeField] private EventReference UI_Sounds;
    [SerializeField] private EventReference harvest;
    [SerializeField] private EventReference menuMusicEvent;
    [SerializeField] private EventReference earthquakeEvent;

    private EventInstance ambientInstance;
    private EventInstance backgroundInstance;
    private EventInstance menuMusicInstance;
    private EventInstance earthquakeInstance;

    private bool isEarthquakePlaying = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ambientInstance = RuntimeManager.CreateInstance(ambientEvent);
        ambientInstance.start();

        backgroundInstance = RuntimeManager.CreateInstance(backgroundEvent);
        backgroundInstance.start();
    }

    private bool IsEventInstancePlaying(EventInstance instance)
    {
        if (!instance.isValid())
            return false;

        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING;
    }

    public void SetWindPower(float parameterValue)
    {
        if (ambientInstance.isValid() && !IsEventInstancePlaying(ambientInstance))
            ambientInstance.start();
        ambientInstance.setParameterByName("WindPower", parameterValue);
    }

    public void PlayFootstep(bool isRunning)
    {
        EventInstance footstepInstance = RuntimeManager.CreateInstance(footstepEvent);
        if (isRunning)
            footstepInstance.setParameterByNameWithLabel("State", "Running");
        else
            footstepInstance.setParameterByNameWithLabel("State", "Walking");
        footstepInstance.start();
        footstepInstance.release();
    }

    public void PlayJump(bool start)
    {
        EventInstance jumpInstance = RuntimeManager.CreateInstance(jumpEvent);
        if (start)
            jumpInstance.setParameterByNameWithLabel("State", "Jump");
        else
            jumpInstance.setParameterByNameWithLabel("State", "Land");
        jumpInstance.start();
        jumpInstance.release();
    }

    public void PlayHarvest()
    {
        EventInstance harvestInstance = RuntimeManager.CreateInstance(harvest);
        harvestInstance.start();
        harvestInstance.release();
    }

    // This method can be used for any UI click sound, the "Action" parameter can be set to differentiate between different types of clicks:
    // Click, Craft, OpenUI, Writing, Consume, Drink, BreakPotion
    public void PlayUIClick(string action)
    {
        EventInstance uiInstance = RuntimeManager.CreateInstance(UI_Sounds);
        uiInstance.setParameterByNameWithLabel("Action", action);
        uiInstance.start();
        uiInstance.release();
    }

    public void SetBackground(float stress)
    {
        if (backgroundInstance.isValid() && !IsEventInstancePlaying(backgroundInstance))
            backgroundInstance.start();
        backgroundInstance.setParameterByName("Stress", stress);
    }

    public void PlayFinalCue()
    {
        EventInstance finalCueInstance = RuntimeManager.CreateInstance(finalCue);
        finalCueInstance.start();
        finalCueInstance.release();
    }

    public void PlayMenuMusic()
    {
        if (menuMusicInstance.isValid() && IsEventInstancePlaying(menuMusicInstance))
            return;
        menuMusicInstance = RuntimeManager.CreateInstance(menuMusicEvent);
        menuMusicInstance.start();
    }

    public void StopMenuMusic()
    {
        if (menuMusicInstance.isValid())
        {
            menuMusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
            menuMusicInstance.release();
        }
    }

    public void PlayEarthquake()
    {
        if (earthquakeInstance.isValid() && IsEventInstancePlaying(earthquakeInstance) && isEarthquakePlaying)
            return;
        earthquakeInstance = RuntimeManager.CreateInstance(earthquakeEvent);
        earthquakeInstance.start();
        isEarthquakePlaying = true;
    }

    public void StopEarthquake()
    {
        if (earthquakeInstance.isValid() && isEarthquakePlaying)
        {
            earthquakeInstance.stop(STOP_MODE.ALLOWFADEOUT);
            earthquakeInstance.release();
            isEarthquakePlaying = false;
        }
    }
    
    private void OnDestroy()
    {
        if (ambientInstance.isValid())
        {
            ambientInstance.stop(STOP_MODE.IMMEDIATE);
            ambientInstance.release();
        }
        if (backgroundInstance.isValid())
        {
            backgroundInstance.stop(STOP_MODE.IMMEDIATE);
            backgroundInstance.release();
        }
    }

}
