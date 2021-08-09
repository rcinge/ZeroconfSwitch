ZeroconfSwitch
==============

# Wrapper for Zeroconf [Zeroconf](http://www.nuget.org/packages/Zeroconf) to make it functional in Xamarin with iOS >= 14.5

iOS 14.5 and above introduced new restrictions on mDNS clients. Low-level socket-based clients (like Zeroconf) are blocked at the iOS library/system call level unless the program has a special com.apple.developer.networking.multicast entitlement

One way to work around this restriction is to use the iOS NSNetServiceBrowser and NSNetService objects; that is what ZeroconfSwitch does.

This project is built as a wrapper to avoid polluting the Zeroconf source code with what is ultimately a Xamarin.iOS issue and workaround.

It is structured as a wrapper so that method calls in non-Xamarin.iOS builds are passed through to Zeroconf without interference from ZeroconfSwitch.

## Installation

Currently, this is released only via source code. I hope to change this to a NuGet package soon.

## Usage 

ZeroconfSwitch exposes the same public methods as Zeroconf; to use ZeroconfSwitch in a target project that currently uses Zeroconf:

1. Remove the Zeroconf NuGet package from your target project
2. Add a reference to the local ZeroconfSwitch project to your target project
3. Change all "using Zeroconf" to "using ZeroconfSwitch", and import the IZeroconfHost object:

```csharp
using ZeroconfSwitch;
using IZeroconfHost = Zeroconf.IZeroconfHost;
```

4. In your Xamarin.iOS project, modify Info.plist to include something like the following:

```
  <key>NSLocalNetworkUsageDescription</key>
  <string>Looking for local mDNS/Bonjour services</string>
  <key>NSBonjourServices</key>
  <array>
    <string>_audioplayer-discovery._tcp</string>
    <string>_http._tcp</string>
    <string>_printer._tcp</string>
    <string>_apple-mobdev2._tcp</string>
  </array>
```

The effect of the above: the first time your app runs, iOS will prompt the user for permission to allow mDNS queries, displaying the <string> value of the key NSLocalNetworkUsageDescription.

The array of mDNS services are the services that your app needs in order to run properly; iOS will only allow mDNS information from those services to reach the app.

4. If your target project uses BrowseAsync(), you need to let ZeroconfSwitch know which services the app needs, like this:

```csharp
// Use the array of NSBonjourServices from Info.plist; in this list, append the domain and terminating period (usually ".local.")
static List<string> BrowseDomainProtocolList = new List<string>()
{
    "_audioplayer-discovery._tcp.local.",
    "_http._tcp.local.",
    "_printer._tcp.local.",
    "_apple-mobdev2._tcp.local.",
};

...

ZeroconfSwitchResolver.SetBrowseDomainProtocols(BrowseDomainProtocolList);

...
call BrowseAsync() as usual...
```

ZeroconfSwitch is only allowed to "browse" whatever is allowed in Info.plist.

## Unimplemented features

ListenForAnnouncementsAsync()
ResolverListener()

## Notes/Implementation Details

The callback functions are based on a simple-minded implementation: they will be called only after each ScanTime interval has expired for each distinct protocol/mDNS service.

Calling BrowseAsync() followed by ResolveAsync() is essentially doing the same work twice: BrowseAsync is simulated using ResolveAsync() with the list of provided BrowseDomainProtocolList contents.

The more protocols/mDNS services you resolve, the longer it takes the library to return: minimumTotalDelayTime = (nProtocols * ScanTime).

## Known bugs

There is no propagation of errors (NetService_ResolveFailure, Browser_NotSearched) from the iOS API to ZeroconfSwitch. You simply get nothing and like it.

This project was created by copying the Zeroconf repository and then deleting the unnecessary things. There are certainly some files and configuration remaining in this project that make sense only for Zeroconf and have not been removed yet.

## Hacking information

ZeroconfSwitch.BonjourBrowser.cs is where the iOS API integration lives
Sockaddr.cs contains the C# definitions of the BSD Socket API structures
ZeroconfSwitchResolver.Async.cs contains the majority of the wrapper code (the #ifdef __IOS__ stuff)

## Credits

This library was made possible through the efforts of the following project:

* [Zeroconf](https://github.com/novotnyllc/Zeroconf) by Claire Novotny
