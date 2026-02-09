
using System.Collections;
using UnityEditor.Build;
using UnityEngine;
using Yarn.Unity;

public class OnCollisionExitTriggerDialogue : MonoBehaviour
{

    public bool isFinale = false;
    public GameObject finaleSceneObject;

    
    private DialogueRunner dialogueRunner;
    [SerializeField] private string node;
    [SerializeField] private string triggerTag;
    [SerializeField] private int attempts;
    [SerializeField] private int attemptsCounter = 0;
    [SerializeField] private float waitTime;
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
        HandleExit(other.gameObject);
    }

    void Update()
    {
        if (attemptsCounter >= attempts)
        {
            attemptsCounter = 0;
            StartCoroutine("StartDialogue");
        }
    }

    IEnumerator StartDialogue()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Player playerScript  = player.GetComponent<Player>();
        playerScript.DisablePlayer();

        yield return new WaitForSeconds(waitTime);
        dialogueRunner.StartDialogue(node);
        if (isFinale && finaleSceneObject != null)
        {
            finaleSceneObject.SetActive(true);
        }
    }
}
