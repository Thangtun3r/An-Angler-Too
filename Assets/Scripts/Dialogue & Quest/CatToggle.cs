using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class CatToggle : MonoBehaviour
{
    private GameObject catLay;
    private GameObject catOnHand;
    private GameObject catOnRug;

    void Awake()
    {
        catLay = GameObject.FindGameObjectWithTag("KittyLay");
        if (catLay != null)
        {
            catLay.SetActive(true);
        }

        catOnRug = GameObject.FindGameObjectWithTag("KittyOnRug");
        if (catOnRug != null)
        {
            catOnRug.SetActive(false);
        }

        catOnHand = GameObject.FindGameObjectWithTag("KittyOnHand");
        if (catOnHand != null)
        {
            catOnHand.SetActive(false);
        }
    }

    [YarnCommand("toggleCat")]
    public void ToggleCat()
    {
        bool isCatLayActive = catLay.activeSelf;

        catLay.SetActive(!isCatLayActive);
        catOnHand.SetActive(isCatLayActive);
    }

    [YarnCommand("toggleRugCat")]
    public void ToggleRugCat()
    {
        if (catOnRug != null)
        {
            catOnRug.SetActive(true);
        }
        else
        {
            Debug.LogWarning("CatToggle: catOnRug is null");
        }

        if (catOnHand != null)
        {
            Destroy(catOnHand);
        }

        if (catLay != null)
        {
            Destroy(catLay);
        }
    }
}