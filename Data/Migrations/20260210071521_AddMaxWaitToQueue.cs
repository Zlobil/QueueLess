using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueLess.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxWaitToQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxWaitMinutes",
                table: "Queues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 1,
                column: "MaxWaitMinutes",
                value: 30);

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 2,
                column: "MaxWaitMinutes",
                value: 60);

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 3,
                column: "MaxWaitMinutes",
                value: 30);

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 4,
                column: "MaxWaitMinutes",
                value: 45);

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 5,
                column: "MaxWaitMinutes",
                value: 60);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxWaitMinutes",
                table: "Queues");
        }
    }
}
