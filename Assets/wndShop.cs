using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class wndShop : Window
{
	const string PURCHASED_SKIN_NAME = "PurchasedSkin";

	[SerializeField] Text _coinsText;

	[SerializeField] GameObject _buttonsCore;


	public override void onShowWnd()
	{
		Stats.Instance.SetTimer( "shop_show_sign", 0 );

		UpdateCoinsText();

		for(int i= 0; GetBuyBtn(i) != null; ++i)
		{
			BuySkinButton btn= GetBuyBtn(i);

			if( ItemUnlocked(i) )
				btn.UnlockButton();
		}

		GetBuyBtn( Stats.Instance.skin ).TurnOnPickedLook();
	}

	private void UpdateCoinsText()
	{
		_coinsText.text = Stats.Instance.coins.ToString();
	}

	public bool HasItemToBuy()
	{
		for(int i= 0; GetBuyBtn(i) != null; ++i)
		{
			BuySkinButton btn= GetBuyBtn(i);

			if( Stats.Instance.coins >= btn.Price && btn.IsRewarded == false && btn.IsPurchased == false )
				return true;
		}
		return false;
	}

	public void onSelectItem( int index )
	{
		BuySkinButton btn = GetBuyBtn( index );
		BuySkinButton btnCurr = GetBuyBtn( Stats.Instance.skin );

		if( btn.IsRewarded && btn.IsPurchased == false )
		{
			ADs.ShowRewardedVideo( ( int obj ) =>
			{
				Stats.Instance.SetParam( $"{PURCHASED_SKIN_NAME}{index}", index );

				btn.UnlockButton();
				btn.TurnOnPickedLook();
				btnCurr.TurnOffPickedLook();

				Stats.Instance.skin = index;

				Game.Instance.RespawnSnake();
			} );
		}
		else
		{
			if( !btn.IsPurchased )
			{
				if( Stats.Instance.SpendCoins( btn.Price ) )
				{
					Stats.Instance.SetParam( $"{PURCHASED_SKIN_NAME}{index}", index );

					btn.UnlockButton();
					btn.TurnOnPickedLook();
					btnCurr.TurnOffPickedLook();

					Stats.Instance.skin = index;

					Vibro.Medium();

					Game.Instance.RespawnSnake();
				}
				else
				{
					EffectProc proc= EffectProc.GetProc( _coinsText.gameObject );

					proc.AddEffect( new FxVibro(0,0.01f) );
					proc.AddEffect( new FxRotateZ(   0, 20f, 0.1f, false ) );
					proc.AddEffect( new FxRotateZ( 20f,-20f, 0.2f, false ) );
					proc.AddEffect( new FxRotateZ(-20f, 10f, 0.1f, false ) );
					proc.AddEffect( new FxRotateZ( 10f,  0f, 0.1f, false ) );
					proc.AddEffect( new FxVibro(1,0.01f) );
				}
			}
			else
			{
				btn.TurnOnPickedLook();
				btnCurr.TurnOffPickedLook();

				Stats.Instance.skin = index;

				Vibro.Light();

				Game.Instance.RespawnSnake();
			}
		}

		UpdateCoinsText();
	}


	private BuySkinButton GetBuyBtn( int index )
	{
		if( index >= _buttonsCore.transform.childCount )
			return null;

		Transform t= _buttonsCore.transform.GetChild( index );

		return t.GetComponent<BuySkinButton>();
	}

	bool ItemUnlocked( int i )
	{
		return Stats.Instance.HasParam( $"{PURCHASED_SKIN_NAME}{i}" ) || i == 0;
	}

}
