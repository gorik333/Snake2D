
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif



public class InAppItem
{
	public bool consumable;
	public string product_id;
	public string name;
	public string currency;
	public string loc_price;
}


#if UNITY_PURCHASING

public class InAppPurchase : MonoBehaviour, IStoreListener
{

	public static event System.Action OnPurchaseFailedEvent = delegate { };
	public static event System.Action OnRestorePurchaseFailedEvent = delegate { };

	public static event System.Action<string> OnPurchaseSuccessEvent = delegate { };
	public static event System.Action<string> OnRestorePurchaseSuccessEvent = delegate { };

	public static event System.Action<string,string,float,string,string> OnPurchaseSuccessEventDetailed = delegate { };

	static InAppPurchase _instance= null;

	public static InAppPurchase Instance
	{
		get
		{
			//if( _instance == null )
			//	_instance= new InAppPurchase();
			return _instance;
		}
	}

	InAppPurchase()
	{
		_instance= this;
	}

	/// ------------------------------------------------------


	List<InAppItem> m_items= new List<InAppItem>();


	private static IStoreController m_StoreController;          // The Unity Purchasing system.
	private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.


	/// ------------------------------------------------------


	public void AddItem( string name, string item_id, bool consumable, string def_price )
	{
		InAppItem item= new InAppItem();

		item.name= name;
		item.product_id= item_id;
		item.consumable= consumable;
		item.loc_price= def_price;

		m_items.Add( item );
	}


	public bool BuyProduct( string name )
	{
		InAppItem item= GetItemByName( name );

		if( item != null )
		{
			//#if UNITY_EDITOR
			//			OnPurchaseFailedEvent();
			//			return true;
			//#endif

			BuyProductID( item.product_id );
			return true;
		}
		return false;
	}

	public string GetItemPrice( string name )
	{
		InAppItem item= GetItemByName( name );
		if( item != null )
			return item.loc_price;
		return "";
	}


	InAppItem GetItemByID( string id )
	{
		foreach(InAppItem item in m_items)
		{
			if( item.product_id == id )
				return item;
		}
		return null;
	}

	InAppItem GetItemByName( string name )
	{
		foreach(InAppItem item in m_items)
		{
			if( item.name == name )
				return item;
		}
		return null;
	}

	/// ------------------------------------------------------


	public void Init() 
	{
		if( IsInitialized() )
			return;

		var builder = ConfigurationBuilder.Instance( StandardPurchasingModule.Instance() );

		foreach(InAppItem item in m_items)
		{
			builder.AddProduct( item.product_id, item.consumable ? ProductType.Consumable : ProductType.NonConsumable );
		}

		UnityPurchasing.Initialize( this, builder );
	}


	private bool IsInitialized()
	{
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}


	void BuyProductID( string productId )
	{
		// If Purchasing has been initialized ...
		if( IsInitialized() )
		{
			// ... look up the Product reference with the general product identifier and the Purchasing 
			// system's products collection.
			Product product = m_StoreController.products.WithID( productId );

			// If the look up found a product for this device's store and that product is ready to be sold ... 
			if( product != null && product.availableToPurchase )
			{
				Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
				// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
				// asynchronously.
				m_StoreController.InitiatePurchase( product );
			}
			// Otherwise ...
			else
			{
				// ... report the product look-up failure situation  
				//Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");

				OnPurchaseFailedEvent.Invoke();
			}
		}
		// Otherwise ...
		else
		{
			// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
			// retrying initiailization.
			//Debug.Log("BuyProductID FAIL. Not initialized.");

			OnPurchaseFailedEvent.Invoke();
		}
	}


	// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
	// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
	public void RestorePurchases()
	{
		// If Purchasing has not yet been set up ...
		if( !IsInitialized() )
		{
			OnRestorePurchaseFailedEvent.Invoke();

			// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
			Debug.Log("RestorePurchases FAIL. Not initialized.");
			return;
		}

		// If we are running on an Apple device ... 
		if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer )
		{
			// ... begin restoring purchases
			Debug.Log( "RestorePurchases started ..." );

			// Fetch the Apple store-specific subsystem.
			IAppleExtensions apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

			// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
			// the Action below, and ProcessPurchase if there are previously purchased products to restore.
			apple.RestoreTransactions((result) =>
			{
				// The first phase of restoration. If no more responses are received on ProcessPurchase then 
				// no purchases are available to be restored.
				Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");

				Invoke( "RestoreFailInvoke", 9f );
			});
		}
		// Otherwise ...
		else
		{
			// We are not running on an Apple device. No work is necessary to restore purchases.
			Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);

