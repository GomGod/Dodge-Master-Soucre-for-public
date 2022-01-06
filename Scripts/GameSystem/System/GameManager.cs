using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PlayNANOO;
using UnityEngine.Serialization;

[DefaultExecutionOrder(1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    #region PlayNanoo
    public Plugin nanooPlugin;
  
    public void SetUserNickname(string nickname)
    {
        nanooPlugin.AccountNickanmePut(nickname, false,  (status, errorCode, jsonString, values) =>
        {
            Debug.Log(status.Equals(Configure.PN_API_STATE_SUCCESS) ? values["nickname"].ToString() : "Fail");
        });
    }
    private void AccountSignIn()
    {
        nanooPlugin.AccountGuestSignIn((status, errorCode, jsonString, values) =>
        {
            if (status.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                var nickname = values["nickname"].ToString();
                if (nickname is GlobalConst.UnknownNickname or "")
                {
                    SetUserNickname();
                }
                SetAccountStatus(true);
            }
            else
            {
                SetAccountStatus(false);
                ErrorWindowManager.Instance.PopErrorWindow(ErrorWindowManager.ErrorType.AccountError);
            }
        });
    }
    private void RecordRanking(int maxScore)
    {
        nanooPlugin.RankingRecord(GlobalConst.RankingTableID, maxScore, "Score", (state, message, rawData, dictionary) => {
            if(state.Equals(Configure.PN_API_STATE_SUCCESS)) {
                Debug.Log("Success");
            } else {
                ErrorWindowManager.Instance.PopErrorWindow(ErrorWindowManager.ErrorType.NetworkError);
            }
        });
    }

    #endregion
    
    #region UserData
    private UserData _currentUserData;
    private bool _accountLoadFailed;
    
    public void SetUserData(UserData udata)
    {
        _currentUserData = udata; 
        UserDataManager.SaveUserData();
    }

    public UserData GetUserData()
    {
        return _currentUserData;
    }
    #endregion
    
    #region StateControl
    public enum GameStates { Title, Game, Record };
    [SerializeField] private UIController uiController;
    private GameStates _currentGameState;
    private GameStates CurrentGameState
    {
        get => _currentGameState;
        set
        {
            _currentGameState = value;
            uiController.SetUiAlignment(_currentGameState);
        }
    }
    
    public void ReturnToTitle()
    {
        CurrentGameState = GameStates.Title;
    }

    public void SetUserNickname()
    {
        uiController.PresetInputUserName();
    }
    #endregion
    
    #region CharacterControl
    [SerializeField] private Character character;
    private Vector2 _characterPosition;

    public enum MoveDirection
    {
        U, D, R, L, N
    }

    public void TryCharacterMove(MoveDirection direction)
    {
        var currentVector = _characterPosition;

        switch (direction)
        {
            case MoveDirection.U:
                currentVector.y += 1;
                break;
            case MoveDirection.D:
                currentVector.y -= 1;
                break;
            case MoveDirection.R:
                currentVector.x += 1;
                break;
            case MoveDirection.L:
                currentVector.x -= 1;
                break;
            case MoveDirection.N:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        if (!CheckValidCoord((int) currentVector.x, (int) currentVector.y))
        {
            return;
        }

        GlobalSources.Instance.audioCharacterMove.Play();
        _characterPosition = currentVector;
        character.Moving = true;
        character.transform.DOMove(_coordinatesGrid[(int) _characterPosition.y, (int) _characterPosition.x], 0.25f)
            .OnComplete(() => { character.Moving = false; });
    }

    private static bool CheckValidCoord(int x, int y)
    {
        return x is < 6 and > 0 && y is < 6 and > 0;
    }
    public Vector2 GetPositionFromGirdVector(Vector2 vec) => _coordinatesGrid[(int) vec.y, (int) vec.x];
    #endregion
    
    #region GameInitialize
    [SerializeField] private InputManager inputManager;
    [SerializeField] private BoxCollider2D fieldArea;
    private readonly Vector2[,] _coordinatesGrid = new Vector2[7, 7];
    private void Awake()
    {
        Instance = this;
        nanooPlugin = Plugin.GetInstance();
        GetGirdCoord();
        _characterPosition = new Vector2(3, 3);
        inputManager.AddListener(character);
        TryCharacterMove(MoveDirection.N);
        CurrentGameState = GameStates.Title;
        PatternStore.InitPatternStorage();
    }
    private void Start()
    {
        ScreenSetting();
        uiController.SetUiAlignment(CurrentGameState);
        if (!UserDataManager.LoadUserData())
        {
            uiController.PresetInputUserName();
        }
        //account sign in
        AccountSignIn();
    }

    public void SetAccountStatus(bool success) => _accountLoadFailed = !success;

    private void OnPreCull()
    {
        if (Camera.main is not null) GL.Clear(true, true, Camera.main.backgroundColor);
    }

    private void ScreenSetting()
    {
        var mainCamera = Camera.main;
        if (mainCamera is null) return;
        var rect = mainCamera.rect;
        var scaleHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
        var scaleWidth = 1f / scaleHeight;
        if (scaleHeight < 1)
        {
            rect.height = scaleHeight;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) / 2f;
        }
        mainCamera.rect = rect;
        OnPreCull();
    }
    
    private void GetGirdCoord()
    {
        var bounds = fieldArea.bounds;
        var size = bounds.size;
        var height = size.y;
        var width = size.x;
        var wOffset = width * 0.2f;
        var hOffset = height * 0.2f;

        var originPoint = bounds.min;
        originPoint.x -= wOffset * 0.5f;
        originPoint.y -= hOffset * 0.5f;

        for (var i = 0; i < 7; i += 1)
        {
            for (var j = 0; j < 7; j += 1)
            {
                _coordinatesGrid[i, j].x = originPoint.x + j * wOffset;
                _coordinatesGrid[i, j].y = originPoint.y + i * hOffset;
            }
        }
    }
    #endregion
    
    #region Ingame
    [SerializeField] private WarningSystem warningSystem;    
    [SerializeField] private LaserObjectManager laserObjectManager;
    [SerializeField] public Transform objectsContainer;
    
    public readonly PatternStore PatternStore = new();
    
    public bool durPattern;
    private const float MINInterval = 0.6f;
    private int _score;
    private int _lastScore;
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            if (_lastScore < 1)
                uiController.SetScore(Score);

            if (_score % 15 != 0 || _score == 0) return;
            
            uiController.PrintCombo();
            GlobalSources.Instance.audioComboSfx.volume = 0;
            GlobalSources.Instance.audioComboSfx.DOFade(1.0f, 0.5f);
            GlobalSources.Instance.audioComboSfx.Play();
        }
    }
    private IEnumerator GameRoutine()
    {
        while (true)
        {
            var interval = 1.33f - Score / 80f;
            interval = interval < MINInterval ? MINInterval : interval;
            yield return new WaitUntil(()=>!durPattern);
            yield return new WaitForSeconds(interval);
            laserObjectManager.AddRandomPattern();
        }
        // ReSharper disable once IteratorNeverReturns
    }
    public void InitializeGame()
    {
        _lastScore = 0;
        durPattern = false;
        CurrentGameState = GameStates.Game;
        StartCoroutine(GameRoutine());
        laserObjectManager.ClearPatterns();
        laserObjectManager.ActivatePatternProcessor();
        Score = 0;
        GlobalSources.Instance.audioBgm.Play();
        GlobalSources.Instance.audioBgm.DOKill();
        GlobalSources.Instance.audioBgm.DOFade(0.33f, 1f);
    }
    
    public void RequestWarning(IEnumerable<Vector2> posList, float duration)
    {
        foreach (var vec in posList)
        {
            warningSystem.SetWarningTile(vec, duration);
        }
    }
    public void GameOver()
    {
        CurrentGameState = GameStates.Record;
        _lastScore = Score;        
        StopAllCoroutines();
        laserObjectManager.DisablePatternProcessor();
        FadeBGM(0f, 1f);
        uiController.SetScore(_lastScore);
        if (_accountLoadFailed) return;
        
        _currentUserData.maxScore =_currentUserData.maxScore < _lastScore ? _lastScore : _currentUserData.maxScore;
        
        RecordRanking(_currentUserData.maxScore);
        
        _currentUserData.stackedScore += _lastScore;
        UserDataManager.SaveUserData();

        if (!_currentUserData.noAds)
        {
            AdmobAdsManager.Instance.ShowInterstitial();
        }
    }
    #endregion

    #region BgmController

    private void FadeBGM(float target, float duration)
    {
        GlobalSources.Instance.audioBgm.DOFade(target, duration).OnComplete(() =>
        {
            if (target < 0.001f) GlobalSources.Instance.audioBgm.Stop();
            else GlobalSources.Instance.audioBgm.Play();
        });
    }

    #endregion
}
