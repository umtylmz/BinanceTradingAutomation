using AzureTest.DataAccessLayer.Concrete;
using AzureTest.Job;
using Domain.Enum;
using Domain.Model;
using Domain.Test.Model;
using Helper;

namespace AzureTest
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        //private readonly Spot2Binance _spot2Binance;
        //private readonly IsolatedMarginV1 _isolatedMarginV1;
        private readonly IsolatedMarginV2 _isolatedMarginV2;
        private readonly IsolatedMarginV2TaskDetailDAL _isolatedMarginV2TaskDetailDAL;
        private List<SymbolPrice> livePriceList = new();
        private SymbolPrice BTCUSDTPrice = new();
        private const string BTCUSDTSpotLive1 = "BTCUSDTSpotLive1";
        private const string IsolatedMarginV1T1 = "IsolatedMarginV1T1";
        private const string IsolatedMarginV2T1 = "IsolatedMarginV2T1";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            //_isolatedMarginV1 = new IsolatedMarginV1();
            _isolatedMarginV2 = new IsolatedMarginV2();
            _isolatedMarginV2TaskDetailDAL = new();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                #region GERÇEK İŞLEM ALANI
                try
                {
                    #region cosmos db test
                    //IsolatedMarginV2TaskDetail taskDetail = new()
                    //{
                    //    InitialBuyingAmount = 1,
                    //    BuyingOrderId = "4",
                    //    BuyingPrice = 34567,
                    //    IsIsolated = true,
                    //    LossLayer = 4,
                    //    RecvWindow = 10000,
                    //    SellingOrderID = "55555",
                    //    SymbolType = SymbolTypeEnum.BTCBUSD,
                    //    TaskName = "deneme1",
                    //    TotalPayment = 34,
                    //    TradingStatus = IsolatedMarginV1StatusEnum.Started,
                    //    Id = "deneme1"
                    //};

                    //IsolatedMarginV2TaskDetail taskDetail2 = new()
                    //{
                    //    InitialBuyingAmount = 1,
                    //    BuyingOrderId = "4",
                    //    BuyingPrice = 34567,
                    //    IsIsolated = true,
                    //    LossLayer = 4,
                    //    RecvWindow = 10000,
                    //    SellingOrderID = "55555",
                    //    SymbolType = SymbolTypeEnum.BTCBUSD,
                    //    TaskName = "a2",
                    //    TotalPayment = 34,
                    //    TradingStatus = IsolatedMarginV1StatusEnum.Started,
                    //    Id = "aa2"
                    //};

                    //var aa = _isolatedMarginV2TaskDetailDAL.Add(taskDetail2).Result;
                    //var bb = _isolatedMarginV2TaskDetailDAL.Get(a => a.TaskName == taskDetail.TaskName).Result;

                    //bb.LossLayer = 6;
                    //var cc = _isolatedMarginV2TaskDetailDAL.Update(bb); 
                    #endregion

                    //livePriceList = CommonJob.GetPriceReal(SymbolTypeEnum.BTCUSDT);
                    //BTCUSDTPrice = livePriceList.First(a => a.Symbol == SymbolTypeEnum.BTCUSDT);

                    //await _spot2Binance.Execute(BTCUSDTSpotLive1, BTCUSDTPrice.Price, BTCUSDTPrice.AddedDate, 10000, 25, BTCUSDTPrice.Price * 0.1m);
                    //await _isolatedMarginV1.Execute(IsolatedMarginV1T1, SymbolTypeEnum.BTCUSDT, BTCUSDTPrice.Price, 12);
                    await _isolatedMarginV2.Execute(IsolatedMarginV2T1, SymbolTypeEnum.BTCUSDT);
                }
                catch (Exception)
                {
                    LogHelper.LogMessage($"Price bilgisi alınamadı.", MessageTypeEnum.ERROR);
                }
                #endregion

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}