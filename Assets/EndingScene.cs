using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class EndingScene : MonoBehaviour
{
    public Image targetImage;

    void Start()
    {
        if (targetImage != null)
        {
            targetImage.enabled = false;
        }
    }

    [YarnCommand("endGame")]
    public void EndGame()
    {
        if (targetImage != null)
        {
            targetImage.enabled = true;
        }
    }
}
