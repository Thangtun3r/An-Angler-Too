using UnityEngine;
using Yarn.Unity;

public class DialogueOnInteract : MonoBehaviour, IPlayerInteraction
{
    private DialogueRunner dialogueRunner;
    [SerializeField] private string node;
    [SerializeField] private PromptZoom highlightZoom;

    void Awake()
    {
        dialogueRunner = GameObject.FindFirstObjectByType<DialogueRunner>();
        node = this.gameObject.name;
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
