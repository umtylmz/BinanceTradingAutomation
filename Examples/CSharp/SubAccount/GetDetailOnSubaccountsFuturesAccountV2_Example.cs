namespace Binance.Spot.SubAccountExamples
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Binance.Common;
    using Binance.Spot;
    using Binance.Spot.Models;
    using Microsoft.Extensions.Logging;

    public class GetDetailOnSubaccountsFuturesAccountV2_Example
    {
        public static async Task Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<GetDetailOnSubaccountsFuturesAccountV2_Example>();

            HttpMessageHandler loggingHandler = new BinanceLoggingHandler(logger: logger);
            HttpClient httpClient = new HttpClient(handler: loggingHandler);

            var subAccount = new SubAccount(httpClient);

            var result = await subAccount.GetDetailOnSubaccountsFuturesAccountV2("abc@test.com", FuturesType.USDT_MARGINED_FUTURES);
        }
    }
}