using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FishContainer : MonoBehaviour, IFish
{
    [Header("Fish Data")]
    public ItemSO fishSO;

    [Header("Bite Timing")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    float biteWindow = 11.5f;

    public event System.Action OnFishBite;
    public event System.Action OnFishGoAway;

    bool fishIsBiting;
    Transform currentBobber;

    Coroutine biteRoutine;
    Vector3 surfaceCenter;

    public void BobberLanded(Transform bobber)
    {
        currentBobber = bobber;
        surfaceCenter = bobber.position;

        StopAllInternal();

        biteRoutine = StartCoroutine(FishBiteRoutine());
    }

    public void BobberLeft()
    {
        StopAllInternal();
        currentBobber = null;
    }

    public bool IsBiting()
    {
        return fishIsBiting;
    }

    public bool TryCatchFish(Transform handTransform)
    {
        if (!fishIsBiting)
            return false;

        fishIsBiting = false;
        StopAllInternal();

        Vector3 spawnPos = currentBobber != null
            ? currentBobber.position
            : transform.position;

        GameObject fishObj = Instantiate(
            fishSO.item_prefab,
            spawnPos,
            Quaternion.identity
        );

        AnimateFishToHand(fishObj, handTransform);
        return true;
    }

    IEnumerator FishBiteRoutine()
    {
        while (currentBobber != null)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

            fishIsBiting = true;
            OnFishBite?.Invoke();

            yield return new WaitForSeconds(biteWindow);

            if (fishIsBiting)
            {
                fishIsBiting = false;
                OnFishGoAway?.Invoke();
            }
        }
    }


    void StopAllInternal()
    {
        if (fishIsBiting)
        {
            fishIsBiting = false;
            OnFishGoAway?.Invoke();
        }

        if (biteRoutine != null)
        {
            StopCoroutine(biteRoutine);
            biteRoutine = null;
        }

        if (currentBobber != null)
            currentBobber.position = surfaceCenter;
    }

    void AnimateFishToHand(GameObject fishObj, Transform hand)
    {
        float travelTime = 1.5f;
        float spinPerMeter = 360f;

        Vector3 startPos = fishObj.transform.position;
        Vector3 endPos = hand.position;

        float distance = Vector3.Distance(startPos, endPos);
        float totalSpin = distance * spinPerMeter;

        Sequence seq = DOTween.Sequence();

        seq.Append(
            fishObj.transform.DOMove(endPos, travelTime)
        );

        seq.Join(
            fishObj.transform.DORotate(
                    new Vector3(0f, totalSpin, 0f),
                    travelTime,
                    RotateMode.FastBeyond360
                )
                .SetRelative(true)
                .SetEase(Ease.OutQuad)
        );


        seq.OnComplete(() =>
        {
            fishObj.transform.SetParent(hand);
            fishObj.transform.localPosition = Vector3.zero;
            fishObj.transform.localRotation = Quaternion.identity;

            DOVirtual.DelayedCall(2f, () =>
            {
                InventoryService.AddToPlayer(fishSO);
                Destroy(fishObj);
            });
        });
    }

}
