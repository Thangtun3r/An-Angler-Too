using UnityEngine;
using Yarn.Unity;

public class CatToggle : MonoBehaviour
{
    public GameObject catLay;
    public GameObject catOnHand;

    [YarnCommand ("toggleCat")]
    public void ToggleCat()
    {
        if (catLay == null || catOnHand == null)
            return;

        bool isCatLayActive = catLay.activeSelf;

        catLay.SetActive(!isCatLayActive);
        catOnHand.SetActive(isCatLayActive);
    }
}