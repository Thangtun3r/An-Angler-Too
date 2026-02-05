using UnityEngine;
using System.Collections;
using DG.Tweening;
public class FishContainer : MonoBehaviour, IFish
{
    public ItemSO fishSO;

    [Header("Bite Timing")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    public float biteWindow = 1.5f;

    private bool fishIsBiting;
    private Transform currentBobber;

    public void BobberLanded(Transform bobber)
    {
        currentBobber = bobber;
        StartCoroutine(FishBiteRoutine());
    }

    public bool IsBiting()
    {
        return fishIsBiting;
    }

    IEnumerator FishBiteRoutine()
    {
        float wait = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(wait);

        fishIsBiting = true;
        Debug.Log("Fish is biting!");

        yield return new WaitForSeconds(biteWindow);

        fishIsBiting = false;
        Debug.Log("Fish got away...");
    }

    public bool TryCatchFish(Transform handTransform)
    {
        if (!fishIsBiting) return false;

        fishIsBiting = false;

        GameObject fishObj = Instantiate(
            fishSO.item_prefab,
            transform.position,           
            Quaternion.identity
        );

        AnimateFishToHand(fishObj, handTransform);

        return true;
    }
    private void AnimateFishToHand(GameObject fishObj, Transform hand)
    {
        Sequence seq = DOTween.Sequence();

        float travelTime = 0.5f;

        // flyyy to hand
        seq.Append(
            fishObj.transform.DOMove(hand.position, travelTime)
                .SetEase(Ease.InOutQuad)
        );
        
        seq.Join(
            fishObj.transform.DORotate(
                new Vector3(0, 1440f, 0),
                travelTime,
                RotateMode.FastBeyond360
            ).SetEase(Ease.Linear)
        );

        seq.OnComplete(() =>
        {
            fishObj.transform.SetParent(hand);
            fishObj.transform.localPosition = Vector3.zero;
            fishObj.transform.localRotation = Quaternion.identity;

            // Pause before inventory
            DOVirtual.DelayedCall(2f, () =>
            {
                InventoryService.AddToPlayer(fishSO);
                Destroy(fishObj);
            });
        });
    }



}