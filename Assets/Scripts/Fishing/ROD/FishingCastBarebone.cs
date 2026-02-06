using UnityEngine;

public class FishingCast : MonoBehaviour
{

    [Header("Player Setup")] 
    public Transform handTransform;
    [Header("Bobber Setup")]
    public Rigidbody bobberRT;
    public Bobber bobber;
    public bool HasCasted => hasCasted;
    public bool IsReeling => isReeling;

    
    [Header("Rod Setup")]
    public Transform head;
    public Transform rodHead;
    public LineRenderer line;

    [Header("Tuning")]
    public float castForce = 15f;
    public float reelSpeed = 12f;

    private bool hasCasted;
    private bool isReeling;
    [HideInInspector] public bool isTalking;

    private void Start()
    {
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
        
        bobber.ResetBobber();

        bobberRT.transform.parent = null;
        bobberRT.transform.position = head.position;

        bobberRT.isKinematic = false;
        bobberRT.velocity = Vector3.zero;
        bobberRT.angularVelocity = Vector3.zero;

        bobberRT.AddForce(head.forward * castForce, ForceMode.Impulse);
    }

    private void StartReel()
    {
        if (bobber.currentFish != null && bobber.currentFish.IsBiting())
        {
            bobber.currentFish.TryCatchFish(handTransform);
        }
        else if (bobber.currentFish != null)
        {
            bobber.currentFish.BobberLeft();
        }

        isReeling = true;
        bobberRT.isKinematic = true;
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
}