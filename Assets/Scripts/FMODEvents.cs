using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }
    [field: SerializeField] public EventReference ambienceWater { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference fishingRodBaitDown { get; private set; }
    [field: SerializeField] public EventReference fishingRodReelingThrow { get; private set; }
    [field: SerializeField] public EventReference fishingRodReelingIdle { get; private set; }
    [field: SerializeField] public EventReference fishingRodFishCaughtLoop { get; private set; }
    [field: SerializeField] public EventReference fishingRodReelingLoop { get; private set; }
    [field: SerializeField] public EventReference fishingPullUp { get; private set; }
    [field: SerializeField] public EventReference fishingRodRollback { get; private set; }

    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference uiClick { get; private set; }
    [field: SerializeField] public EventReference uiPickUp { get; private set; }
    [field: SerializeField] public EventReference uiPlaceDown { get; private set; }
    [field: SerializeField] public EventReference uiBoxOpen { get; private set; }
    [field: SerializeField] public EventReference uiBoxClose { get; private set; }
    [field: SerializeField] public EventReference uiDiscard { get; private set; }
    [field: SerializeField] public EventReference uiPageTurn { get; private set; }
    [field: SerializeField] public EventReference uiHover { get; private set; }

    public static FMODEvents Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Error: More than one AudioManager instance found — destroying the new one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
