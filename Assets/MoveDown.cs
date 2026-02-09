using UnityEngine;
using DG.Tweening;
using Yarn.Unity;


public class MoveDown : MonoBehaviour
{
    public GameObject ogVan;
    public float duration = 0.25f;
    public Ease ease = Ease.OutQuad;

    [YarnCommand("moveDown")]
    public void MoveBack()
    {
        ogVan.transform.DOLocalMoveZ(ogVan.transform.localPosition.z - 19.83f, duration).SetEase(ease);
    }
}