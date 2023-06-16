using System;
using System.Timers;

using Codice.Client.Common.Threading;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class UnityPlasticTimerBuilder : IPlasticTimerBuilder
    {
        IPlasticTimer IPlasticTimerBuilder.Get(bool bModalMode, ThreadWaiter.TimerTick timerTickDelegate)
        {
            return new UnityPlasticTimer(DEFAULT_TIMER_INTERVAL, timerTickDelegate);
        }

        IPlasticTimer IPlasticTimerBuilder.Get(bool bModalMode, int timerInterval, ThreadWaiter.TimerTick timerTickDelegate)
        {
            return new UnityPlasticTimer(timerInterval, timerTickDelegate);
        }

        const int DEFAULT_TIMER_INTERVAL = 100;
    }

    internal class UnityPlasticTimer : IPlasticTimer
    {
        internal UnityPlasticTimer(int timerInterval, ThreadWaiter.TimerTick timerTickDelegate)
        {
            mTimerInterval = timerInterval;
            mTimerTickDelegate = timerTickDelegate;
        }

        void IPlasticTimer.Start()
        {
            mTimer = new Timer();
            mTimer.Interval = mTimerInterval;
            mTimer.Elapsed += OnTimerTick;

            mTimer.Start();
        }

        void IPlasticTimer.Stop()
        {
            mTimer.Stop();
            mTimer.Elapsed -= OnTimerTick;
            mTimer.Dispose();
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            mTimerTickDelegate();
        }

        Timer mTimer;
        int mTimerInterval;
        ThreadWaiter.TimerTick mTimerTickDelegate;
    }
}
