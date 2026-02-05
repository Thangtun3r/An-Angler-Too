using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class SceneCommandRouter : MonoBehaviour
{

    [YarnCommand("changeScene")]
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("attempted change scene");
    }
}