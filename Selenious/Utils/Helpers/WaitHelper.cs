namespace Phoenix.Core.Utils.Helpers
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    public static class WaitHelper
    {
        /// <summary>
        /// Times out after desired amount of time, when boolean condition is false and dislays desired message.
        /// </summary>
        public static void Wait(Func<bool> condition, TimeSpan timeout, string message)
        {
            Wait(condition, timeout, TimeSpan.FromSeconds(1), message);
        }

        /// <summary>
        /// Times out after desired amount of time, when boolean condition is false and dislays desired message.
        /// </summary>
        public static void Wait(Func<bool> condition, TimeSpan timeout, TimeSpan sleepInterval, string message)
        {
            var result = Wait(condition, timeout, sleepInterval);

            if (!result)
            {
                throw new TimeoutException(string.Format(CultureInfo.CurrentCulture, "Timeout after {0} second(s), {1}", timeout.TotalSeconds, message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool Wait(Func<bool> condition, TimeSpan timeout, TimeSpan sleepInterval)
        {
            var result = false;
            var start = DateTime.Now;
            var canceller = new CancellationTokenSource();
            var task = Task.Factory.StartNew(condition, canceller.Token);

            while ((DateTime.Now - start).TotalSeconds < timeout.TotalSeconds)
            {
                if (task.IsCompleted)
                {
                    if (task.Result)
                    {
                        result = true;
                        canceller.Cancel();
                        break;
                    }

                    task = Task.Factory.StartNew(
                        () =>
                            {
                                using (canceller.Token.Register(Thread.CurrentThread.Abort))
                                {
                                    return condition();
                                }
                            },
                        canceller.Token);
                }

                Thread.Sleep(sleepInterval);
            }

            canceller.Cancel();
            return result;
        }
    }
}
