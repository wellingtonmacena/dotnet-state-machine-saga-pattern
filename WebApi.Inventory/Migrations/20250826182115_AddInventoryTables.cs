using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApi.Inventory.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Storages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    StorageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockItems_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "Manufacturer", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("3a1f2c44-5f6d-4e5e-9b3f-21a7e8d1c001"), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc), "128GB, Titânio", "Apple", "iPhone 15 Pro", 9999.99m, new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("7f4b8c36-9f1a-4b0a-9f92-1f0a3e6c2d55"), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc), "34 polegadas, 144Hz", "LG", "Monitor LG UltraWide", 2499.50m, new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d4f1c872-5b6a-4d3f-9f73-7e8c9fda9009"), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc), "Ultrabook com tela 13.3”", "Dell", "Notebook Dell XPS 13", 8500.00m, new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Storages",
                columns: new[] { "Id", "CreatedAt", "UpdatedAt" },
                values: new object[] { new Guid("c8e2f91a-6f2b-4c6e-81a1-6c3d8f7a9002"), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "StockItems",
                columns: new[] { "Id", "CreatedAt", "Location", "ProductId", "Quantity", "StorageId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5c3b7a18-7e61-41d2-8322-0f9d4cfa5005"), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc), "Depósito B", new Guid("7f4b8c36-9f1a-4b0a-9f92-1f0a3e6c2d55"), 20, new Guid("c8e2f91a-6f2b-4c6e-81a1-6c3d8f7a9002"), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("9e8f4c72-27a9-4e5d-9e33-5a4b6d8a4004"), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc), "Depósito A", new Guid("3a1f2c44-5f6d-4e5e-9b3f-21a7e8d1c001"), 5, new Guid("c8e2f91a-6f2b-4c6e-81a1-6c3d8f7a9002"), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b27a1e56-1c47-4e89-ae63-2b49cfa49003"), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc), "Depósito A", new Guid("d4f1c872-5b6a-4d3f-9f73-7e8c9fda9009"), 10, new Guid("c8e2f91a-6f2b-4c6e-81a1-6c3d8f7a9002"), new DateTime(2025, 8, 26, 14, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_ProductId",
                table: "StockItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_StorageId",
                table: "StockItems",
                column: "StorageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockItems");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Storages");
        }
    }
}
