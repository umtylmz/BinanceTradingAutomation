using Domain.Enum;

namespace Domain.Entity
{
    public class SymbolPrice : BaseEntity
    {
        public SymbolTypeEnum SymbolType { get; set; }
        public decimal Price { get; set; }
    }
}
