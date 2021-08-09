using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using ZeroconfSwitch;
using IZeroconfHost = Zeroconf.IZeroconfHost;

namespace ZeroconfSwitchTest.Xam  
{
#pragma warning disable RECS0014 // If all fields, properties and methods members are static, the class can be made static.
    public class App
#pragma warning restore RECS0014 // If all fields, properties and methods members are static, the class can be made static.
    {
        public static Page GetMainPage()
        {
            var output = new Label();

            var resolve = new Button
            {
                Text = "Resolve"
            };
            resolve.Clicked += (s, e) => ResolveOnClicked(output);
            var browse = new Button
            {
                Text = "Browse"
            };
            browse.Clicked += (s, e) => BrowseOnClicked(output);

            

            return new ContentPage
            {
                Content = new ScrollView
                {
                    Content = new StackLayout()
                    {
                        Children =
                    {
                        new Label{Text = "Zeroconf Test"},
                        resolve,
                        browse,
                        output
                    }
                    }
                }
            };
        }

        // See ZeroconfSwitchResolver.Async.cs
        // Use the array of NSBonjourServices from Info.plist; in this list, append the domain and terminating period (usually ".local.")
        static List<string> BrowseDomainProtocolList = new List<string>()
        {
            "_audioplayer-discovery._tcp.local.",
            "_http._tcp.local.",
            "_printer._tcp.local.",
            "_apple-mobdev2._tcp.local.",
        };

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        static async void BrowseOnClicked(Label output)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            ILookup<string, string> responses = null;
            //await Task.Run(async () =>
            //{

            //});

            output.Text += Environment.NewLine;

            // Xamarin iOS only: BrowseDomainsAsync() is not usable without this initialization
            ZeroconfSwitchResolver.SetBrowseDomainProtocols(BrowseDomainProtocolList);

            responses = await ZeroconfSwitchResolver.BrowseDomainsAsync();
            foreach (var service in responses)
            {
                output.Text += (service.Key + Environment.NewLine);

                foreach (var host in service)
                    output.Text += (("\tIP: " + host) + Environment.NewLine);

            }
        }

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        static async void ResolveOnClicked(Label output)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            IReadOnlyList<IZeroconfHost> responses = null;
            //await Task.Run(async () =>
            //{

            //});

            output.Text += Environment.NewLine;

            // Xamarin iOS only: BrowseDomainsAsync() is not usable without this initialization
            ZeroconfSwitchResolver.SetBrowseDomainProtocols(BrowseDomainProtocolList);

            var domains = await ZeroconfSwitchResolver.BrowseDomainsAsync();

            responses = await ZeroconfSwitchResolver.ResolveAsync(domains.Select(g => g.Key));
                

            foreach (var resp in responses)
            {
                output.Text += (resp + Environment.NewLine);
            }
            
        }
    }
}
