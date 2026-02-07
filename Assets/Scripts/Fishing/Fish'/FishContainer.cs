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
    public static event System.Action OnStoreStarted;
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

        float distance = Vector3.Distance(fishObj.transform.position, hand.position);
        float totalSpin = distance * spinPerMeter;

        fishObj.transform.SetParent(hand, worldPositionStays: true);
        Sequence seq = DOTween.Sequence();
        seq.Append(fishObj.transform.DOLocalMove(Vector3.zero, travelTime).SetEase(Ease.OutQuad));
        seq.Join(
            fishObj.transform.DOLocalRotate(
                new Vector3(0f, totalSpin, 0f),
                travelTime,
                RotateMode.FastBeyond360
            ).SetEase(Ease.OutQuad)
        );

        seq.OnComplete(() =>
        {
            Sequence settle = DOTween.Sequence();

            settle.Append(fishObj.transform.DOLocalMove(Vector3.zero, 0.20f).SetEase(Ease.OutQuad));
            float currentY = fishObj.transform.localEulerAngles.y;
            settle.Join(fishObj.transform.DOLocalRotate(new Vector3(0f, currentY, 0f), 0.20f)
                .SetEase(Ease.OutQuad));

            settle.Join(fishObj.transform.DOLocalRotate(Vector3.zero, 0.20f).SetEase(Ease.OutQuad));

            settle.OnComplete(() =>
            {
                DOVirtual.DelayedCall(2f, () =>
                {
                    OnStoreStarted?.Invoke();
                    InventoryService.AddToPlayer(fishSO);
                });
            });
        });
    }
}
