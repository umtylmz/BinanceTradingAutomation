// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

namespace TradingAutomationApi.Core.DataAccess.EntityFramework
{
    public class EFContext<TContext> where TContext : DbContext, new()
    {
        public static TContext GetInstance()
        {
            return new TContext();
        }
    }
}
