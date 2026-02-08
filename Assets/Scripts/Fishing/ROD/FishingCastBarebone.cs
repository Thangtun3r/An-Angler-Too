using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class FishingCast : MonoBehaviour
{

    [SerializeField] private Camera cam;   // assign in Inspector (or use Camera.main)
    public float aimDistance = 25f;
    
    [Header("Player Setup")] 
    public Transform handTransform;
    [Header("Bobber Setup")]
    public Rigidbody bobberRT;
    public Bobber bobber;
    public bool HasCasted => hasCasted;
    public bool IsReeling => isReeling;
    
    [Header("Hand")]
    [SerializeField] private Animator handAnimator;   
    [SerializeField] private string isStoringParam = "isStoring";


    
    [Header("Rod Setup")]
    public Transform head;
    public Transform rodHead;
    public LineRenderer line;
    [SerializeField] private RodAnimationController rodAnimationController;

    [Header("Tuning")]
    public float castForce = 15f;
    public float reelSpeed = 12f;

    private bool hasCasted;
    private bool isReeling;
    [HideInInspector] public bool isTalking;
    private bool hasFishToPullUp;

    private EventInstance reelingIdleInstance;
    private EventInstance rollbackLoopInstance;
    private bool rollbackActive;

    private void Start()
    {
        if (rodAnimationController == null)
            rodAnimationController = GetComponentInChildren<RodAnimationController>();

        line.positionCount = 2;
        line.useWorldSpace = true;
        AttachToRodIdle();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTalking)
        {
            if (!hasCasted && !isReeling)
            {
                CastRod();
            }
            else if (hasCasted && !isReeling)
            {
                StartReel();
            }
        }

        IdleFollowRod();
        ReelMovement();
        UpdateLine();
    }

    private void CastRod()
    {
        hasCasted = true;
        isReeling = false;
        hasFishToPullUp = false;

        StopLoop(ref reelingIdleInstance);
        StopLoop(ref rollbackLoopInstance);
        rollbackActive = false;
        var events = FMODEvents.Instance;
        if (events != null)
            PlayOneShot(events.fishingRodReelingThrow);
    
        bobber.ResetBobber();
    
        bobberRT.transform.parent = null;
    
        bobberRT.transform.position = rodHead.position;

        if (cam == null) cam = Camera.main;
    
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
 
        Vector3 targetPoint = ray.origin + ray.direction * aimDistance;
        if (Physics.Raycast(ray, out RaycastHit hit, aimDistance))
            targetPoint = hit.point;
    
        Vector3 dir = (targetPoint - rodHead.position).normalized;
    
        bobberRT.isKinematic = false;
        bobberRT.velocity = Vector3.zero;
        bobberRT.angularVelocity = Vector3.zero;
    
        bobberRT.AddForce(dir * castForce, ForceMode.Impulse);
        if (events != null)
            StartLoop(ref reelingIdleInstance, events.fishingRodReelingIdle);
    }

    private void StartReel()
    {
        bool fishBiting = bobber.currentFish != null && bobber.currentFish.IsBiting();
        if (fishBiting)
        {
            bool caught = bobber.currentFish.TryCatchFish(handTransform);
            hasFishToPullUp = true;
            if (caught)
            {
                if (rodAnimationController != null)
                    rodAnimationController.StopBiteLoopsImmediate();

                var events = FMODEvents.Instance;
                if (events != null)
                    PlayOneShot(events.fishingPullUp);
            }
            else
            {
                var events = FMODEvents.Instance;
                if (events != null)
                    StartLoop(ref rollbackLoopInstance, events.fishingRodRollback);
                rollbackActive = true;
            }
        }
        else if (bobber.currentFish != null)
        {
            bobber.currentFish.BobberLeft();
            var events = FMODEvents.Instance;
            if (events != null)
                StartLoop(ref rollbackLoopInstance, events.fishingRodRollback);
            rollbackActive = true;
        }
        else
        {
            var events = FMODEvents.Instance;
            if (events != null)
                StartLoop(ref rollbackLoopInstance, events.fishingRodRollback);
            rollbackActive = true;
        }

        isReeling = true;
        bobberRT.isKinematic = true;
        StopLoop(ref reelingIdleInstance);
    }



    private void ReelMovement()
    {
        if (!isReeling) return;

        bobberRT.transform.position = Vector3.MoveTowards(
            bobberRT.transform.position,
            rodHead.position,
            reelSpeed * Time.deltaTime
        );
        
        if (Vector3.Distance(bobberRT.transform.position, rodHead.position) < 0.05f)
        {
            isReeling = false;
            hasCasted = false;
            AttachToRodIdle();
            StopLoop(ref rollbackLoopInstance);
            var events = FMODEvents.Instance;
            if (rollbackActive && events != null)
                PlayOneShot(events.fishingRodRollbackRetrieved);
            // Pull-up SFX is triggered immediately on successful catch (in StartReel).
            hasFishToPullUp = false;
            rollbackActive = false;
        }
    }

    private void IdleFollowRod()
    {
        if (!hasCasted && !isReeling)
        {
            AttachToRodIdle();
        }
    }

    private void AttachToRodIdle()
    {
        bobberRT.isKinematic = true;
        bobberRT.transform.parent = rodHead;
        bobberRT.transform.localPosition = Vector3.zero;
        bobberRT.transform.localRotation = Quaternion.identity;
    }

    private void UpdateLine()
    {
        line.SetPosition(0, rodHead.position);
        line.SetPosition(1, bobberRT.transform.position);
    }

    private void OnDisable()
    {
        StopLoop(ref reelingIdleInstance);
        StopLoop(ref rollbackLoopInstance);
        hasFishToPullUp = false;
        rollbackActive = false;
    }

    private void PlayOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, rodHead != null ? rodHead.position : transform.position);
    }

    private void StartLoop(ref EventInstance instance, EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        if (instance.isValid())
            return;

        instance = AudioManager.Instance.CreateEventInstance(evt);
        instance.start();
    }

    private void StopLoop(ref EventInstance instance)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.StopEventInstance(ref instance);
    }
}
