using System;
using UnityEngine;
using GoogleMobileAds.Api;

[DefaultExecutionOrder(2)]
public class AdmobAdsManager : MonoBehaviour
{
    public static AdmobAdsManager Instance;
    
    private InterstitialAd _interstitial;
    private BannerView _banner;
    
    private void Start()
    {
        Instance = this;
        MobileAds.Initialize(initStatus => { });
        RequestInterstitial();
        RequestBanner();
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        var adUnitId = GlobalConst.AdBannerAndroidUnitID;
#else
        var adUnitId = "unexpected_platform";
#endif
        _banner = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        var adRequest = new AdRequest.Builder().Build();
        
        _banner.LoadAd(adRequest);
    }

    public void ShowBanner()
    {
        _banner.Show();
    }

    public void HideBanner()
    {
        _banner.Hide();
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        var adUnitId = GlobalConst.AdInterstitialAndroidUnitID;
#else
        var adUnitId = "unexpected_platform";
#endif
        _interstitial = new InterstitialAd(adUnitId);
        
        //ads events
        // Called when an ad is shown.
        this._interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        //
        
        
        var request = new AdRequest.Builder().Build();
        _interstitial.LoadAd(request);
    }

    private static void HandleOnAdOpened(object sender, EventArgs args)
    {
        var udata = GameManager.Instance.GetUserData();
        udata.stackedScore = 0;
        GameManager.Instance.SetUserData(udata);
    }
    
    public void ShowInterstitial()
    {
        if (_interstitial.IsLoaded())
        {
            _interstitial.Show();
        }
        else
        {
            RequestInterstitial();
        }
    }
}