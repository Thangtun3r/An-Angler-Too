using UnityEngine;
using Yarn.Unity;

public class CatOnHandDialogue : MonoBehaviour
{
    private bool isTalking;
    private DialogueRunner dialogueRunner;
    [SerializeField] private string node;

    void Awake()
    {
        dialogueRunner = GameObject.FindFirstObjectByType<DialogueRunner>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isTalking)
        {
            dialogueRunner.StartDialogue(node);
        }
    }

    void OnEnable()
    {
        dialogueRunner.onDialogueStart.AddListener(DialogueStart);
        dialogueRunner.onDialogueComplete.AddListener(DialogueEnd);
    }

    void DialogueStart()
    {
        isTalking = true;
    }

    void DialogueEnd()
    {
        isTalking = false;
    }
}
