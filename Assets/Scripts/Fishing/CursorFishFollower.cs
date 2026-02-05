using UnityEngine;
using UnityEngine.UI;

public class CursorFollower : MonoBehaviour
{
    public static CursorFollower Instance;
    private Image img;

    void Awake() 
    { 
        Instance = this;
        img = GetComponent<Image>();
        img.raycastTarget = false;
        SetIcon(null);
    }

    void Update()
    {
        if (img.enabled) transform.position = Input.mousePosition;
    }

    public void SetIcon(Sprite s)
    {
        img.sprite = s;
        img.enabled = s != null;
    }
}