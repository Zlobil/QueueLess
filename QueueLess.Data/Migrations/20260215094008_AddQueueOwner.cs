using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueLess.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQueueOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Queues",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "11111111-1111-1111-1111-111111111111", 0, "e4477e23-36cf-4681-84d0-68c28928bb7d", "admin@queueless.com", true, false, null, "ADMIN@QUEUELESS.COM", "ADMIN@QUEUELESS.COM", "AQAAAAIAAYagAAAAEN2BmhClDkBRAWi2DVcAeNFaftmyA4mFULgWeZ5JYPVTTeVKTXDDM1SS/xvPb5KLeQ==", null, false, "cacaa912-3b0a-4929-8ade-5777cb6bfb68", false, "admin@queueless.com" });

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaxWaitMinutes", "OwnerId" },
                values: new object[] { 30, "11111111-1111-1111-1111-111111111111" });

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MaxWaitMinutes", "OwnerId" },
                values: new object[] { 30, "11111111-1111-1111-1111-111111111111" });

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "MaxWaitMinutes", "OwnerId" },
                values: new object[] { 30, "11111111-1111-1111-1111-111111111111" });

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "MaxWaitMinutes", "OwnerId" },
                values: new object[] { 30, "11111111-1111-1111-1111-111111111111" });

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "MaxWaitMinutes", "OwnerId" },
                values: new object[] { 30, "11111111-1111-1111-1111-111111111111" });

            migrationBuilder.CreateIndex(
                name: "IX_Queues_OwnerId",
                table: "Queues",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Queues_AspNetUsers_OwnerId",
                table: "Queues",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queues_AspNetUsers_OwnerId",
                table: "Queues");

            migrationBuilder.DropIndex(
                name: "IX_Queues_OwnerId",
                table: "Queues");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "11111111-1111-1111-1111-111111111111");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Queues");

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 1,
                column: "MaxWaitMinutes",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 2,
                column: "MaxWaitMinutes",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 3,
                column: "MaxWaitMinutes",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 4,
                column: "MaxWaitMinutes",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: 5,
                column: "MaxWaitMinutes",
                value: 0);
        }
    }
}
