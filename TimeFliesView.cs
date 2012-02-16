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

		//public PointFObservable CurrentTouch { get; set; }

		public TimeFliesView(RectangleF rect) : base(rect)
		{
			UserInteractionEnabled = true;
			_currentTouchPointSubject = new Subject<PointF>();
			_currentTouchPointObserver = _currentTouchPointSubject.AsObservable();

			// var observablePointF = new Observable<PointF> ((System.IObserver<PointF> observer) => _currentTouchPointSubject.Subscribe (observer));

			// var obs = (IObservable<PointF>)_currentTouchPointSubject;

			// CurrentTouch = ((System.IObservable<PointF>)_currentTouchPointSubject).AsObservable();

			var wholeCoverageView = new UIControl(rect) {UserInteractionEnabled = true};
			AddSubview(wholeCoverageView); // add one covering the whole voew so we get hits
			BecomeFirstResponder();
			Reactive("Time flies like an arrow");

			// Register<ScheduledItem<DateTimeOffset, Action>>();
			
			// var a = new ScheduledItem<DateTimeOffset, Action>(null, null, null, new DateTimeOffset(), null);
			
			// Register<System.Reactive.ImmutableList<IObserver<PointF>>>();

			// start an async operation using RX
			// var o = Observable.Start(() => { Console.WriteLine("Calculating..."); Thread.Sleep(4000); Console.WriteLine("Done."); });
			// o.Subscribe();   // subscribe 
					
//			var source = Observable.Interval(TimeSpan.FromSeconds(1)).Timestamp();
//			var delayed = source.Delay(TimeSpan.FromSeconds(4)).Timestamp();
// 
//			delayed.Subscribe(item => Debug.WriteLine("Datum {0}, Create on {1:ss} and propogate on {2:ss}", item.Value.Value, item.Value.Timestamp, item.Timestamp));
			
//			var oneNumberEveryFifteenSeconds = Observable.Interval(TimeSpan.FromSeconds(15));
//
//			// Instant echo
//			oneNumberEveryFifteenSeconds.Subscribe(num => Debug.WriteLine(num));
//
//			// One second delay
//			oneNumberEveryFifteenSeconds.Delay(TimeSpan.FromSeconds(2)).Subscribe(num => Debug.WriteLine("...{0}...", num));
//
//			// Two second delay
//			oneNumberEveryFifteenSeconds.Delay(TimeSpan.FromSeconds(5)).Subscribe(num => Debug.WriteLine("......{0}......", num));
		}

		public IObservable<PointF> CurrentTouch
		{
			get { return _currentTouchPointObserver; }
		}

//		public static void Register<T>()
//		{
//#pragma warning disable 219
//			var i = 0;
//#pragma warning restore 219
//// ReSharper disable ConditionIsAlwaysTrueOrFalse
//			if (new List<T>() != null) i++;
//// ReSharper disable RedundantAssignment
//			if (new T[0] != null) i++;
//// ReSharper restore RedundantAssignment
//// ReSharper restore ConditionIsAlwaysTrueOrFalse
//		}

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
			//Console.WriteLine("Touch move(begin)");
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
			//Console.WriteLine("Touch move :" + _count++);
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
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
			//Console.WriteLine("Touch move (end)");
		}

		public RectangleF GetCenter(float width, float height)
		{
			var rect = new RectangleF((Frame.Width/2) - (width/2), (Frame.Height/2) - (height/2), width, height);
			return rect;
		}

		private void Reactive(string msg)
		{
			var message = msg.ToCharArray();

			var uiDispatcher = new NSRunloopScheduler(UIApplication.SharedApplication);

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

				CurrentTouch
					.Delay(TimeSpan.FromSeconds(0.07*i), uiDispatcher)
					//.Sample(TimeSpan.FromMilliseconds(5))
					//.ObserveOn(uiDispatcher)
					.Subscribe(e => UpdateLabelPosition(label, e, closure));

				label.UserInteractionEnabled = true;

				AddSubview(label);
			}
		}

		private void UpdateLabelPosition(UILabel label, PointF pos, int closure)
		{
			label.Frame = new RectangleF(pos.X + closure*20 - 5, pos.Y + 15, label.Frame.Width, label.Frame.Height);
			//Debug.WriteLine("Update label frame: " + label.Frame);
		}
	}
}

#if FSHARP


		
open System.Windows
open System.Windows.Controls
open System.Windows.Media 

type MyWindow() as this =
	inherit Window()   

	let WIDTH = 20.0
	let canvas = new Canvas(Width=800.0, Height=400.0, Background = Brushes.White)
	let chars =
		" F# reacts to events!"
		|> Seq.map (fun c ->
			new TextBlock(Width=WIDTH, Height=30.0, FontSize=20.0, Text=string c,
						  Foreground=Brushes.Black, Background=Brushes.White))
		|> Seq.toArray
	do
		this.Content <- canvas
		this.Title <- "MyWPFWindow"
		this.SizeToContent <- SizeToContent.WidthAndHeight
		for tb in chars do
			canvas.Children.Add(tb) |> ignore 

		this.MouseMove
		|> Observable.map (fun ea -> ea.GetPosition(this))
		//|> Observable.filter (fun p -> p.X < 300.0)
		|> Observable.add (fun p ->
			async {
				for i in 0..chars.Length-1 do
					do! Async.Sleep(90)
					Canvas.SetTop(chars.[i], p.Y)
					Canvas.SetLeft(chars.[i], p.X + float i*WIDTH)
			} |> Async.StartImmediate
		)


#endif