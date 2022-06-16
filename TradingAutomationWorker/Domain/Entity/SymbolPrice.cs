using Domain.Entity;
using Domain.Enum;

namespace TradingAutomationWorker.Domain.Entity
{
    public class SymbolPrice : BaseEntity
    {
        public DateTime TradingHour { get; set; }
        public SymbolTypeEnum SymbolType { get; set; }
        public decimal Opening { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Closing { get; set; }
    }
}
