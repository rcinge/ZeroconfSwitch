#if __IOS__
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using Network;

using Zeroconf;
using ObjCRuntime;

namespace ZeroconfSwitch
{
    static class ZeroconfNetServiceBrowser
    {
        static internal Task<IReadOnlyList<IZeroconfHost>> ResolveAsync(string protocol,
                                                      TimeSpan scanTime = default(TimeSpan),
                                                      int retries = 2,
                                                      int retryDelayMilliseconds = 2000,
                                                      Action<IZeroconfHost> callback = null,
                                                      CancellationToken cancellationToken = default(CancellationToken),
                                                      System.Net.NetworkInformation.NetworkInterface[] netInterfacesToSendRequestOn = null)
        {
            if (string.IsNullOrWhiteSpace(protocol))
                throw new ArgumentNullException(nameof(protocol));

            return ResolveAsync(new[] { protocol },
                    scanTime,
                    retries,
                    retryDelayMilliseconds, callback, cancellationToken, netInterfacesToSendRequestOn);
        }

        static internal async Task<IReadOnlyList<IZeroconfHost>> ResolveAsync(IEnumerable<string> protocols,
                                                                    TimeSpan scanTime = default(TimeSpan),
                                                                    int retries = 2,
                                                                    int retryDelayMilliseconds = 2000,
                                                                    Action<IZeroconfHost> callback = null,
                                                                    CancellationToken cancellationToken = default(CancellationToken),
                                                                    System.Net.NetworkInformation.NetworkInterface[] netInterfacesToSendRequestOn = null)
        {
            if (retries <= 0) throw new ArgumentOutOfRangeException(nameof(retries));
            if (retryDelayMilliseconds <= 0) throw new ArgumentOutOfRangeException(nameof(retryDelayMilliseconds));
            if (scanTime == default(TimeSpan))
                scanTime = TimeSpan.FromSeconds(2);

            var options = new ResolveOptions(protocols)
            {
                Retries = retries,
                RetryDelay = TimeSpan.FromMilliseconds(retryDelayMilliseconds),
                ScanTime = scanTime
            };

            return await ResolveAsync(options, callback, cancellationToken, netInterfacesToSendRequestOn).ConfigureAwait(false);
        }

        static internal async Task<IReadOnlyList<IZeroconfHost>> ResolveAsync(ResolveOptions options,
                                                            Action<IZeroconfHost> callback = null,
                                                            CancellationToken cancellationToken = default(CancellationToken),
                                                            System.Net.NetworkInformation.NetworkInterface[] netInterfacesToSendRequestOn = null)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (netInterfacesToSendRequestOn != null)
            {
                throw new NotImplementedException($"iOS NSNetServiceBrowser/NSNetService does not support per-network interface requests");
            }

            List<IZeroconfHost> combinedResultList = new List<IZeroconfHost>();

            // Seems you must reuse the one BonjourBrowser (which is really an NSNetServiceBrowser)... multiple instances do not play well together

            BonjourBrowser bonjourBrowser = new BonjourBrowser();

            foreach (var protocol in options.Protocols)
            {
                ResolveOptions perProtocolBrowseOption = new ResolveOptions(protocol)
                {
                    AllowOverlappedQueries = options.AllowOverlappedQueries,
                    Retries = options.Retries,
                    RetryDelay = options.RetryDelay,
                    ScanQueryType = options.ScanQueryType,
                    ScanTime = options.ScanTime,
                };
                bonjourBrowser.SetResolveOptions(perProtocolBrowseOption, callback, cancellationToken, netInterfacesToSendRequestOn);

                bonjourBrowser.StartServiceSearch();

                await Task.Delay(options.ScanTime, cancellationToken).ConfigureAwait(false);

                bonjourBrowser.StopServiceSearch();

                // Simpleminded callback implementation
                var results = bonjourBrowser.ReturnZeroconfHostResults();
                foreach (var result in results)
                {
                    if (callback != null)
                    {
                        callback(result);
                    }
                }

                combinedResultList.AddRange(results);
            }

            return combinedResultList;
        }

        static internal async Task<ILookup<string, string>> BrowseDomainsAsync(List<string> browseDomainProtocolList, TimeSpan scanTime = default(TimeSpan),
                                                              int retries = 2,
                                                              int retryDelayMilliseconds = 2000,
                                                              Action<string, string> callback = null,
                                                              CancellationToken cancellationToken = default(CancellationToken),
                                                              System.Net.NetworkInformation.NetworkInterface[] netInterfacesToSendRequestOn = null)

        {
            if (retries <= 0) throw new ArgumentOutOfRangeException(nameof(retries));
            if (retryDelayMilliseconds <= 0) throw new ArgumentOutOfRangeException(nameof(retryDelayMilliseconds));
            if (scanTime == default(TimeSpan))
                scanTime = TimeSpan.FromSeconds(2);

            var options = new BrowseDomainsOptions
            {
                Retries = retries,
                RetryDelay = TimeSpan.FromMilliseconds(retryDelayMilliseconds),
                ScanTime = scanTime
            };

            return await BrowseDomainsAsync(browseDomainProtocolList, options, callback, cancellationToken, netInterfacesToSendRequestOn).ConfigureAwait(false);
        }

        static internal async Task<ILookup<string, string>> BrowseDomainsAsync(List<string> browseDomainProtocolList, BrowseDomainsOptions options,
                                                                     Action<string, string> callback = null,
                                                                     CancellationToken cancellationToken = default(CancellationToken),
                                                                     System.Net.NetworkInformation.NetworkInterface[] netInterfacesToSendRequestOn = null)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (netInterfacesToSendRequestOn != null)
            {
                throw new NotImplementedException($"iOS NSNetServiceBrowser/NSNetService does not support per-network interface requests");
            }

            ResolveOptions resolveOptions = new ResolveOptions(browseDomainProtocolList);
            var zeroconfResults = await ResolveAsync(resolveOptions, callback: null, cancellationToken, netInterfacesToSendRequestOn);

            List<IntermediateResult> resultsList = new List<IntermediateResult>();
            foreach (var host in zeroconfResults)
            {
                foreach (var service in host.Services)
                {
                    foreach (var ipAddr in host.IPAddresses)
                    {
                        IntermediateResult b = new IntermediateResult();
                        b.ServiceNameAndDomain = service.Key;
                        b.HostIPAndService = $"{ipAddr}: {BonjourBrowser.GetServiceType(service.Value.Name, includeTcpUdpDelimiter: false)}";

                        resultsList.Add(b);

                        // Simpleminded callback implementation
                        if (callback != null)
                        {
                            callback(service.Key, ipAddr);
                        }
                    }
                }
            }

            ILookup<string, string> results = resultsList.ToLookup(k => k.ServiceNameAndDomain, h => h.HostIPAndService);
            return results;
        }

        class IntermediateResult
        {
            public string ServiceNameAndDomain;
            public string HostIPAndService;
        }
    }
}
#endif