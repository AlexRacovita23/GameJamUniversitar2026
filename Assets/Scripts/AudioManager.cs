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

    private EventInstance ambientInstance;
    private EventInstance backgroundInstance;

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

}
