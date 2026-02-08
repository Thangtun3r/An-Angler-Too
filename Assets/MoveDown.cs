using UnityEngine;
using DG.Tweening;
using Yarn.Unity;


public class MoveDown : MonoBehaviour
{
    public class MoveZByTween : MonoBehaviour
    {
        public float duration = 0.25f;
        public Ease ease = Ease.OutQuad;

        [YarnCommand("moveDown")]
        public void MoveBack()
        {
            transform.DOLocalMoveZ(transform.localPosition.z - 19.83f, duration)
                    .SetEase(ease);
        }
    }
}