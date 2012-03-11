#region

using System.Drawing;
using MonoTouch.UIKit;

#endregion

namespace RXDemoApp
{
	public class TabControllerContainer : UITabBarController
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var controller1 = new TimeFliesViewController();
			var controller2 = new SecondViewController();

			SetViewControllers(new UIViewController[] {controller1, controller2}, true);
			SelectedViewController = controller1;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
		}
	}

	public class SecondViewController : UIViewController
	{
		public SecondViewController()
		{
			// Tab bar item 
			var item = new UITabBarItem();
			item.Title = "RX Demo - Time Flies";
			TabBarItem = item;
		}

		public override void ViewDidLoad()
		{
			var label = new UILabel();
			label.Text = "RX Demo - no second demo yet";
			label.Frame = new RectangleF(100, 100, 100, 100);
			View.AddSubview(label);

			base.ViewDidLoad();
		}
	}
}