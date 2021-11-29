using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Window : MonoBehaviour
{

	public void Show( bool visible= true )
	{
		gameObject.SetActive( visible );

		if( visible )
			onShowWnd();
		else
			onHideWnd();
	}

	public void Hide()
	{
		Show( false );
	}

	public bool IsVisible { get { return gameObject.activeSelf; } }

	public virtual void onShowWnd()
	{
	}

	public virtual void onHideWnd()
	{
	}

}


public class WndGameOver : Window
{

	public GameObject m_restart;
	public GameObject m_continue;
	public GameObject m_noThanks;

	public Text m_txtTimer;

	public Text m_txtScores;
	public Text m_txtBest;

	float m_timeShow;


    void Start()
    {
    }

	public void Show( bool canContinue, int scores, float percentComplete )
	{
		base.Show();

		//m_txtScores.text= scores.ToString();
		EffectProc proc= m_txtScores.GetComponent<EffectProc>();
		proc.AddEffect( new FxScore( 0, scores, 0.4f ) );

		m_txtBest.text= Stats.Instance.bestScores.ToString();

		bool cont= canContinue && ADs.HasRewardedVideo();

		m_restart.SetActive( !cont );
		m_continue.SetActive( cont );

		m_noThanks.SetActive( false );

		Invoke( "ShowNoThanks", 1.4f );

		m_timeShow= 6.5f;

		if( cont == false )
			ADs.ShowInterstitialAuto();
	}

	public void onClickContinueCancel()
	{
		Analytics.SendEvent( "click_CONTINUE_cancel", "level", Stats.Instance.level );

		CancelContinueInt();
	}

	public void DisableContinue()
	{
		m_timeShow= -1f;

		m_restart.SetActive( true );
		m_continue.SetActive( false );
	}

	void CancelContinueInt()
	{
		DisableContinue();

		ADs.ShowInterstitialAuto();
	}

	void ShowNoThanks()
	{
		m_noThanks.SetActive( true );
	}

	void Update()
	{
		if( m_timeShow > 0 )
		{
			m_txtTimer.text= ((int)m_timeShow).ToString();

			m_timeShow-= Time.deltaTime;

			if( m_timeShow < 1f )
			{
				m_timeShow= -1f;

				CancelContinueInt();
			}
		}
	}

}

