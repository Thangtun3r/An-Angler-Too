using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class BobYarnCommands : MonoBehaviour
{
    [Header("Wheel")]
    [SerializeField] private GameObject starringWheel;

    [Header("Fly Up")]
    [SerializeField] private Vector3 flyUpOffset = new Vector3(0f, 5f, 0f);
    [SerializeField] private float flyUpDuration = 2f;
    [SerializeField] private Vector3 flyUpRotateAxis = new Vector3(0f, 1f, 0f);
    [SerializeField] private float flyUpRotateDegrees = 360f;

    [Header("Fly Away")]
    [SerializeField] private Transform flyAwayTarget;
    [SerializeField] private float flyAwayDuration = 2f;

    private void Start()
    {
        if (starringWheel != null)
        {
            // Keep it hidden until the player fulfills the quest.
            starringWheel.SetActive(false);
        }
    }

    [YarnCommand("bobFlyAway")]
    public void BobFlyAway()
    {
        StopAllCoroutines();
        if (starringWheel != null)
        {
            starringWheel.SetActive(true);
        }
        StartCoroutine(FlyAwayRoutine());
    }

    private IEnumerator FlyAwayRoutine()
    {
        Vector3 start = transform.position;
        Vector3 upTarget = start + flyUpOffset;
        Quaternion startRot = transform.rotation;
        Quaternion upRot = startRot * Quaternion.AngleAxis(flyUpRotateDegrees, flyUpRotateAxis.normalized);
        float t = 0f;

        float upDuration = Mathf.Max(0.01f, flyUpDuration);
        while (t < 1f)
        {
            t += Time.deltaTime / upDuration;
            transform.position = Vector3.Lerp(start, upTarget, t);
            transform.rotation = Quaternion.Slerp(startRot, upRot, t);
            yield return null;
        }

        if (flyAwayTarget == null)
        {
            yield break;
        }

        Vector3 lookDir = flyAwayTarget.position - transform.position;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
        }

        Vector3 awayStart = transform.position;
        Vector3 awayTarget = flyAwayTarget.position;
        t = 0f;

        float awayDuration = Mathf.Max(0.01f, flyAwayDuration);
        while (t < 1f)
        {
            t += Time.deltaTime / awayDuration;
            transform.position = Vector3.Lerp(awayStart, awayTarget, t);
            yield return null;
        }
    }
}
