using Domain.Enum;
using Domain.Model;
using Domain.Test.Model;
using TradingAutomationWorker.EF;
using TradingAutomationWorker.Job;
using TradingAutomationWorker.Test;

namespace TradingAutomationWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly List<Domain.Entity.SymbolPrice> _symbolPrices;
        //private readonly Spot2 _spot2;
        //private readonly Spot3 _spot3;
        //private readonly Spot2Binance _spot2Binance;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            #region Veritabanından currency price datasının çekilir
            if (_symbolPrices is null || _symbolPrices.Count == 0)
            {

                using TAContext context = new();
                _symbolPrices = context.SymbolPrices.Where(a => a.SymbolType == SymbolTypeEnum.BTCUSDT).OrderBy(a => a.TradingHour).ToList();
            }
            #endregion

            #region İşlemler için fake wallet datası oluşturulur
            //List<TestWalletData> fakeTaskWalletDataList = new List<TestWalletData>()
            //{
            //    new TestWalletData()
            //    {
            //        TaskName = "test1",
            //        TaskWalletList = new List<SpotWallet>()
            //        {
            //            new SpotWallet()
            //            {
            //                CurrencyType = CurrencyEnum.BTC,
            //                Quantity = 0,
            //                IsBaseAsset = true
            //            },
            //            new SpotWallet()
            //            {
            //                CurrencyType = CurrencyEnum.USDT,
            //                Quantity = 30,
            //                IsBaseAsset = false
            //            }
            //        }
            //    }
            //};

            //List<TestTaskData> taskDatalist = new List<TestTaskData>()
            //{
            //    new TestTaskData()
            //    {
            //        TaskName = "test1",
            //        TradingStatus = SpotTradingStatusEnum.InUSDT
            //    }
            //};
            //_spot2 = new(fakeTaskWalletDataList, taskDatalist);
            #endregion


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                #region zaman dilimlerini tek tek gönderen yöntem(gerçek uygulamabuna uygun olacak) 
                //Task.Run(() =>
                //{
                //    CommonJob.GetPrice(TradingPairEnum.BTCUSDT, TradingPairEnum.BTCBUSD);
                //});

                //M1 m1 = new M1();
                //IsolatedMarginAccountInfo accountInfo = m1.GetMarginAccountInfo(TradingPairEnum.BTCUSDT, TradingPairEnum.BTCBUSD).Result;
                //m1.TransferSpotAssetToMarginWallet(accountInfo);

                //await Task.Delay(1000, stoppingToken);

                //Spot spotTest = new();
                //spotTest.Test1(symbolPrices);


                //var test = _symbolPrices.AsEnumerable().GroupBy(a => new DateTime(a.TradingHour.Year, a.TradingHour.Month, a.TradingHour.Day, a.TradingHour.Hour, 0, 0)).Select(a => a.ToList()).ToList();
                //decimal counter1 = 0;
                //decimal counter2 = 0;

                //foreach (var item in test)
                //{
                //    counter1 += item.Max(a => a.High) - item.First().Opening;
                //    counter2++;
                //}

                //var result = counter1 / counter2;


                foreach (TradingAutomationWorker.Domain.Entity.SymbolPrice item in _symbolPrices)
                {
                    List<decimal> priceList = new();
                    if (item.Opening >= item.Closing)
                        priceList = new() { item.Opening, item.Low, item.High, item.Closing };
                    else
                        priceList = new() { item.Opening, item.High, item.Low, item.Closing };

                    foreach (decimal price in priceList)
                    {
                        //_spot2.Execute("test1", price, item.TradingHour, 50, 1000, price * 0.1m);
                    }
                }
                #endregion

                #region saatlere göre gruplama
                //var test = symbolPrices.AsEnumerable().GroupBy(a => new DateTime(a.TradingHour.Year, a.TradingHour.Month, a.TradingHour.Day, a.TradingHour.Hour, 0, 0)).Select(a => a.ToList()).ToList();

                //decimal btc = 0;
                //decimal usdt = 30;
                //DateTime? oldDate = null;

                //foreach (var item in test)
                //{
                //    if (oldDate != null && oldDate.Value.Month != item.First().TradingHour.Date.Month)
                //    {
                //        Console.WriteLine($"{item.First().TradingHour.Date.Month} : {usdt}");
                //    }

                //    decimal opening = item.First().Opening;
                //    decimal max = item.Max(a => a.High);
                //    decimal min = item.Min(a => a.Low);
                //    decimal closing = item.Last().Closing;

                //    SymbolPrice maxItem = item.Where(a => a.High == max).First();
                //    SymbolPrice minItem = item.Where(a => a.Low == min).First();
                //    bool isFirstMax = 

                //    if (usdt > opening * 0.1m)
                //    {
                //        btc = (opening * 0.1m) / opening;
                //        usdt -= opening * 0.1m;
                //    }
                //    else
                //    {
                //        btc = usdt / opening;
                //        usdt = 0;
                //    }

                //    if (max - opening > 100)
                //    {
                //        usdt += btc * (opening + 100);
                //        btc = 0;
                //    }
                //    else if (opening - min > 100)
                //    {
                //        usdt += btc * (opening - 100);
                //        btc = 0;
                //    }
                //    else
                //    {
                //        usdt += btc * closing;
                //        btc = 0;
                //    }

                //    oldDate = item.First().TradingHour.Date;
                //}
                #endregion

                #region binance history indirme kodları
                //List<TradingPairEnum> pairList = new() { TradingPairEnum.BTCUSDT, TradingPairEnum.BTCBUSD };
                //using (var client = new WebClient())
                //{
                //    foreach (TradingPairEnum pair in pairList)
                //    {
                //        for (DateTime i = new DateTime(2021, 3, 1); i < DateTime.Today; i = i.AddDays(1))
                //        {
                //            string destFile = Path.Combine($"\\CryptoCurrencyKlineData\\{pair}", $"{pair}-1m-{i.ToString("yyyy-MM-dd")}.zip");
                //            if (!File.Exists(destFile))
                //            {
                //                client.DownloadFile($"https://data.binance.vision/data/spot/daily/klines/{pair}/1m/{pair}-1m-{i.ToString("yyyy-MM-dd")}.zip", destFile);
                //                Console.WriteLine(destFile);
                //            }
                //            //client.DownloadFile($"https://data.binance.vision/data/spot/daily/klines/{pair}/1m/{pair}-1m-{i.ToString("yyyy-MM-dd")}.zip.CHECKSUM", destFile + "zip.CHECKSUM");
                //        }
                //    }
                //} 
                #endregion

                #region binance history database kayıt kodları
                //List<TradingPairEnum> pairList = new() { TradingPairEnum.BTCUSDT, TradingPairEnum.BTCBUSD };
                //List<SymbolPrice> dataList = new();

                //foreach (TradingPairEnum pair in pairList)
                //{
                //    for (DateTime i = new DateTime(2021, 3, 1); i < DateTime.Today; i = i.AddDays(1))
                //    {
                //        string destFile = Path.Combine($"\\CryptoCurrencyKlineData\\{pair}\\{pair}-1m-{i.ToString("yyyy-MM-dd")}", $"{pair}-1m-{i.ToString("yyyy-MM-dd")}.csv");
                //        if (File.Exists(destFile))
                //        {
                //            string[] rows = File.ReadAllLines(destFile);
                //            foreach (string row in rows)
                //            {
                //                string[] data = row.Split(",");
                //                dataList.Add(new SymbolPrice()
                //                {
                //                    TradingHour = new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(data[0])),
                //                    SymbolType = pair,
                //                    Opening = Convert.ToDecimal(data[1].ToString().Replace(".", ",")),
                //                    High = Convert.ToDecimal(data[2].ToString().Replace(".", ",")),
                //                    Low = Convert.ToDecimal(data[3].ToString().Replace(".", ",")),
                //                    Closing = Convert.ToDecimal(data[4].ToString().Replace(".", ",")),
                //                    AddedDate = DateTime.Now,
                //                    IsActive = true
                //                }); ;
                //            }
                //        }
                //        else
                //        {
                //            Console.WriteLine($"{pair}-1m-{i.ToString("yyyy - MM - dd")}", $"{pair}-1m-{i.ToString("yyyy-MM-dd")}.csv Dosyası Bulunamadı");
                //        }
                //    }
                //}

                //using (TAContext context = new())
                //{
                //    context.SymbolPrices.AddRange(dataList);
                //    context.SaveChanges();
                //}
                #endregion
            }
        }
    }
}

//todo: maks ve min bilgilerini m1job modele ekleyn fonksiyon yaz