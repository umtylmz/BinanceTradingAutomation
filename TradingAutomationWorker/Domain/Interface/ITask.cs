// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace TradingAutomationWorker.Domain.Interface
{
    internal interface ITask
    {
        internal static Dictionary<string, List<object>> TaskList { get; set; } = new();
        public void Execute();
    }
}
