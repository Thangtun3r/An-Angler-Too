using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yarn.Unity;

public class DialogueOnInteract : MonoBehaviour, IPlayerInteraction
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] string node;

    public void Highlight()
    {
        
    }

    public void Interact()
    {
        //Run Dialogue runner
        dialogueRunner.StartDialogue(node);
    }
}
