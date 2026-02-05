using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem circularWind;

    [Header("Circular Wind")]
    [Tooltip("From 0 to 1 (E.g: 0.5 will make it 50% spawn rate).\nDetermines how frequent the circular wind is called. Check every second.")]
    [SerializeField] private float spawnPercentage = 0.2f;

    private void Start()
    {
        circularWind = transform.Find("Circular Wind").GetComponent<ParticleSystem>();

        StartCoroutine(CheckForParticleSpawn());
    }

    private IEnumerator CheckForParticleSpawn()
    {
        while (true)
        {
            if (Random.value < spawnPercentage)
            {
                circularWind.Emit(1);
            }

            yield return new WaitForSeconds(1);
        }
    }
}