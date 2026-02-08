using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerAttackSwing { get; private set; }
    [field: SerializeField] public EventReference playerAttackImpact { get; private set; }
    [field: SerializeField] public EventReference playerAttackImpactLastHit { get; private set; }
    [field: SerializeField] public EventReference playerWalk { get; private set; }
    [field: SerializeField] public EventReference playerDash { get; private set; }
    [field: SerializeField] public EventReference playerDie { get; private set; }
    [field: SerializeField] public EventReference playerDrinkPotion { get; private set; }

    [field: Header("Boss SFX")]
    [field: SerializeField] public EventReference bossWalk { get; private set; }
    [field: SerializeField] public EventReference bossCloseAttack { get; private set; }
    [field: SerializeField] public EventReference bossLongAttack { get; private set; }
    [field: SerializeField] public EventReference bossTeleportAttack { get; private set; }
    [field: SerializeField] public EventReference bossCharge { get; private set; }
    [field: SerializeField] public EventReference bossStrongAttack { get; private set; }
    [field: SerializeField] public EventReference bossHit { get; private set; }
    [field: SerializeField] public EventReference bossRoar { get; private set; }
    [field: SerializeField] public EventReference bossStaggered { get; private set; }
    [field: SerializeField] public EventReference deathScreen { get; private set; }

    [field: Header("Coin SFX")]
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
