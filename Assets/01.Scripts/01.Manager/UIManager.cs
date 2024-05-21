using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using System.Collections;
using System;
using DG.Tweening;

public enum eUIType
{
    Lobby,
    InGame,
}

public class UIManager : MonoBehaviour
{
    public static UIManager Inst;

    [SerializeField] RectTransform _canvasRect;
    public RectTransform CanvasRect => _canvasRect;

    [SerializeField] PopupController _popupController = null;
    public PopupController Popup => _popupController;

    [Header("GlobalPanel")]
    [SerializeField] GlobalPanel _global = null;

    [Header("LobbyPanel")]
    [SerializeField] LobbyUI _lobby = null;

    [Header("GamePanel")]
    [SerializeField] GameUI _game = null;
    public GameUI Game => _game;

    eUIType _currUIType = eUIType.Lobby;
    public eUIType UIType => _currUIType;
    public eGameMode _mode;

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        Inst = this;
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (_lobby)
        {
            _lobby.Init();
            _lobby.Enter();
        }

        if (_popupController)
            _popupController.Init();

        if (_global)
            _global.Init();
    }

    public void GameStart(int num)
    {
        _currUIType = eUIType.InGame;

        var mwdm = (MyWealthDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyWealthDataModel);
        if (mwdm.SteminaCnt == 5)
            mwdm.SteminaDate = DateTime.UtcNow;

        mwdm.SteminaCnt--;

        var mdm = (MissionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MissionDataModel);
        foreach (int key in mdm.GetKeys(eMissionKind.Stemina))
            mdm.MyMission.SetAddMission(key, 1);

        _lobby.Exit();
        _game.Enter(_mode);
        _global.SetActiveLifeAddBtn(false);

        GameManager.Inst.GameStart(num);
    }

    public void GameExit()
    {
        _currUIType = eUIType.Lobby;

        _lobby.Enter();
        _game.Exit();

        _global.SetActiveLifeAddBtn(true);

        GameManager.Inst.GameExit();

        StopAllCoroutines();

        //float showPer = UnityEngine.Random.Range(0f, 1f);
        //if (showPer < Data.Inst.GetADPercentData(eADPercentType.gotolobby))
        //    AdsManager.Inst.ShowInterstitialAD(null);
    }

    public void ClickGameModeBtn(eGameMode mode, int num)
    {
        _popupController.ExitAll();

        _mode = mode;

        GameStart(num);
    }

    public void ShowFadePanel(Action onComplete)
    {
        _global.ShowFade(onComplete);
    }

    public void SetActiveLoading(bool isActive)
    {
        _global.SetActiveLoading(isActive);
    }

    public void SetAddItemPanel(Dictionary<eProductType, Dictionary<int, int>> addItems)
    {
        _global.SetAddItemPanel(addItems);
    }

    public void SetUpdateLobbyUI()
    {
        _lobby.SetUpdateCurrentPanel();
    }
}
