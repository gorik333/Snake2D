

#include <ctime>
#include <sys/sysctl.h>
#include <mach/mach_time.h>
#include "TimeAntiCheat_ios.h"

extern "C"
{

	long TimeAntiCheat_SystemUptime()
	{
		struct timeval boottime;
		size_t len = sizeof(boottime);
		
		int mib[2] = { CTL_KERN, KERN_BOOTTIME };
		
		if( sysctl(mib, 2, &boottime, &len, NULL, 0) < 0 )
		{
			return -1.0;
		}
		
		time_t bsec = boottime.tv_sec;
		time_t csec = time(NULL);
		
		time_t diff= csec - bsec;
		
		return (int)diff;
	}

}
