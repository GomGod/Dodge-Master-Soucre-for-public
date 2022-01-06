using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ErrorWindowManager : MonoBehaviour
{
    public static ErrorWindowManager Instance;
    
    [SerializeField] private GameObject errWindow;
    [SerializeField] private TextMeshProUGUI errMsg;

    private void Start()
    {
        Instance = this;
    }

    public enum ErrorType
    {
        AccountError,
        NetworkError,
        PurchaseError,
        RankLoadError,
        RestoreError
    };

    
    
    public void PopErrorWindow(ErrorType eType)
    {
        errWindow.gameObject.SetActive(true);
        errMsg.text = eType switch
        {
            ErrorType.AccountError => GlobalConst.ErrorMsgAccount,
            ErrorType.NetworkError => GlobalConst.ErrorMsgNetworkErr,
            ErrorType.PurchaseError => GlobalConst.ErrorMsgPurchase,
            ErrorType.RankLoadError => GlobalConst.ErrorMsgRankingLoadFail,
            ErrorType.RestoreError => GlobalConst.ErrorMsgRestoreFail,
            _ => throw new ArgumentOutOfRangeException(nameof(eType), eType, null)
        };
    }
}
