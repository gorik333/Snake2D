using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIProc : MonoBehaviour
{
    static UIProc _instance;

    public Text m_txtDebug;

    [SerializeField]
    private GameUI m_gameUI;

    public Game m_game;

    public WndMainMenu m_mainMenu;

    public WndGameOver m_wndGameOver;

    public wndGameWin m_wndGameWin;

    public WndRateUS m_wndRateUS;

    public wndShop m_wndShop;

    public wndMessage m_msg;

    public BannerCover m_bannerCover;

    int m_txtDebugLines = 0;

    public static UIProc Instance { get { return _instance; } }


    UIProc()
    {
        _instance = this;
    }


    void Awake()
    {
#if UNITY_EDITOR
        //PlayerPrefs.DeleteAll();
        //Application.logMessageReceived+= HandleLog;
#endif
    }

    void Start()
    {
        bool firstLaunch = Stats.Instance.HasParam("init_ads_delay") == false;

        DebugOut("FIRST launch: " + firstLaunch);

        DebugOut("DAY: " + Stats.Instance.DaysFromStart());

        DebugOut("Anti CHEAT: " + TimeAntiCheat.Instance.GetDebugStr());

        //YaMetrica.Init( firstLaunch );
        Analytics.Initialize();

        FBMan.Instance.InitFB();

        Invoke("InitADS", Stats.Instance.GetParamFloat("init_ads_delay", 1f));
        Stats.Instance.SetParam("init_ads_delay", 0.1f);

        ShowMainMenu(true);

        InvokeRepeating("WriteFPS", 1f, 0.5f);
    }


    void InitADS()
    {
        bool showBanner = true;

        ADs.Init( Stats.Instance.adsRemoved ? ADType.Rewarded : ADType.All, showBanner );
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string str = "log " + type.ToString()[0];
        if (type == LogType.Exception)
            str += "x";
        DebugOut(str + " " + logString);
    }

    public void ShowMainMenu(bool show)
    {
        if (!show && !m_mainMenu.IsVisible)
            return;

        m_mainMenu.Show(show);
    }

    public void ShowGameOver(bool canContinue, int scores)
    {
        m_wndGameOver.Show(canContinue, scores, 0f);
    }

    public void ShowWin(int level)
    {
        m_gameUI.TurnOffCoinCounter();

        m_wndGameWin.Show(level);
    }

    public void ShowMessage(string text)
    {
        m_msg.Show(text);
    }

    public bool ShowRateUS(bool force = false)
    {
        if (m_wndRateUS.CanShow(force))
        {
            m_wndRateUS.Show(true);
            return true;
        }
        return false;
    }

    public void onClickContinue()
    {
        Analytics.SendEvent("CLICK_CONTINUE", "level", Stats.Instance.level);

        YaMetrica.SendEvent("video_ads_watch", "ad_type", "rewarded", "placement", "continue");

        bool showRew =
        ADs.ShowRewardedVideo((int obj) =>
       {
           Analytics.SendEvent("REWARDED_OK");

           m_game.Continue();

           m_wndGameOver.Hide();
       });

        if (showRew)
            m_wndGameOver.DisableContinue();
        else
            Analytics.SendEvent("NO_REWARDED", "level", Stats.Instance.level);
    }

    public void onClickPlayMain()
    {
        ShowMainMenu(false);
    }

    public void onClickNext()
    {
        Stats.Instance.level++;

        m_wndGameWin.Hide();

        m_game.Restart();

        ShowMainMenu(true);
    }

    public void onClickRestart()
    {
        m_wndGameOver.Hide();

        ShowMainMenu(true);

        m_game.Restart();
    }

    public void onClickShop()
    {
        m_wndShop.Show();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        //DebugOut( "- FOCUS: " + hasFocus.ToString() );
    }

    private void WriteFPS()
    {
        float fps = 1.0f / Time.deltaTime;
        //DebugOut( "[FPS]: " + (int)fps );
    }

    public void DebugOut(string str)
    {
    }


    public bool IsMainMenuShown => m_mainMenu.isActiveAndEnabled;

	public GameUI GameUI { get => m_gameUI; set => m_gameUI =  value ; }
}
