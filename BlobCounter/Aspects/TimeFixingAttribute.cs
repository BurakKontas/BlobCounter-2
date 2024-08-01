using MethodDecorator.Fody.Interfaces;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BlobCounter.Helpers;

namespace BlobCounter.Aspects
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TimeFixingAttribute : Attribute, IMethodDecorator
    {
        private int _time = 0;
        private Stopwatch _stopwatch;
        private HighResolutionWait _waiter;

        public TimeFixingAttribute(int time)
        {
            _time = time;
            _waiter = new HighResolutionWait();
        }


        public void OnEntry()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public async void OnExit()
        {
            _stopwatch.Stop();
            int elapsedTime = _stopwatch.Elapsed.Milliseconds + 1;

            int sleepTime = _time - elapsedTime;

            _waiter.Wait(sleepTime);
            
        }

        #region Didnt Use
        public void Init(object instance, MethodBase method, object[] args)
        {
            
        }
        public void OnException(Exception exception)
        {
            Console.WriteLine(exception);
        }
        #endregion
    }
}