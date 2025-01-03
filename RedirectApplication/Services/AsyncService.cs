using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedirectApplication.Services
{
    internal abstract class AsyncService : BackgroundService
    {
        private TimeSpan _period;

        protected AsyncService(TimeSpan period)
        {
            _period = period;
        }

        protected abstract Task StartIntervalAsync(CancellationToken endingInput);

        protected sealed override async Task ExecuteAsync(CancellationToken endingInput)
        {
            using var timer = new PeriodicTimer(_period);

            do
            {
                await StartIntervalAsync(endingInput);
            } while (!endingInput.IsCancellationRequested && await timer.WaitForNextTickAsync(endingInput));
        }

    }
}
