
#import <StoreKit/StoreKit.h>


extern "C"
{
	
	bool NativeRateInApp_Show()
	{
		if( [[[UIDevice currentDevice] systemVersion] floatValue] >= 10.3 )
		{
			[SKStoreReviewController requestReview];
			return true;
		}
		return false;
	}
	
}
