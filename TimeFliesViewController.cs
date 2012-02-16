#region

using MonoTouch.UIKit;

#endregion

namespace RXDemoApp
{
	public class TimeFliesViewController : UIViewController
	{
		public TimeFliesViewController()
		{
			var item = new UITabBarItem {Title = @"Time Flies"};
			TabBarItem = item;
			View = new TimeFliesView(View.Frame);
		}
	}
}