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
			UIApplication.Main(args, "", "AppDelegate"); // must pass your application delegate's registered class name here
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
			var window = new UIWindow(UIScreen.MainScreen.Bounds);

			_tabController = new TabControllerContainer();
			window.AddSubview(_tabController.View);

			return window;
		}
	}
}