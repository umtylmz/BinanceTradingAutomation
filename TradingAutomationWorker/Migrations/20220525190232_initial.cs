using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingAutomationWorker.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SymbolPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradingHour = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SymbolType = table.Column<int>(type: "int", nullable: false),
                    Opening = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Closing = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymbolPrices", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SymbolPrices");
        }
    }
}
