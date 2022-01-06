using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject titleMainScreenPos;
    [SerializeField] private GameObject titleInGameScreenPos;
    [SerializeField] private GameObject screenTouchDetectionButton;
    [SerializeField] private GameObject textTouchScreen;
    [SerializeField] private TextMeshProUGUI scoreTmp;

    [SerializeField] private GameObject mainTitleSet;
    [SerializeField] private GameObject ingameSet;
    [SerializeField] private GameObject gameOverSet;
    [SerializeField] private Transform gameOverScorePos;

    [SerializeField] private Transform ingameScorePos;
    
    private readonly Vector3 _scaleMainTitle = new(1.75f, 1.75f, 1.75f);
    private readonly Vector3 _scaleInGameTitle = new(0.75f, 0.75f, 0.75f);


    public void SetUiAlignment(GameManager.GameStates states)
    {

        switch (states)
        {
            case GameManager.GameStates.Title:
                PresetMainTitle();
                break;
            case GameManager.GameStates.Game:
                PresetInGame();
                break;
            case GameManager.GameStates.Record:
                PresetGameOver();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(states), states, null);
        }
    }

    
    private void PresetMainTitle()
    {
        mainTitleSet.SetActive(true);
        ingameSet.SetActive(false);
        gameOverSet.SetActive(false);
        
        title.transform.DOMove(titleMainScreenPos.transform.position, 0.5f);
        title.transform.DOScale(_scaleMainTitle, 0.5f);
        textTouchScreen.gameObject.SetActive(true);
        screenTouchDetectionButton.gameObject.SetActive(true);
        scoreTmp.gameObject.SetActive(false);
        
    }

    private void PresetInGame()
    {
        mainTitleSet.SetActive(false);
        ingameSet.SetActive(true);
        gameOverSet.SetActive(false);
        
        title.transform.DOMove(titleInGameScreenPos.transform.position, 0.5f);
        title.transform.DOScale(_scaleInGameTitle, 0.5f);
        textTouchScreen.gameObject.SetActive(false);
        screenTouchDetectionButton.gameObject.SetActive(false);
        scoreTmp.gameObject.SetActive(true);
        scoreTmp.gameObject.transform.DOMove(ingameScorePos.position, 0f);
        scoreTmp.gameObject.transform.DOScale(_scoreSizeOrigin, 0f);
        gameOverSet.SetActive(false);
    }

    private readonly Vector3 _scoreSizeOrigin = new(1f, 1f, 1f);
    private readonly Vector3 _gameOverScoreScale = new(1.5f, 1.5f, 1.5f);
    
    private void PresetGameOver()
    {
        mainTitleSet.SetActive(false);
        ingameSet.SetActive(false);
        gameOverSet.SetActive(true);
        
        scoreTmp.transform.DOMove(gameOverScorePos.position, 0.5f);
        scoreTmp.gameObject.transform.DOScale(_gameOverScoreScale, 0.5f);
    }

    [SerializeField] GameObject uidInputWindow;

    public void PresetInputUserName()
    {
        uidInputWindow.SetActive(true);
    }

    public void SetUserId(string uid)
    {
        UserDataManager.SetNewUserData(uid);
    }

    public void SetScore(float score)
    {
        scoreTmp.text = $"Score : {score}";
    }


    private int _comboOrder = 0;
    [SerializeField] private Transform cReadyPos;
    [SerializeField] private Transform cWaitPos;
    [SerializeField] private Transform cOutPos;
    [SerializeField] private List<GameObject> comboImages;
    [SerializeField] private Ease comboEase;
    [SerializeField] private Ease comboOutEase;

    public void PrintCombo()
    {
        var targetImage = comboImages[_comboOrder];
        targetImage.transform.DOMove(cReadyPos.position, 0);
        StartCoroutine(ComboFX(targetImage));

        _comboOrder += 1;
        if (_comboOrder >= comboImages.Count)
            _comboOrder = 0;
    }

    private IEnumerator ComboFX(GameObject targetImage)
    {
        targetImage.transform.DOMove(cWaitPos.position, 0.7f).SetEase(comboEase).OnComplete(() =>
        {
            targetImage.transform.DOMove(cOutPos.position, 1.25f).SetEase(comboOutEase);
        });

        yield return 0;
    }
}
