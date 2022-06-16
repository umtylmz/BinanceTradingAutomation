// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Binance.Spot;
using Domain.Common;
using Domain.Interface;
using Domain.Test.Model;

namespace Domain.Abstract
{
    public class TaskAbstract
    {
        protected static Dictionary<string, IWallet>? WalletDictionary { get; set; } = new();
        protected static Dictionary<string, ITaskDetail>? TaskDetailDictionary { get; set; } = new();
        protected static Wallet WalletBinanceService { get; set; } = new(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);
        protected static SpotAccountTrade SpotAccountTradeBinanceService { get; set; } = new(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);
        protected static MarginAccountTrade MarginAccountTradeBinanceService { get; set; } = new(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);

        public TaskAbstract(List<TestWallet> testWalletList = null, List<TestTaskDetail> testTaskDetailList = null)
        {
            if (testWalletList != null)
            {
                foreach (TestWallet testWallet in testWalletList)
                {
                    if (!WalletDictionary.ContainsKey(testWallet.TaskName))
                        WalletDictionary.Add(testWallet.TaskName, testWallet.TaskWalletList);
                }
            }

            if (testTaskDetailList != null)
            {
                foreach (TestTaskDetail testTaskDetail in testTaskDetailList)
                {
                    if (!TaskDetailDictionary.ContainsKey(testTaskDetail.TaskName))
                        TaskDetailDictionary.Add(testTaskDetail.TaskName, testTaskDetail.TaskDetail);
                }
            }
        }
    }
}
