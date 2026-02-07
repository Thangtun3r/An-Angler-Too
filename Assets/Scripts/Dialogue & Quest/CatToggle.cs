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
        if (catLay != null)
        {
            catLay.SetActive(true);
        }
        else
        {
            Debug.LogWarning("CatToggle: No GameObject found with tag 'KittyLay'");
        }

        catOnRug = GameObject.FindGameObjectWithTag("KittyOnRug");
        if (catOnRug != null)
        {
            catOnRug.SetActive(false);
        }
        else
        {
            Debug.LogWarning("CatToggle: No GameObject found with tag 'KittyOnRug'");
        }
    }

    [YarnCommand("toggleCat")]
    public void ToggleCat()
    {
        if (catLay == null || catOnHand == null)
        {
            Debug.LogWarning("CatToggle: toggleCat called but references are missing");
            return;
        }

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