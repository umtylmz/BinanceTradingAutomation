namespace Binance.Spot.SavingsExamples
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Binance.Common;
    using Binance.Spot;
    using Binance.Spot.Models;
    using Microsoft.Extensions.Logging;

    public class ChangeFixedActivityPositionToDailyPosition_Example
    {
        public static async Task Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<ChangeFixedActivityPositionToDailyPosition_Example>();

            HttpMessageHandler loggingHandler = new BinanceLoggingHandler(logger: logger);
            HttpClient httpClient = new HttpClient(handler: loggingHandler);

            var savings = new Savings(httpClient);

            var result = await savings.ChangeFixedActivityPositionToDailyPosition("BTC001", 1);
        }
    }
}