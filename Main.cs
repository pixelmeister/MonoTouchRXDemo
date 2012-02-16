#region

using System.Drawing;
using System.Threading;
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

#endregion

namespace RXDemoApp
{
	public class Application
	{
		private static void Main(string[] args)
		{
			//try
			{
				UIApplication.Main(args, "", "AppDelegate"); // must pass your application delegate's class name here
			}
			//catch(Exception ex)
			{
			//	var ex1 = ex;
			}
		}
	}


	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		private TabControllerContainer _tabController;
		private UIWindow _window;

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			using (var pool = new NSAutoreleasePool())
			{
				  _window = CreateMainWindow();
				  _window.MakeKeyAndVisible();
			}
			
			return true;
		}

		private UIWindow CreateMainWindow()
		{
			// Create the main window and add a big purple label
			var window = new UIWindow(UIScreen.MainScreen.Bounds);

			_tabController = new TabControllerContainer();
			window.AddSubview(_tabController.View);

			return window;
		}

		// This method is allegedly required in iPhoneOS 3.0
		public override void OnActivated(UIApplication application)
		{
		}
	}
}