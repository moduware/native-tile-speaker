using Foundation;
using Moduware.Platform.Tile.iOS;
using Serilog;
using UIKit;

namespace Moduware.Tile.Speaker.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Enabling logging
            Log.Logger = new LoggerConfiguration()
            .WriteTo.NSLog()
            .CreateLogger();

            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            // If you have defined a root view controller, set it here:
            Window.RootViewController = new RootViewController();

            // make the window visible
            Window.MakeKeyAndVisible();

            return true;
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // custom stuff here using different properties of the url passed in
            var viewController = (TileViewController)Window.RootViewController;
            viewController.OnQueryRecieved(url.AbsoluteString);

            return true;
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
            var viewController = (TileViewController)Window.RootViewController;
            viewController.OnResumeActions();
        }
    }
}


