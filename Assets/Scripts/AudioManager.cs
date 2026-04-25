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
    [SerializeField] private EventReference musicEvent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
