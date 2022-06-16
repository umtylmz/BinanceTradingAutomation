// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using TradingAutomationWorker.Domain.Interface;

namespace TradingAutomationWorker.Domain.Abstract
{
    internal abstract class TaskAbstract : ITask
    {
        public abstract void Execute();
    }
}
