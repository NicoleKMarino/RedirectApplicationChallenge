using Microsoft.Extensions.Logging;
using static RedirectApplication.Redirect;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace RedirectApplication.Services
{
    internal class RedirectService : AsyncService
    {
        private readonly Redirect _redirectHelper;
        private readonly ILogger _logger;

        public RedirectService(Redirect redirectHelper, TimeSpan period, ILogger<RedirectService> logger) : base(period)
        {
            _redirectHelper = redirectHelper;
            _logger = logger;
        }

        protected override async Task StartIntervalAsync(CancellationToken endingInput)
        {
            try
            {
                _logger.LogInformation("Refreshing redirect");

                await _redirectHelper.Refresh();

                _logger.LogInformation("Refreshing Complete");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "There was an error");
            }
        }
    }
}