			OnRestorePurchaseFailedEvent.Invoke();
		}
	}

	void RestoreFailInvoke()
	{
		OnRestorePurchaseFailedEvent.Invoke();
	}

	//  
	// --- IStoreListener
	//

	public void OnInitialized( IStoreController controller, IExtensionProvider extensions )
	{
		// Purchasing has succeeded initializing. Collect our Purchasing references.
		//Debug.Log("OnInitialized: PASS");

		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;

		foreach(InAppItem item in m_items)
		{
			var product = m_StoreController.products.WithStoreSpecificID( item.product_id );

			Debug.Log( "InApps - Inited! -> " + item.product_id + " // " + product.ToString() );

			if( product != null )
			{
				item.currency= product.metadata.isoCurrencyCode;
				item.loc_price = product.metadata.localizedPriceString;

				if( product.metadata.isoCurrencyCode == "RUB" )
				{
					item.loc_price = item.loc_price.Replace( ",00", "" );
					item.loc_price = item.loc_price.Replace( ".00", "" );
				}
			}
		}
	}


	public void OnInitializeFailed( InitializationFailureReason error )
	{
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
		Debug.Log("InApps - OnInitializeFailed InitializationFailureReason:" + error);
	}


	public PurchaseProcessingResult ProcessPurchase( PurchaseEventArgs args ) 
	{
		CancelInvoke( "RestoreFailInvoke" );

		InAppItem item= GetItemByID( args.purchasedProduct.definition.id );

		if( item != null )
		{
			if( args.purchasedProduct.hasReceipt )
			{
				if( ValidatePurchase( args.purchasedProduct.receipt ) )
				{
					float price= (float)decimal.ToDouble( args.purchasedProduct.metadata.localizedPrice );

					OnPurchaseSuccessEventDetailed( item.name, item.product_id, price, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.transactionID );

					OnPurchaseSuccessEvent( item.name );
				}
				else
					OnPurchaseFailedEvent.Invoke();
			}		
		}

		// Return a flag indicating whether this product has completely been received, or if the application needs 
		// to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
		// saving purchased products to the cloud, and when that save is delayed. 
		return PurchaseProcessingResult.Complete;
	}


	public void OnPurchaseFailed( Product product, PurchaseFailureReason failureReason )
	{
		// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
		// this reason with the user to guide their troubleshooting actions.
		//Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

		OnPurchaseFailedEvent.Invoke();
	}



	bool ValidatePurchase( string receipt )
	{
		// Unity IAP's validation logic is only included on these platforms.

#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
		// Prepare the validator with the secrets we prepared in the Editor
		// obfuscation window.
		var validator = new CrossPlatformValidator( GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier );

		try
		{
			// On Google Play, result has a single product ID.
			// On Apple stores, receipts contain multiple products.
			var result = validator.Validate( receipt );
			// For informational purposes, we list the receipt(s)
			Debug.Log( "Receipt is valid. Contents:" );
			foreach( IPurchaseReceipt productReceipt in result )
			{
				Debug.Log( productReceipt.productID );
				Debug.Log( productReceipt.purchaseDate );
				Debug.Log( productReceipt.transactionID );
			}
		}
		catch( IAPSecurityException )
		{
			Debug.Log( "Invalid receipt, not unlocking content" );
			return false;
		}
#endif
		return true;
	}


}


#else


public class InAppPurchase : MonoBehaviour
{

	public static event System.Action OnPurchaseFailedEvent = delegate { };
	public static event System.Action OnRestorePurchaseFailedEvent = delegate { };

	public static event System.Action<string> OnPurchaseSuccessEvent = delegate { };
	public static event System.Action<string> OnRestorePurchaseSuccessEvent = delegate { };

	public static event System.Action<string,string,float,string,string> OnPurchaseSuccessEventDetailed = delegate { };

	static InAppPurchase _instance= null;

	public static InAppPurchase Instance
	{
		get
		{
			if( _instance == null )
				_instance= new InAppPurchase();
			return _instance;
		}
	}

	/// ------------------------------------------------------


	public void Init() {}

	public void AddItem( string name, string item_id, bool consumable, string def_price ) {}

	public bool BuyProduct( string name ) { return false; }

	public string GetItemPrice( string name ) { return name; }

	public void RestorePurchases() {}

}


#endif