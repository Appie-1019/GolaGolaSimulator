using UnityEngine;
using UnityEngine.UI;

public class MobileGameobjectSetter : MonoBehaviour
{
    [Header("Mobile UI Raycast Set")]
    [SerializeField] private Image[] mobileRaycastDisabledImages;
    [Header("Mobile Gameobject Enable/Disable Set")]
    [SerializeField] private GameObject[] mobileEnabledGameObjects;
    [SerializeField] private GameObject[] mobileDisabledGameObjects;
    [Header("Destroy This Gameobject?")]
    [SerializeField] private bool destroyAfterAction = true;

    void Start()
    {
        DisableRaycastForMobile();
        DisableGameobjectForMoblie();
        EnableGameobjectForMoblie();

        if (destroyAfterAction)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void DisableRaycastForMobile()
    {
        if (mobileRaycastDisabledImages == null || mobileRaycastDisabledImages.Length == 0) return;

        foreach (Image img in mobileRaycastDisabledImages)
        {
            if (img != null) img.raycastTarget = !DataManager.isMobile;
        }
    }

    private void DisableGameobjectForMoblie()
    {
        if (mobileDisabledGameObjects == null || mobileDisabledGameObjects.Length == 0) return;

        foreach (GameObject gameObject in mobileDisabledGameObjects)
        {
            if (gameObject != null) gameObject.SetActive(!DataManager.isMobile);
        }
    }

    private void EnableGameobjectForMoblie()
    {
        if (mobileEnabledGameObjects == null || mobileEnabledGameObjects.Length == 0) return;

        foreach (GameObject gameObject in mobileEnabledGameObjects)
        {
            if (gameObject != null) gameObject.SetActive(DataManager.isMobile);
        }
    }
}
