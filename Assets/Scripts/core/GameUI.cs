using UnityEngine;
using UnityEngine.UI;


public class GameUI : MonoBehaviour
{
	[SerializeField]
	private Image m_imgBar;
	public Text m_txtLevelL;
	public Text m_txtLevelR;

	[SerializeField]
	private GameObject m_coinCountPanel;

	[SerializeField]
	private Text m_coinCountText;

	[SerializeField]
	private Camera UI_Camera;


	void Start()
	{
		UpdateBar( 0, Stats.Instance.level );
	}


	public void UpdateBar( float progress, int currLevel )
	{
		m_imgBar.fillAmount = progress;

		m_txtLevelL.text = currLevel.ToString();
		m_txtLevelR.text = ( currLevel + 1 ).ToString();
	}

	public void SetBarColor( Color col )
	{
		Image[] images = m_imgBar.GetComponentsInChildren<Image>();

		foreach (Image img in images)
			img.color = col;

		m_imgBar.color = col;
	}


	public Vector3 GetCoinCounterPosition()
	{
		//return UI_Camera.ScreenToWorldPoint( m_coinCountPanel.transform.position );
		return m_coinCountPanel.transform.position;
	}


	public void ChangeCounterText( int newValue, float delay = 0f, float time = 0.2f )
	{
		int startValue= int.Parse( m_coinCountText.text );

		EffectProc effectProc = EffectProc.GetProc( m_coinCountText.gameObject );

		effectProc.AddEffect( new Effect( delay, false ) ); 
		effectProc.AddEffect( new FxScore( startValue, newValue, time ) );
	}


	public void TurnOnCoinCounter()
	{
		m_coinCountText.text = Stats.Instance.coins.ToString();

		m_coinCountPanel.SetActive( true );
	}


	public void TurnOffCoinCounter()
	{
		m_coinCountPanel.SetActive( false );
	}
}
