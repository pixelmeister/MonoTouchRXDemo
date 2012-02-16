#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RxUtilitiesForMTouch;
using System.Diagnostics;

#endregion

namespace RXDemoApp
{
//	public class PointFObservable : IObservable<PointF>
//	{
//	    private readonly IScheduler _scheduler;
//	    private PointF Value { get; set; }
//	
//	    public PointFObservable(IScheduler scheduler)
//	    {
//	        _scheduler = scheduler;
//	    }
//
//	    public IDisposable Subscribe(IObserver<PointF> observer)
//	    {
//	        return _scheduler.Schedule(() =>
//	        {
//	            var point = default(PointF);
//	
//	            try
//	            {
//	                point = Value;
//	            }
//				catch (Exception ex)
//	            {
//	                observer.OnError(ex);
//	                return;
//	            }
//	
//	            observer.OnNext(point);
//	            observer.OnCompleted();
//	        });
//	    }
//	}

	public class TimeFliesView : UIView
	{
		private readonly IObservable<PointF> _currentTouchPointObserver;
		private readonly Subject<PointF> _currentTouchPointSubject;

		public TimeFliesView(RectangleF rect) : base(rect)
		{
			UserInteractionEnabled = true;
			_currentTouchPointSubject = new Subject<PointF>();
			_currentTouchPointObserver = _currentTouchPointSubject.AsObservable();
			var wholeCoverageView = new UIControl(rect) {UserInteractionEnabled = true};
			AddSubview(wholeCoverageView); // add one covering the whole view so we get hits
			BecomeFirstResponder();
			Reactive("Time flies like an arrow");
		}

		public IObservable<PointF> CurrentTouchObservable
		{
			get { return _currentTouchPointObserver; }
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			// Move relative to the original touch point 
			var touch = (touches.AnyObject as UITouch);

			if (touch != null)
			{
				var pt = (touches.AnyObject as UITouch).LocationInView(this);
				var position = Frame.Location;
				position.X += pt.X;
				position.Y += pt.Y;

				_currentTouchPointSubject.OnNext(position);
			}
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);
			var touch = (touches.AnyObject as UITouch);

			if (touch != null)
			{
				// Move relative to the original touch point 
				var pt = (touches.AnyObject as UITouch).LocationInView(this);
				var position = Frame.Location;
				position.X += pt.X;
				position.Y += pt.Y;

				_currentTouchPointSubject.OnNext(position);
			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			var touch = (touches.AnyObject as UITouch);

			if (touch != null)
			{
				var pt = (touches.AnyObject as UITouch).LocationInView(this);
				var position = Frame.Location;
				position.X += pt.X;
				position.Y += pt.Y;

				_currentTouchPointSubject.OnNext(position);
			}
		}

		public RectangleF GetCenter(float width, float height)
		{
			var rect = new RectangleF((Frame.Width/2) - (width/2), (Frame.Height/2) - (height/2), width, height);
			return rect;
		}

		private void Reactive(string msg)
		{
			var message = msg.ToCharArray();

			var uiDispatcher = new UIScheduler();

			for (var i = 0; i < message.Length; i++)
			{
				var label = new UILabel
				            	{
				            		Text = message[i].ToString(CultureInfo.InvariantCulture),
				            		Frame = GetCenter(32, 32),
				            		BackgroundColor = UIColor.Clear,
				            		TextAlignment = UITextAlignment.Center,
				            		TextColor = UIColor.White,
				            		Font = UIFont.FromName("courier", 48)
				            	};

				var closure = i;

				CurrentTouchObservable
					.Delay(TimeSpan.FromSeconds(0.07*i), uiDispatcher)
					.Subscribe(e => UpdateLabelPosition(label, e, closure));

				label.UserInteractionEnabled = true;
				AddSubview(label);
			}
		}

		private void UpdateLabelPosition(UILabel label, PointF pos, int closure)
		{
			label.Frame = new RectangleF(pos.X + closure*25 - 5, pos.Y + 15, label.Frame.Width, label.Frame.Height);
		}
	}
}
