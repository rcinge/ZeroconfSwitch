using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Foundation;
using UIKit;

using Xamarin.Forms;
using ZeroconfSwitchTest.Xam;

namespace ZeroconfSwitchTest.Xamarin.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Foundation.Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Forms.Init();

            window = new UIWindow((RectangleF)UIScreen.MainScreen.Bounds)
            {
                RootViewController = App.GetMainPage().CreateViewController()
            };

            window.MakeKeyAndVisible();

            return true;
        }
    }
}
