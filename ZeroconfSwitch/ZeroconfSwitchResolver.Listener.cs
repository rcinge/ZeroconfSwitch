using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Zeroconf;

#if __IOS__
using UIKit;
#endif

namespace ZeroconfSwitch
{
    static partial class ZeroconfSwitchResolver
    {
        public static ZeroconfResolver.ResolverListener CreateListener(IEnumerable<string> protocols,
                                                 int queryInterval = 4000,
                                                 int pingsUntilRemove = 2,
                                                 TimeSpan scanTime = default(TimeSpan),
                                                 int retries = 2,
                                                 int retryDelayMilliseconds = 2000)
        {
            ZeroconfResolver.ResolverListener result;

#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 5))
            {
                // !!! @@@ #2
                result = ZeroconfResolver.CreateListener(protocols, queryInterval, pingsUntilRemove, scanTime, retries, retryDelayMilliseconds);
            }
            else
            {
                result = ZeroconfResolver.CreateListener(protocols, queryInterval, pingsUntilRemove, scanTime, retries, retryDelayMilliseconds);
            }
#else
            result = ZeroconfResolver.CreateListener(protocols, queryInterval, pingsUntilRemove, scanTime, retries, retryDelayMilliseconds);
#endif

            return result;
        }

        public static ZeroconfResolver.ResolverListener CreateListener(string protocol,
                                                         int queryInterval = 4000,
                                                         int pingsUntilRemove = 2,
                                                         TimeSpan scanTime = default(TimeSpan),
                                                         int retries = 2,
                                                         int retryDelayMilliseconds = 2000)
        {
            ZeroconfResolver.ResolverListener result;

#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 5))
            {
                // !!! @@@ #3
                result = ZeroconfResolver.CreateListener(protocol, queryInterval, pingsUntilRemove, scanTime, retries, retryDelayMilliseconds);
            }
            else
            {
                result = ZeroconfResolver.CreateListener(protocol, queryInterval, pingsUntilRemove, scanTime, retries, retryDelayMilliseconds);
            }
#else
            result = ZeroconfResolver.CreateListener(protocol, queryInterval, pingsUntilRemove, scanTime, retries, retryDelayMilliseconds);
#endif

            return result;
        }
    }
}
