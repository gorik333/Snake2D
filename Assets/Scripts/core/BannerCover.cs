using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BannerCover : MonoBehaviour
{
	public Canvas m_canvas;

	Image m_img;
    

    void Start()
    {
		m_img= GetComponent<Image>();

		//Debug.Log( "DELEGAT NEEDED" );
        //MoPubManager.OnAdLoadedEvent+= BannerAdLoadedEvent;
		//BannerAdLoadedEvent( "", 50 );

		MaxSdkCallbacks.OnBannerAdLoadedEvent += OnBannerAdLoadedEvent;
		MaxSdkCallbacks.OnBannerAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
    }

	void OnBannerAdLoadFailedEvent( string adUnitId, int err )
	{
		HideBanner();
	}

	void OnBannerAdLoadedEvent( string adUnitId )
	{
		BannerAdLoadedEvent( adUnitId, CalculateMaxBannerHeightPx());
	}

	void BannerAdLoadedEvent(string adUnitId, float height)
	{
		CanvasScaler cs = m_canvas.GetComponent<CanvasScaler>();

		float sheight = Screen.height;

		float referenceResolution = cs.referenceResolution.y;

		float aspect = referenceResolution / sheight;

		float bannerHeight = 0;

#if UNITY_IOS
		bannerHeight = 50f * aspect * 3f;
		height = 50f;
#endif

#if UNITY_ANDROID

		bannerHeight = aspect * CalcBannerHeight(height);
#endif

		UIProc.Instance.DebugOut("COVER: " + sheight + " / " + referenceResolution + " / " + aspect + " / h= " + bannerHeight + " / BH= " + height);
		//UIProc.Instance.DebugOut( "CALC HEIGHT: " + CalcBannerHeight(50) );
		UIProc.Instance.DebugOut("DPI: " + Screen.dpi + " / " + GetScreenSize());


		m_img.enabled = true;

		float clampedH = Mathf.Clamp(bannerHeight, 0, 3 * height);

		if (CameraScaler.isIPhoneXR)
			clampedH += 30f;

		if (CameraScaler.isIPhoneX)
			clampedH += 80f;

		if (CameraScaler.isIPhoneXSMax)
			clampedH += 107f;

		m_img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clampedH);
	}

	public void HideBanner()
	{
		if (m_img)
			m_img.enabled = false;
	}


	public float CalcBannerHeight(float height)
	{
		return Mathf.RoundToInt(height * Screen.dpi / 160f);
	}

	int CalculateMaxBannerHeightPx()
	{
		if( Screen.height >= 720 * Mathf.RoundToInt(Screen.dpi / 160) && GetScreenSize() >= 7f )
		{
			return 90;
		}
		else
		{
			return 50;
		}
	}

	float GetScreenSize()
	{
		float pixels = Mathf.Sqrt((Screen.width * Screen.width) + (Screen.height * Screen.height));
		float inch = pixels / Screen.dpi;
		return inch;
	}
}
