using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QueueLess.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialDatas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ServiceLocations",
                columns: new[] { "Id", "Address", "CreatedOn", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "12 Industrial Street, Sofia", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "AutoFix Garage", "+359888123456" },
                    { 2, "8 Main Blvd, Plovdiv", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Downtown Barber Shop", "+359887222333" },
                    { 3, "25 Health St, Varna", new DateTime(2026, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Smile Dental Clinic", "+359889333444" },
                    { 4, "14 Tech Ave, Sofia", new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "SmartFix Phone Repair", "+359886444555" },
                    { 5, "1 Freedom Sq, Burgas", new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Municipal Services Office", "+359885555666" }
                });

            migrationBuilder.InsertData(
                table: "Queues",
                columns: new[] { "Id", "AverageServiceTimeMinutes", "CreatedOn", "Description", "IsOpen", "Name", "ServiceLocationId" },
                values: new object[,]
                {
                    { 1, 30, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Engine oil and oil filter replacement.", true, "Oil Change Service", 1 },
                    { 2, 45, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Brake system inspection and diagnostics.", true, "Brake Inspection", 1 },
                    { 3, 25, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Classic men's haircut and styling.", true, "Men's Haircut", 2 },
                    { 4, 20, new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Routine dental examination.", true, "Dental Check-up", 3 },
                    { 5, 40, new DateTime(2026, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Smartphone screen replacement service.", true, "Phone Screen Replacement", 4 }
                });

            migrationBuilder.InsertData(
                table: "QueueEntries",
                columns: new[] { "Id", "ClientName", "JoinedOn", "QueueId" },
                values: new object[,]
                {
                    { 1, "Ivan Petrov", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Georgi Dimitrov", new DateTime(2026, 1, 15, 9, 30, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, "Petar Kolev", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 4, "Maria Ivanova", new DateTime(2026, 1, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 5, "Nikolay Stoyanov", new DateTime(2026, 1, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "QueueEntries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "QueueEntries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "QueueEntries",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "QueueEntries",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "QueueEntries",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServiceLocations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ServiceLocations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceLocations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServiceLocations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ServiceLocations",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
