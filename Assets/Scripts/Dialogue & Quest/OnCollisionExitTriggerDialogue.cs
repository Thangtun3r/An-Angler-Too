using UnityEngine;
using Yarn.Unity;

public class OnCollisionExitTriggerDialogue : MonoBehaviour
{
    
    private DialogueRunner dialogueRunner;
    [SerializeField] private string node;
    [SerializeField] private string triggerTag;
    [SerializeField] private int attempts;
    [SerializeField] private int attemptsCounter = 0;
    void Awake()
    {
        dialogueRunner = GameObject.FindFirstObjectByType<DialogueRunner>();
    }

    private void HandleExit(GameObject other)
    {
        if (!other.CompareTag(triggerTag))
            return;

        attemptsCounter++;
    }

    private void OnTriggerExit(Collider other)
    {
        HandleExit(other.gameObject);
    }

    private void OnCollisionExit(Collision other)
    {
        //Debug.Log("collided");
        HandleExit(other.gameObject);
    }

    void Update()
    {
        if (attemptsCounter >= attempts)
        {
            dialogueRunner.StartDialogue(node);
            attemptsCounter = 0;
        }
    }
}
