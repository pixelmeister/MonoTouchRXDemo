using System;
using System.Reactive;
using System.Reactive.Concurrency;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Reactive.Disposables;
using System.Diagnostics;
using System.Globalization;

namespace RxUtilitiesForMTouch
{
	public class NSRunloopScheduler : IScheduler
	{
		readonly NSObject _theApp;
		
		static int _schedCount = 0;

#if MACOS
		public NSRunloopScheduler (NSApplication app)
		{
			theApp = app;
		}
#else
		public NSRunloopScheduler (UIApplication app)
		{
			if (app == null) throw new ArgumentNullException("app");
			_theApp = app;
		}
#endif

		public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			//this.Log().Debug("Scheduling on Runloop");
			Action scheduledItem = () => action(this, state);
			_theApp.BeginInvokeOnMainThread(new NSAction(scheduledItem));
			return Disposable.Empty;
		}
		
		// returns abs val non-negative TimeSpan.
		public static TimeSpan AbsValDiff (DateTimeOffset due, DateTimeOffset now)
		{
			if ((due - now) > TimeSpan.Zero)
			{
				return due - now;
			}
			else
			{
				return now - due;
			}
		}
		

		public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			
			var now = Now;
//			var diff = Scheduler.Normalize(dueTime - Now);
			var diff = AbsValDiff(dueTime, now);
			CultureInfo ci = CultureInfo.InvariantCulture;

			
			//Debug.WriteLine ("Due time: " + dueTime.ToString("hh:mm:ss.fffffff", ci) + ", now: " + now.ToString("hh:mm:ss.fffffff", ci));

			if (diff == TimeSpan.Zero)
			{
				//Debug.WriteLine("Sched now!");
				return Schedule(state, action);			
			}
			
			//Debug.WriteLine("Sched Later!");
			return Schedule(state, dueTime - now, action);			
		}

		public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			//this.Log().Debug("Scheduling on Runloop");
			Action scheduledItem = () => action(this, state);
			
			Debug.Assert (dueTime > TimeSpan.Zero);
			var timer = NSTimer.CreateScheduledTimer(dueTime, new NSAction(scheduledItem));
			//Debug.WriteLine ("Due time: " + dueTime);
			return Disposable.Create(timer.Invalidate);
		}


		public DateTimeOffset Now 
		{
			get { return DateTimeOffset.Now; }
		}
	}
}
