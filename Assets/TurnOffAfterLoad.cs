using UnityEngine;

public class TurnOffAfterLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }
}
