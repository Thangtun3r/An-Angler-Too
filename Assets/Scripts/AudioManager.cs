using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;


public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances = new List<EventInstance>();
    private List<StudioEventEmitter> eventEmitters = new List<StudioEventEmitter>();
    private EventInstance ambienceEventInstance;
    public static AudioManager Instance { get; private set; }
    private bool isPrimaryInstance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Error: More than one AudioManager instance found — destroying the new one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        isPrimaryInstance = true;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (FMODEvents.Instance != null)
        {
            Debug.Log("AudioManager: initializing ambience.", this);
            InitializeAmbience(FMODEvents.Instance.ambience);
        }
        else
        {
            Debug.LogWarning("AudioManager: FMODEvents instance missing on Start.", this);
        }
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public void StopEventInstance(ref EventInstance eventInstance)
    {
        if (!eventInstance.isValid())
        {
            return;
        }

        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance.release();
        eventInstance.clearHandle();
    }
    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        if (emitter == null)
        {
            emitter = emitterGameObject.AddComponent<StudioEventEmitter>();
        }
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    public void StopEmittersOn(GameObject root)
    {
        if (root == null)
        {
            return;
        }

        StudioEventEmitter[] emitters = root.GetComponentsInChildren<StudioEventEmitter>(true);
        for (int i = 0; i < emitters.Length; i++)
        {
            emitters[i].Stop();
        }
    }
    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        if (ambienceEventInstance.isValid())
        {
            ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ambienceEventInstance.release();
        }

        Debug.Log("AudioManager: creating ambience instance.", this);
        ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    public void SetAmbienceStage(float value)
    {
        ambienceEventInstance.setParameterByName("StageIntensity", value);
    }

    public void EnsureAmbienceRunning()
    {
        if (FMODEvents.Instance == null)
        {
            Debug.LogWarning("AudioManager: EnsureAmbienceRunning called with no FMODEvents instance.", this);
            return;
        }

        if (!ambienceEventInstance.isValid())
        {
            Debug.Log("AudioManager: ambience invalid, reinitializing.", this);
            InitializeAmbience(FMODEvents.Instance.ambience);
            return;
        }

        PLAYBACK_STATE state;
        ambienceEventInstance.getPlaybackState(out state);
        Debug.Log($"AudioManager: ambience playback state = {state}.", this);
        if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
        {
            Debug.Log("AudioManager: restarting ambience.", this);
            ambienceEventInstance.start();
        }
    }
    private void CleanUp()
    {
        //stop and release all instances
        if (eventInstances != null)
        {
            foreach (EventInstance eventInstance in eventInstances)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventInstance.release();
            }
        }
        if (eventEmitters != null)
        {
            foreach (StudioEventEmitter emitter in eventEmitters)
            {
                emitter.Stop();
            }
        }
    }
    private void OnDestroy()
    {
        if (!isPrimaryInstance || Instance != this)
        {
            return;
        }

        CleanUp();
        Instance = null;
    }
}
