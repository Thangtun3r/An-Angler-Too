using UnityEngine;

public class RodAnimationController : MonoBehaviour
{
    float currentSpinSpeed;
    bool castImpulseActive;

    Quaternion reelBaseRotation;

    float biteCurrentAngle;
    float biteTargetAngle;
    float biteHoldTimer;
    float biteShakeTimer;
    float currentBiteHoldTime;

    public float biteMinStep = 90f;
    public float biteMaxStep = 270f;
    public float biteRotateSpeed = 720f;

    public float biteHoldTimeMin = 0.4f;
    public float biteHoldTimeMax = 1.2f;

    public float biteShakeAmount = 8f;
    public float biteShakeSpeed = 18f;

    public float castEaseOut = 3f;

    public enum RodState
    {
        Idle_Rod = 0,
        Cast_Rod = 1,
        Reel_Rod = 2,
        FishBite_Rod = 3,
        Reel_End = 4
    }

    public FishingCast fishingCast;
    public Animator rodAnimator;
    public Transform reelTransform;

    public float reelSpinSpeed = 720f;
    public float castSpinSpeed = 360f;

    public string rodStateParam = "Rod_State";

    public RodState CurrentState { get; private set; }

    bool fishBiting;
    IFish currentFish;

    void Awake()
    {
        if (fishingCast == null)
            fishingCast = GetComponentInParent<FishingCast>();

        if (rodAnimator == null)
            rodAnimator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        Bobber.OnBobberLanded += HandleBobberLanded;
    }

    void OnDisable()
    {
        Bobber.OnBobberLanded -= HandleBobberLanded;
        UnsubscribeFromFish();
    }

    void Start()
    {
        reelBaseRotation = reelTransform.localRotation;
        SetState(RodState.Idle_Rod);
    }

    void Update()
    {
        UpdateStateFromGameplay();
        UpdateReelSpin();
    }

    void HandleBobberLanded()
    {
        castImpulseActive = false;

        UnsubscribeFromFish();

        if (fishingCast == null || fishingCast.bobber == null)
            return;

        IFish fish = fishingCast.bobber.currentFish;
        if (fish == null)
            return;

        currentFish = fish;
        currentFish.OnFishBite += OnFishBite;
        currentFish.OnFishGoAway += OnFishGoAway;

        if (currentFish.IsBiting())
            OnFishBite();
    }

    void UnsubscribeFromFish()
    {
        if (currentFish == null)
            return;

        currentFish.OnFishBite -= OnFishBite;
        currentFish.OnFishGoAway -= OnFishGoAway;
        currentFish = null;
    }

    void UpdateStateFromGameplay()
    {
        if (fishBiting && !fishingCast.IsReeling)
        {
            SetState(RodState.FishBite_Rod);
            return;
        }

        if (fishingCast.IsReeling)
        {
            fishBiting = false;
            SetState(RodState.Reel_Rod);
            return;
        }

        if (fishingCast.HasCasted)
        {
            SetState(RodState.Cast_Rod);
            return;
        }

        SetState(RodState.Idle_Rod);
    }

    void SetState(RodState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        if (newState == RodState.Cast_Rod)
        {
            currentSpinSpeed = -castSpinSpeed;
            castImpulseActive = true;
        }

        rodAnimator.SetInteger(rodStateParam, (int)newState);
    }

    void OnFishBite()
    {
        fishBiting = true;

        biteCurrentAngle = 0f;
        biteTargetAngle = GetNextBiteStep();
        biteHoldTimer = 0f;
        biteShakeTimer = 0f;
        currentBiteHoldTime = Random.Range(biteHoldTimeMin, biteHoldTimeMax);

        SetState(RodState.FishBite_Rod);
    }

    void OnFishGoAway()
    {
        fishBiting = false;
        reelTransform.localRotation = reelBaseRotation;
        SetState(RodState.Reel_Rod);
    }

    float GetNextBiteStep()
    {
        float step = Random.Range(biteMinStep, biteMaxStep);
        return biteCurrentAngle + step * (Random.value > 0.5f ? 1f : -1f);
    }

    void UpdateReelSpin()
    {
        if (reelTransform == null)
            return;

        if (CurrentState == RodState.FishBite_Rod)
        {
            if (Mathf.Abs(biteCurrentAngle - biteTargetAngle) > 1f)
            {
                biteCurrentAngle = Mathf.MoveTowards(
                    biteCurrentAngle,
                    biteTargetAngle,
                    biteRotateSpeed * Time.deltaTime
                );
            }
            else
            {
                biteHoldTimer += Time.deltaTime;
                biteShakeTimer += Time.deltaTime * biteShakeSpeed;

                float shake =
                    Mathf.Sin(biteShakeTimer) *
                    Random.Range(5f, biteShakeAmount);

                if (biteHoldTimer >= currentBiteHoldTime)
                {
                    biteHoldTimer = 0f;
                    currentBiteHoldTime = Random.Range(
                        biteHoldTimeMin,
                        biteHoldTimeMax
                    );
                    biteTargetAngle = GetNextBiteStep();
                }

                reelTransform.localRotation =
                    reelBaseRotation *
                    Quaternion.Euler(0f, biteCurrentAngle + shake, 0f);

                return;
            }

            reelTransform.localRotation =
                reelBaseRotation *
                Quaternion.Euler(0f, biteCurrentAngle, 0f);

            return;
        }

        if (CurrentState == RodState.Cast_Rod && castImpulseActive)
        {
            reelTransform.Rotate(
                Vector3.up,
                currentSpinSpeed * Time.deltaTime,
                Space.Self
            );
            return;
        }

        if (CurrentState == RodState.Reel_Rod)
        {
            currentSpinSpeed = reelSpinSpeed;

            reelTransform.Rotate(
                Vector3.up,
                currentSpinSpeed * Time.deltaTime,
                Space.Self
            );
            return;
        }

        if (!castImpulseActive)
        {
            float sign = Mathf.Sign(currentSpinSpeed);

            float speed = Mathf.Lerp(
                Mathf.Abs(currentSpinSpeed),
                0f,
                Time.deltaTime * castEaseOut
            );

            currentSpinSpeed = sign * speed;

            reelTransform.Rotate(
                Vector3.up,
                currentSpinSpeed * Time.deltaTime,
                Space.Self
            );
        }

    }
}
