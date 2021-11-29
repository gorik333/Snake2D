using UnityEngine;
using UnityEngine.UI;

public class BuySkinButton : MonoBehaviour
{
	[SerializeField]
	private Text _priceText;

	[SerializeField]
	private GameObject _priceBackground;

	[SerializeField]
	private GameObject _rewardBackground;

	[SerializeField]
	private GameObject _pickedLook;

	[SerializeField]
	private Button _currButton;

	[SerializeField]
	private int _price;

	[SerializeField]
	private bool _isRewarded;

	private bool _isPurchased;


	void Awake()
	{
		_rewardBackground.SetActive( _isRewarded );
		_priceBackground.SetActive( !_isRewarded );

		_priceText.text = _price.ToString();
	}


	public void UnlockButton()
	{
		_priceBackground.SetActive( false );
		_rewardBackground.SetActive( false );

		_isPurchased = true;
	}


	public void TurnOnPickedLook()
	{
		_pickedLook.SetActive( true );

		_currButton.interactable = false;
	}


	public void TurnOffPickedLook()
	{
		_pickedLook.SetActive( false );

		_currButton.interactable = true;
	}


	public int Price => _price;

	public bool IsPurchased => _isPurchased;

	public bool IsRewarded => _isRewarded;
}
