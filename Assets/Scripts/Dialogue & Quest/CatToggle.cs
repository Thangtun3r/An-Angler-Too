using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class CatToggle : MonoBehaviour
{
    private GameObject catLay;
    public GameObject catOnHand;
    private GameObject catOnRug;

    void Awake()
    {
        catLay = GameObject.FindGameObjectWithTag("KittyLay");
        catLay.SetActive(true);

        catOnRug = GameObject.FindGameObjectWithTag("KittyOnRug");
        catOnRug.SetActive(false);
    }

    [YarnCommand ("toggleCat")]
    public void ToggleCat()
    {
        bool isCatLayActive = catLay.activeSelf;

        catLay.SetActive(!isCatLayActive);
        catOnHand.SetActive(isCatLayActive);
    }

    [YarnCommand ("toggleRugCat")]
    public void ToggleRugCat()
    {
        catOnRug.SetActive(true);
        Destroy(catOnHand);
        Destroy(catLay);
    }
}