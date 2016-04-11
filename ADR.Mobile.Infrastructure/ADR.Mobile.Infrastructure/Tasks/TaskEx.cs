using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Tasks
{
    public class TaskEx
    {
        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay in milliseconds before the returned task completes.</param>
        /// <returns>
        /// The timed Task.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Timer is disposed in the timer callback")]
        public static Task Delay(int dueTime)
        {
            if (dueTime <= 0) throw new ArgumentOutOfRangeException("dueTime");

            var tcs = new TaskCompletionSource<bool>();
            var timer = new Timer(self =>
            {
                ((Timer)self).Dispose();
                tcs.TrySetResult(true);
            });
            timer.Change(dueTime, Timeout.Infinite);
            return tcs.Task;
        }
    }
}
