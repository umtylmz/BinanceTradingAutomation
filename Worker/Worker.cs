using Binance.Spot;

namespace Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                MarginAccountTrade marginAccountTrade = new MarginAccountTrade(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);
                var openOrders = marginAccountTrade.QueryMarginAccountsOpenOrders();





                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}