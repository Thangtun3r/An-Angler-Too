using UnityEngine;
using DG.Tweening;
using Yarn.Unity;

public class TurnOnFinalScene : MonoBehaviour
{
    public float zOffset = -1.268f;
    public float duration = 1f;
    public Ease ease = Ease.OutQuad;

    public GameObject prevCar;
    public GameObject newCar;

    [YarnCommand ("turnFinalOn")]
    public void TurningOnFinalScene()
    {   
        prevCar.SetActive(false);
        newCar.SetActive(true);

        Vector3 targetPos = transform.localPosition + new Vector3(0f, 0f, zOffset);
        transform.DOLocalMove(targetPos, duration).SetEase(ease);
    }
}