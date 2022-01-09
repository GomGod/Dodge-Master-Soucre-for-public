using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIComponent;
using UnityEngine;

public class OptionBoard : MonoBehaviour
{
    [SerializeField] private OptionToggle controls;
    [SerializeField] private OptionToggle bgm;
    [SerializeField] private OptionToggle sfx;

    private void Awake()
    {
        controls.SetItems(GlobalConst.Controls);
        bgm.SetItems(GlobalConst.OnOff);
        sfx.SetItems(GlobalConst.OnOff);
    }

    private void OnEnable()
    {
        var udata = GameManager.Instance.GetUserData();
        
        controls.FindItemAndChangeIndex(udata.controls);
        bgm.FindItemAndChangeIndex(udata.bgm);
        sfx.FindItemAndChangeIndex(udata.sfx);
    }

    public void SaveUserdata()
    {
        var udata = GameManager.Instance.GetUserData();
        udata.controls = controls.GetCurrentItem();
        udata.bgm = bgm.GetCurrentItem();
        udata.sfx = sfx.GetCurrentItem();
        GameManager.Instance.SetUserData(udata);
    }
}
