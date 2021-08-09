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
    public static partial class ZeroconfSwitchResolver
    {
        /// <summary>
        ///     Resolves available ZeroConf services
        /// </summary>
        /// <param name="scanTime">Default is 2 seconds</param>
        /// <param name="cancellationToken"></param>
        /// <param name="protocol"></param>
        /// <param name="retries">If the socket is busy, the number of times the resolver should retry</param>
        /// <param name="retryDelayMilliseconds">The delay time between retries</param>
        /// <param name="callback">Called per record returned as they come in.</param>
        /// <param name="netInterfacesToSendRequestOn">The network interfaces/adapters to use. Use all if null</param>
        /// <returns></returns>
        public static async Task<IReadOnlyList<IZeroconfHost>> ResolveAsync(string protocol,
                                                      TimeSpan scanTime = default(TimeSpan),
                                                      int retries = 2,
                                                      int retryDelayMilliseconds = 2000,
                                                      Action<IZeroconfHost> callback = null,
                                                      CancellationToken cancellationToken = default(CancellationToken),
                                                      System.Net.NetworkInformation.NetworkInterface[] netInterfacesToSendRequestOn = null)
        {
            IReadOnlyList<IZeroconfHost> results = null;

#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 5))
            {
                results = await ZeroconfNetServiceBrowser.ResolveAsync(protocol, scanTime);
            }
            else
            {
                results = await ZeroconfResolver.ResolveAsync(protocol, scanTime);
            }
#else
            results = await ZeroconfResolver.ResolveAsync(protocol, scanTime);
#endif

            return results;
        }

        /// <summary>
        ///     Resolves available ZeroConf services
        /// </summary>
        /// <param name="scanTime">Default is 2 seconds</param>
        /// <param name="cancellationToken"></param>
        /// <param name="protocols"></param>
        /// <param name="retries">If the socket is busy, the number of times the resolver should retry</param>
        /// <param name="retryDelayMilliseconds">The delay time between retries</param>
        /// <param name="callback">Called per record returned as they come in.</param>
        /// <param name="netInterfacesToSendRequestOn">The network interfaces/adapters to use. Use all if null</param>
        /// <returns></returns>
        public static async Task<IReadOnlyList<IZeroconfHost>> ResolveAsync(IEnumerable<string> protocols,
                                              TimeSpan scanTime = default(TimeSpan),
                                              int retries = 2,
                                              int retryDelayMilliseconds = 2000,
                                              Action<IZeroconfHost> callback = null,
                                              CancellationToken cancellationToken = default(CancellationToken),
                                              System.Net.NetworkInformation.NetworkInterface[] netInterfacesToSendRequestOn = null)
        {
            IReadOnlyList<IZeroconfHost> results = null;

#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 5))
            {
                results = await ZeroconfNetServiceBrowser.ResolveAsync(protocols, scanTime);
            }
            else
            {
                results = await ZeroconfResolver.ResolveAsync(protocols, scanTime);
            }
#else
            results = await ZeroconfResolver.ResolveAsync(protocols, scanTime);
#endif

            return results;
        }

        /// <summary>
        ///     Resolves available ZeroConf services
        /// </summary>
        /// <param name="options"></param>
        /// <param name="callback">Called per record returned as they come in.</param>
        /// <param name="netInterfacesToSendRequestOn">The network interfaces/adapters to use. Use all if null</param>
        /// <returns></returns>
        public static async Task<IReadOnlyList<IZeroconfHost>> ResolveAsync(ResolveOptions options,
                                                                            Action<IZeroconfHost> callback = null,
                                                                            CancellationToken cancellationToken = default(CancellationToken),
                                                                            System.Net.NetworkInformation.NetworkInterface[] netInterfacesToSendRequestOn = null)
        {
            IReadOnlyList<IZeroconfHost> results = null;

#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 5))
            {
                results = await ZeroconfNetServiceBrowser.ResolveAsync(options, callback, cancellationToken, netInterfacesToSendRequestOn);
            }
            else
            {
                results = await ZeroconfResolver.ResolveAsync(options, callback, cancellationToken, netInterfacesToSendRequestOn);
            }
#else
            results = await ZeroconfResolver.ResolveAsync(options, callback, cancellationToken, netInterfacesToSendRequestOn);
#endif

            return results;
        }

        // Should be set to the list of allowed protocols from info.plist; entries must include domain including terminating dot (usually ".local.")
        // Used by BrowseDomainAsync only; the hack is that "browsing" is really just ResolveAsync() with the result formatted differently
        static List<string> browseDomainProtocolList = new List<string>();

        public static void SetBrowseDomainProtocols(IEnumerable<string> protocols)
        {
            if (protocols == null) { throw new ArgumentException(nameof(protocols));  }
            browseDomainProtocolList.Clear();

            foreach (var protocol in protocols)
            {
                if (protocol != null)
                {
                    browseDomainProtocolList.Add(protocol);
                }
            }
        }

        /// <summary>
        ///     Returns all available domains with services on them
        /// </summary>
        /// <param name="scanTime">Default is 2 seconds</param>
        /// <param name="cancellationToken"></param>
        /// <param name="retries">If the socket is busy, the number of times the resolver should retry</param>
        /// <param name="retryDelayMilliseconds">The delay time between retries</param>
        /// <param name="callback">Called per record returned as they come in.</param>
        /// <param name="netInterfacesToSendRequestOn">The network interfaces/adapters to use. Use all if null</param>
        /// <returns></returns>
        public static async Task<ILookup<string, string>> BrowseDomainsAsync(TimeSpan scanTime = default(TimeSpan),
                                                                             int retries = 2,
                                                                             int retryDelayMilliseconds = 2000,
                                                                             Action<string, string> callback = null,
                                                                             CancellationToken cancellationToken = default(CancellationToken),
                                                                             System.Net.NetworkInformation.NetworkInterface[] netInterfacesToSendRequestOn = null)
        {
            ILookup<string, string> results = null;

#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 5))
            {
                results = await ZeroconfNetServiceBrowser.BrowseDomainsAsync(browseDomainProtocolList, scanTime, retries, retryDelayMilliseconds, callback, cancellationToken,
                    netInterfacesToSendRequestOn);
            }
            else
            {
                results = await ZeroconfResolver.BrowseDomainsAsync(scanTime, retries, retryDelayMilliseconds, callback, cancellationToken, netInterfacesToSendRequestOn);
            }
#else
            results = await ZeroconfResolver.BrowseDomainsAsync(scanTime, retries, retryDelayMilliseconds, callback, cancellationToken, netInterfacesToSendRequestOn);
#endif

            return results;
        }

        /// <summary>
        ///     Returns all available domains with services on them
        /// </summary>
        /// <param name="options"></param>
        /// <param name="callback">Called per record returned as they come in.</param>
        /// <param name="cancellationToken"></param>
        /// <param name="netInterfacesToSendRequestOn">The network interfaces/adapters to use. Use all if null</param>
        /// <returns></returns>
        public static async Task<ILookup<string, string>> BrowseDomainsAsync(List<string> browseDomainProtocolList, BrowseDomainsOptions options,
                                                                             Action<string, string> callback = null,
                                                                             CancellationToken cancellationToken = default(CancellationToken),
                                                                             System.Net.NetworkInformation.NetworkInterface[] netInterfacesToSendRequestOn = null)
        {
            ILookup<string, string> results = null;

#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 5))
            {
                results = await ZeroconfNetServiceBrowser.BrowseDomainsAsync(browseDomainProtocolList, options, callback, cancellationToken, netInterfacesToSendRequestOn);
            }
            else
            {
                results = await ZeroconfResolver.BrowseDomainsAsync(options, callback, cancellationToken, netInterfacesToSendRequestOn);
            }
#else
            results = await ZeroconfResolver.BrowseDomainsAsync(options, callback, cancellationToken, netInterfacesToSendRequestOn);
#endif

            return results;
        }

        /// <summary>
        /// Listens for mDNS Service Announcements
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task ListenForAnnouncementsAsync(Action<ServiceAnnouncement> callback, CancellationToken cancellationToken)
        {
            Task result = null;

#if __IOS__
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 5))
            {
                // !!! @@@ #1
                result = ZeroconfResolver.ListenForAnnouncementsAsync(callback, cancellationToken);
            }
            else
            {
                result = ZeroconfResolver.ListenForAnnouncementsAsync(callback, cancellationToken);
            }
#else
            result = ZeroconfResolver.ListenForAnnouncementsAsync(callback, cancellationToken);
#endif

            return result;
        }
    }
}
