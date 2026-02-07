using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DetectIfCatNearRug : MonoBehaviour
{
    DialogueRunner diagRunner;

    void Awake()
    {
        diagRunner = GameObject.FindFirstObjectByType<DialogueRunner>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            diagRunner.VariableStorage.SetValue("$OnRug", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            diagRunner.VariableStorage.SetValue("$OnRug", false);
        }
    }
}
