using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yarn.Unity;
using UnityEngine;
using DG.Tweening;
using System;

public class DialogueOnInteract : MonoBehaviour, IPlayerInteraction
{
    private DialogueRunner dialogueRunner;
    [SerializeField] private string node;
    [SerializeField] private PromptZoom highlightZoom;

    void Awake()
    {
        dialogueRunner = GameObject.FindFirstObjectByType<DialogueRunner>();
        if (node != null)
        {
            node = this.gameObject.name;
        }
    }
    public void Highlight()
    {
        highlightZoom.ZoomIn();
    }

    public void Unhighlight()
    {
        highlightZoom.ZoomOut();
    }

    public void Interact()
    {
        //Run Dialogue runner
        dialogueRunner.StartDialogue(node);
    }
}
