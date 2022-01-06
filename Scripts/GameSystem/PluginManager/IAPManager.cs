using UnityEngine;
using UnityEngine.Purchasing;


[DefaultExecutionOrder(2)]
public class IAPManager : MonoBehaviour
{
    [SerializeField] private GameObject IAPButtonAdsRemove;

    private void Start()
    {
        var noAds = GameManager.Instance.GetUserData().noAds;
        IAPButtonAdsRemove.SetActive(!noAds);
        if(noAds)
            AdmobAdsManager.Instance.HideBanner();
        else
            AdmobAdsManager.Instance.ShowBanner();
    }

    public void IAPRemoveAds()
    {
        var udata = GameManager.Instance.GetUserData();
        udata.noAds = true;
        GameManager.Instance.SetUserData(udata);
        IAPButtonAdsRemove.SetActive(false);
        AdmobAdsManager.Instance.HideBanner();
    }

    public void PurchaseFailed()
    {
        ErrorWindowManager.Instance.PopErrorWindow(ErrorWindowManager.ErrorType.PurchaseError);
    }
}
