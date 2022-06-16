// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using TradingAutomationWorker.Domain.Entity;

namespace TradingAutomationWorker.EF
{
    internal class TAContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=TradingAutomation;Persist Security Info=True;User ID=sa;Password=123;");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<SymbolPrice> SymbolPrices { get; set; }
    }
}
