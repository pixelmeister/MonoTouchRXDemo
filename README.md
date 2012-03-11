MonoTouch RX Demo
=============

From my[blog post][1] on this at:

A little about RX
------------

Microsoft's Reactive Extensions, referred to as RX, are a fascinating and powerful way to build async software. It's based on concepts from LINQ, functional programming and the observer pattern. Google "reactive extensions" and you'll find tons of material to learn from. Check out my blog page here for a number of videos on it.

You can use RX with Windows Forms, WPF, WP7, and Silverlight. Alas, RX is only proved in dll form from Microsoft.  This poses problems when you are trying to use it with MonoTouch as the RX framework dll's were built targeting the Microsoft system dlls.  While MonoTouch is close, it's not always close enough to work.  I did an experiment using Mono.Cecil to retarget the libraries and fix up some issues, but it looked live a never ending sink of time as news issues would pop up as RX changed.

Getting it to run on MonoTouch with mono-reactive
------------

Luckily two new things have happend that look to allow RX on MonoTouch, completing the circle of cross-platform abilities.  The first is a github project by Atsushi Eno called mono-reactive. It is a open source reimplementation of RX for Mono and potentially MonoTouch/MonoDroid.  I decided to see if we could get it up and running on MonoTouch. The first thing that was needed was a UI dispatcher to handle updating on the UI thread form RX threads.  I started with one that Paul Betts had on his ReactiveUI project on github and tweaked it a bit more to work with mono-reactive. I decided to bring up the RX standard "Time Flies Like An Arrow" demo, which I believe was originally done in Javascript here.  Alas, my first attempt did not work as there were some issues with the Delay() method in mono-reactive. I posted an issue on the github site and Atsushi was speedy in fixing it (thanks!),

Note, it seems to work ok (see the code for issus on throttling), but I think using the dispatch queue calls in grand central might work better. Alas, I don't think the dispatch queue calls that allow a delay, are currently bound in monotouch yet (note I need to submit a bug on that). Also, I posted it with the code from the revision of mono-reactive that I know works. Atsushi has made many more fixes and you should pick them up before doing more work on it.

I am currently using mono-reactive in apps I am writing with MonoTouch right now. Note, it has some rough edges, but you can find them pretty quickly by testing the RX portion of your code cross-platform over on windows. I also have an small event bus that uses RX for loose couple and it saves a lot of work in MonoTouchfor me (I'll post it soon too). Again, I'd like to thank Atsushi for his work. It's a fair amount of very non-trivial code and I appreciate his efforts.  The general issue is the same as Mono has.  I.e. how to keep up to date with changes Microsoft makes. Ideally, MS might open source RX, but that's unlikely to happen. The good news is that something almost as good has happened.

The next approach...
------------

Microsoft just recently released the latest beta of RX 2.0 and the released it as a portable library (well, the portable parts that is). Portable libraries are a wonderful new thing that has been in beta with VS 2010, is included standard in VS2011 and is soon to be a part of Mono/Touch/Droid. Portable libraries allow you to write .NET code that targets a fairly large subset of the Winforms/WPF/WP7/Silverlight platforms, build the library once as a dll and use that dll with all those frameworks. The key then is to split frameworks that you build into portable and non-portable parts.

What MS has done with the latest RX 2.0 beta is to create a portable version that splits the framework into two parts, one that is a portable library that can run on any .NET platform that supports portable libraries and a set of non-portable dlls that implement the platform specific threading and ui dispatching calls.  What this means is that once Mono/Touch/Droid supports portable libraries, that the mono-reactive part can become a very small library just containing the platform specific parts. Thanks MS and the RX team for doing this! 

[1]: http://pillowonsoftware.blogspot.com/2012/03/reactive-extensions-and-monotouch.html