using UnityEngine;
using DG.Tweening;
using Yarn.Unity;
using UnityEngine.Timeline;

public class TurnOnFinalScene : MonoBehaviour
{
    public float zOffset = -1.268f;
    public float duration = 1f;
    public Ease ease = Ease.OutQuad;

    public GameObject prevCar;
    public GameObject newCar;
    public GameObject finalDoor;
    public GameObject sign;

    [YarnCommand ("turnFinalOn")]
    public void TurningOnFinalScene()
    {   
        prevCar.SetActive(false);
        newCar.SetActive(true);
        sign.SetActive(false);

        Vector3 targetPos = finalDoor.transform.localPosition + new Vector3(0f, 0f, zOffset);
        finalDoor.transform.DOLocalMove(targetPos, duration).SetEase(ease);
    }
}