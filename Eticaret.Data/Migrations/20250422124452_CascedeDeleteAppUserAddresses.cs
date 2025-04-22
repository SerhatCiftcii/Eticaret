using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class CascedeDeleteAppUserAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AppUsers_AppUserId",
                table: "Addresses");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 4, 22, 15, 44, 52, 37, DateTimeKind.Local).AddTicks(828), new Guid("7810ac90-044d-47dc-8101-0172f19e7c48") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 4, 22, 15, 44, 52, 37, DateTimeKind.Local).AddTicks(3037));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 4, 22, 15, 44, 52, 37, DateTimeKind.Local).AddTicks(3050));

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AppUsers_AppUserId",
                table: "Addresses",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AppUsers_AppUserId",
                table: "Addresses");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 4, 3, 23, 17, 54, 480, DateTimeKind.Local).AddTicks(3112), new Guid("12282183-2b35-4bae-8e4d-b5cfd1759b3c") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 4, 3, 23, 17, 54, 480, DateTimeKind.Local).AddTicks(5406));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 4, 3, 23, 17, 54, 480, DateTimeKind.Local).AddTicks(5417));

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AppUsers_AppUserId",
                table: "Addresses",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }
    }
}
