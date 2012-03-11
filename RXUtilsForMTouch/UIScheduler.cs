using System;
using System.Reactive;
using System.Reactive.Concurrency;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Reactive.Disposables;
using System.Diagnostics;
using System.Globalization;

// NOTE: thanks to Paul Betts for the original version of this

namespace RxUtilitiesForMTouch
{
	public class UIScheduler : IScheduler
	{
		readonly NSObject _theApp;
		
		public UIScheduler ()
		{
			_theApp = UIApplication.SharedApplication;
		}

		public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			Action scheduledItem = () => action(this, state);
			_theApp.BeginInvokeOnMainThread(new NSAction(scheduledItem));
			return Disposable.Empty;
		}

		public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			
			var now = Now;
			var diff = Scheduler.Normalize(dueTime - Now);

			if (diff == TimeSpan.Zero)
			{
				return Schedule(state, action);			
			}
			
			return Schedule(state, dueTime - now, action);			
		}

		public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			Action scheduledItem = () => action(this, state);			
			Debug.Assert (dueTime > TimeSpan.Zero);
			var timer = NSTimer.CreateScheduledTimer(dueTime, new NSAction(scheduledItem));
			return Disposable.Create(timer.Invalidate);
		}

		public DateTimeOffset Now 
		{
			get { return DateTimeOffset.Now; }
		}
	}
}
